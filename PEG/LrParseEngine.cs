using System.Collections.Generic;
using System.Linq;
using PEG.SyntaxTree;
using PEG.Utils;

namespace PEG
{
    /// <summary>
    /// Enhances the default PEG parsing engine to support left-recursion.
    /// </summary>
    public class LrParseEngine : ParseEngine
    {
        /// <summary>
        /// Each entry corresponds to the index into the input.  Each time left-recursion is instigated,
        /// A LeftRecursion entry is placed into this array.  If one is already present, a new one will
        /// be "pushed" into the array, keeping a reference to the old one -- this one will be restored
        /// once the new left-recursion is finished.
        /// </summary>
        public LeftRecursion[] LeftRecursionSet { get; set; }

        /// <summary>
        /// Each entry corresponds to the index into the input.  Within the BooleanSet, each entry
        /// maps the index of a nonterminal with a boolean state indicating whether the nonterminal
        /// is currently being invoked.
        /// </summary>
        public BooleanSet[] InvocationSet { get; set; }

        /// <summary>
        /// Reproduces the nonterminal callstack so we can accurately back-calculate the EvalSet when
        /// left-recursion "happens".
        /// </summary>
        public Stack<Nonterminal> CallStack { get; set; }

        public LrParseEngine(Grammar grammar, string input) : base(grammar, input)
        {
            Init();
        }

        public LrParseEngine(Grammar grammar, string rawInput, IParseInput input) : base(grammar, rawInput, input)
        {
            Init();
        }

        private void Init()
        {
            LeftRecursionSet = new LeftRecursion[Input.Length + 1];
            InvocationSet = new BooleanSet[Input.Length + 1];
            for (int i = 0; i < InvocationSet.Length; i++)
                InvocationSet[i] = new BooleanSet(Grammar.Nonterminals.Count);
            CallStack = new Stack<Nonterminal>();
        }

        public override ParseOutputSpan ApplyNonterminal(Nonterminal nonterminal, int position)
        {
            Depth++;

            try
            {
                MemoEntry? memoEntry = MemoTable[position, nonterminal.Index];
                LeftRecursion leftRecursion = LeftRecursionSet[position];
                if (memoEntry == null || (leftRecursion != null && leftRecursion.InvolvedSet.Contains(nonterminal.Index)))
                {
                    // If we've detected left recursion...
                    if (InvocationSet[position][nonterminal.Index])
                    {
                        // If there is no left recursion already going on...
                        if (leftRecursion == null)
                        {
                            // Create a new left recursion entry and store it in the LeftRecursionSet for the current position.
                            leftRecursion = new LeftRecursion(nonterminal, Grammar);
                            LeftRecursionSet[position] = leftRecursion;
                        }

                        // If there already is left-recursion going on, we may need to start keeping track of a new thread. Fortunately,
                        // we can know that the new left-recursive state will end before we have to worry about the old one again, so
                        // we can create a linked-list-based stack where each "Next" entry is further up the call stack.  The inverse
                        // of this process is near the end.
                        else if (leftRecursion.Rule != nonterminal)
                        {
                            // This is here because we don't ever want to add an entry to the stack that is already being evaluated.
                            if (leftRecursion.EvalSet.Contains(nonterminal.Index))
                                return new ParseOutputSpan(false);

                            var nextLeftRecursion = leftRecursion;
                            leftRecursion = new LeftRecursion(nonterminal, nextLeftRecursion, Grammar);
                        }

                        // This ensures that we keep track of those nonterminals that were involved in the left recursion. (i.e.
                        // all the nonterminals in the call stack following the nonterminal responsible for the left-recursion.
                        // This is non-inclusive -- it does not include the offending nonterminal located on both sides.)  All such
                        // nonterminals are added to the InvolvedSet of the LeftRecursion record.
                        leftRecursion.Add(CallStack.TakeWhile(o => o != nonterminal));

                        // Keep track of this LeftRecursion at the current position (possibly overwriting a previous entry -- this
                        // entry will be restored when the current left recursion finishes)
                        LeftRecursionSet[position] = leftRecursion;

                        // Return the special LeftRecursion value that indicates a (special) failed attempt.  Below, we check for
                        // this value to ensure that the memo table is not updated with the failure when left-recursion is still
                        // going on (i.e. we will need to make another attempt later and don't want to have it return a failure
                        // because of the memo result)
                        return new ParseOutputSpan(false);
                    }
                    else
                    {
                        memoEntry = new MemoEntry(False, position);
                        bool first = true;
                        ParseOutputSpan answer;
                        while (true)
                        {
                            // Reset the position
                            Position = position;

                            // Update the invocation set and call stack
                            InvocationSet[position][nonterminal.Index] = true;
                            CallStack.Push(nonterminal);

                            // Evaluate the nonterminal
                            answer = nonterminal.Eval(this);

                            // Reset the invocation set and call stack
                            CallStack.Pop();
                            InvocationSet[position][nonterminal.Index] = false;

                            // Except for the first iteration, we never want to continue the iteration (or the loop) if the
                            // evaluation failed to increase the Position.  (The first time through we do need to finish the
                            // iteration as the rest of the code takes care of bookkeeping required to note a failed attempt.)
                            if (memoEntry.Value.Position >= Position && !first)
                            {
                                // Update the position and answer since we are going to revert back to the state in the memo entry
                                Position = memoEntry.Value.Position;
                                answer = memoEntry.Value.Answer;
                                break;
                            }

                            bool processing = false;

                            // If we have an answer (and it's not the special LeftRecursion failure), then we want to store the
                            // result in the memo table.  If it is the special LeftRecursion failure, we don't because that's the
                            // whole reason for the special value.
                            if (!answer.IsFailed && !answer.IsLeftRecursion)
                            {
                                // We want to catch any case where the result was overwritten, as that fact invalidates the current
                                // nonterminal's parse and will force a new iteration of the parse loop.
                                if (!memoEntry.Value.Answer.IsFailed && memoEntry.Value.Position < Position)
                                {
                                    processing = true;
                                }

                                // Update the memo entry
                                MemoTable[position, nonterminal.Index] = memoEntry = new MemoEntry(answer, Position, answer.GetRecords(Output).ToArray());
                            }

                            first = false;

                            // We want to end the loop as early as possible.  For the vast majority of cases, there is no left-recursion going
                            // on at the current context (position+nonterminal).  For these cases, we do not want to evaluate the nonterminal more
                            // than once.  Therefore, the loop will break if the following conditions are true:
                            //
                            // a) We are not specifically forcing a new iteration as specified above (processing)
                            // b) There is absolutely no left-recursion going on or there is, but this nonterminal is not the one being
                            //    recursed.
                            if (!processing && (LeftRecursionSet[position] == null || LeftRecursionSet[position].Rule != nonterminal))
                                break;
                        }

                        // Finally, record a failure if we found nothing even after a left-recursive attempt.
                        if (answer.IsFailed && !answer.IsLeftRecursion)
                        {
                            MemoTable[position, nonterminal.Index] = new MemoEntry(answer, position);
                        }

                        // Reverses the left-recursion process described above so that the LeftRecursionSet is always kept in an
                        // accurate state:  If this was the top of the LeftRecursion stack, the set is cleared, otherwise the
                        // current LeftRecursion is popped and the previous one restored.
                        leftRecursion = LeftRecursionSet[position];
                        if (leftRecursion != null && leftRecursion.Rule == nonterminal)
                        {
                            leftRecursion = leftRecursion.Next;
                            LeftRecursionSet[position] = leftRecursion;
                        }

                        return answer;
                    }
                }
                else
                {
                    Position = memoEntry.Value.Position;
                    foreach (var record in memoEntry.Value.Output)
                    {
                        Output.Add(record);
                    }
                    return memoEntry.Value.Answer;
                }
            }
            finally
            {
                Depth--;
            }
        }

        public ParseException CreateException()
        {
            var realPosition = MaxPosition;
            return new ParseException(RawInput, realPosition);
        }
    }
}
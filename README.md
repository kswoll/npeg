# Packrat Parser in C#
---
This parser is an implementation of a [Packrat Parser](http://en.wikipedia.org/wiki/Parsing_expression_grammar) with support for
[left-recursion](http://en.wikipedia.org/wiki/Left_recursion). The algorithm for left recursion is a modified version of
[Packrat parsers can support left recursion](http://dl.acm.org/citation.cfm?id=1328408.1328424). 

## Packrat Parsers

A normal PackRat parser utilizes a Parsing Expression Grammar (PEG) and parses input in linear time.  This parser does the same, 
but allows for PEGs that contain left-recursion, a staple for anyone familiar with BNF while still maintaining near linear-time.
A PEG is far more expressive than BNF, allowing for conditional, not, zero/one or more, and other useful operators.  The syntax 
of a standard PEG has been modified to allow the grammars to be expressed naturally using C# syntax.

This parser is intended to be just as convenient to use for building a full-scale grammar as for simple ad hoc search/replace in 
strings.  It allows for grammars as sophisticated as languages such as C# itself, to patterns that are simple and nearly as 
concise as regular expressions (but far more scrutable). Before going into depth on what each operator is about, let’s look at 
some small examples.

---

### Decimals

Let's start with a simple pattern that will match a decimal number such as 1, 42, 3.14, -9, etc. First let's create a pattern to handle 1:

``` c#
varr pattern = '0'.To('9');
```

This creates a pattern that accepts any character between 0 and 9, in other words, a digit.  To test this pattern for 1 we simply call the Match(...) method:

``` c#
bool result = pattern.Match("1");
```

Since `1` is a valid character, it would be accepted by this pattern.  What about `42`?  `4` is a valid character, and so is `2`, so will it accept this input?   The answer is no, because we have designed the pattern to only account for a single digit. Since `42` has two digits, it fails. We'll have to modify our pattern to support multiple digits.Fortunately, that is easy -- we can just use the `+` operator to indicate that one or more digits in a row are allowed:

``` c#
var pattern = +'0'.To('9');
```

The plus `+` unary prefix operator requires at least one instance of its operand to be in the input, but allows any number of valid subsequent instances.  In this case, `'0'.To('9')` is the operand so this allows any number of digits.  Thus, `42` is now allowed.

Clearly `3.14` will fail as we've done nothing to account for the decimal point.  Let's modify the pattern:

``` c#
var pattern = +'0'.To('9') + '.' + +'0'.To('9');
```

While this works, it's probably a bit vexing that `'0'.To('9')` is repeated twice.  We can easily fix that by simply declaring a variable:

``` c#
var digit = +'0'.To('9');
var pattern = digit + '.' + digit;
```

Unfortunately, with our change, it now requires the decimal point, so our first two examples now fail.  This is easy to fix using the optional operator, expressed as a tilde `~`:

``` c#
var pattern = digit + ~('.' + digit);
```

Finally, we need to handle the minus sign `-`:

``` c#
var pattern = ~'-'._() + digit + ~('.' + digit);
```

And there you have it, a pattern that can handle any integer or decimal number.

You may be asking yourself about `._()`.  The problem is that all the operators you've seen so far work on the class Expression. `.` is a char, and thus the `~` operator won't work on it.   One solution is to create an extension method that takes a char and returns a CharacterTerminal (a type of Expression) based off it.  That is what the `._()` extension method on char is, and hence the syntax.  It was chosen to be short and minimize distraction.

### Quoted Strings

To illustrate the use of some other operators, let's create a pattern to match quoted string literals as you see in many languages (such as C#).  Some valid values:  `"value"`, `""`, `"a \"quote\""`.   We'll start by creating a pattern that can match `"value"`:

``` c#
var pattern = '"' + +Peg.Any + '"';
```

That is, a double-quote, the special operator Any, and a final double-quote.  The Any operator will accept any possible character.  But wait, surly this includes the double-quote!  How then do you terminate the string?  The answer is to also use the not operator `!`:

``` c#
var pattern = '"' + +(!'"'._() + Peg.Any) + '"';
```

So the one-or-more operator + was used on the expression `(!'"'._() + Peg.Any)`.  The not operator never consumes input, but will cause the Sequence to fail.  In this case, that failure will mean that the one-or-more operator will stop consuming further characters.  At that point, we arrive at the final double-quote and accept it. 

Next, will this pattern successfully match the empty string, `""`?  Since we are using a one-or-more operator, it will not!  Since having no characters between the double-quotes is an acceptable match, we need to replace the operator with the zero-or-more operator:

``` c#
var pattern = '"' + -(!'"'._() + Peg.Any) + '"';
```

As you can see, we switched the `+` for the `-`; the minus sign indicates zero-or-more.  Since zero is less than one (and vice versa), hopefully the symbols should be easy to remember. For the finishing touch, we want to allow double-quote characters so long as they are properly escaped with the backslash:

``` c#
var pattern = '"' + -(!('"'._() | @"\") + Peg.Any | @"\\" | @"\""") + '"';
```

Where before it was a one-or-more of "not the double-quote and any character" now it is a one-or-more of "neither the double-quote nor the backslash plus any character or two backslashes in a row (an escaped backslash) or a backslash followed by a double-quote (an escaped double-quote)".  With just a short pattern like this, we have a powerful way to parse any double-quoted string, including the ability to escape the double-quote. 

It's true, some of this syntax may look pretty foreign.  But one thing to remember is that this is all C#.  This means that you get all the help that you're used to from C#:


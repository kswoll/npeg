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

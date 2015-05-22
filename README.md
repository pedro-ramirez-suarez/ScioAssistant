# Scio Assistant
A Cortana/Siri like assistant using DSL

## What is a DSL?
<p>
A domain-specific language (DSL) is a computer language specialized to a particular application domain. This is in contrast to a general-purpose language (GPL), which is broadly applicable across domains, and lacks specialized features for a particular domain.
</p>
### How to implement a <b>DSL</b>?
There are many tools to build DLSs, for .Net some of them are:
- Visual Studio
- Irony.Net
- Boo language
- ANTLR

For this demo I use <b>Irony.Net</b>, Irony.net allows us to define the grammar and provides a parser that we use to then translate the expression entered by the user in to a SQL query.


Voice or Speech recognition?

<b>Speech</b> recognition is the translation of spoken words into text.

<b>Voice</b> recognition refers to identify the speaker, not what they are saying.


###How to implement what we need?

For our purposes, we need speech recognition rather than voice recognition, we need to translate the user words into text that can be used by the parser so we can “understand” what the user said.
We can implement speech recognition in a few ways, but the two most easy are:

- Using native windows libraries (System.Speech), this is can be used by desktop and web apps.

- For web applications(our case), we can use webkitSpeechRecognition in javascript

This demo uses webkitSpeechRecognition and is configured for Spanish Mexico

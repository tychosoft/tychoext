= About Tychosoft Extensions

This package collects and centralizes testing for many common C# classes I use
in other packages. These used to be duplicated or vendored in other packages.
Since I often write C# service daemons, this collection focuses on that use
case, though these classes may have general uses in client applications as
well.

Tychosoft Extensions are meant to be usable on C# and dotnet generically and
should be fully buildable cross-platform. Most often they would be acquired as
a nuget package. They should also work fine producing native AOT code.
Currently I am testing and developing these classes with .NET 8.0. A simple
Makefile is provided to make it easy to build and test on Linux systems or in
wsl. One can also use Visual Studio / Visual Studio Code for development.

## Dependencies

Tychosoft Extensions makes use of the Microsoft Logging extension, in a new and
more effective stand-alone cross-platform componentized manner. Otherwise there
are no other C# packages this package currently depends on.

## Distributions

The primary means of distribution is as a nuget package and the latest release
should be made available thru the nuget packaging site. A stand-alone detached
source tarball may also be produced from a repository checkout. It can also be
possible to vendor these extensions into another project using git submodules.

## Participation

This project is offered for public use and has a public project page at
https://www.github.com/tychosoft/tychoext which has an issue tracker where you
can submit public bug reports, a wiki for hosting project documentation, and a
public git repository. Patches and merge requests may be submitted in the issue
tracker or thru email. Support requests and other kinds of inquiries may also
be sent thru the tychosoft gitlab help desktop service. Other details about
participation may be found in the Contributing page.


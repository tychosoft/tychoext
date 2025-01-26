# Features

Tychosoft Extensions consist of a series of stand-alone extensions classes for
the C# language and are typically installed as a nupkg file. While they are
general purpose, many were from work on C# telephony service daemons. All
classes are imported under the Tychosoft.Extensions namespace.

## atomic.cs

At the moment this is just a wrapper for atomic counter types. It may include
atomic containers in the future.

## deque.cs

This provides a stable wrapper around linked lists for a deque. This class is
uses for tasks.

## logger.cs

This is a generic stand-alone logger system that is implimented as a pure
static class. This means you do not have to create instances of loggers
everywhere in your code. There is a component Startup and Shutdown method you
probably would call from your main(), and then you just call various static
logger member functions directly.

## scan.cs

The ScanFile and ScanDir classes are used to execute lambda functors from
each line of a text file or each entry in a directory. The idea behind this
is found in the behavior of the Ruby file library.

## tasks.cs

This allows you to break down and call function methods thru a dispatch
queue. The queue is consumed in a single thread context, and the function
requests are processed in order. This allows for writing components where
you want to have all operations that manipulate private data do so from a
single thread context so you can avoid the use of synchronization locks.

The C# implimentation is now based on and offers rough feature parity with the
C++ version (task\_queue) found in ModernCLI. ConcurrentQueue has been replaced
with Deque, and other behaviors are more consistent with ModernCLI.

## semaphores.cs

This provides a counting semaphore that controls execution flow much like the
latest version of semaphore\_t does in Moderncli. This is used to "gate" the
number of concurrent threads that can execute between a Semaphore wait and
post, as well as allows the semaphore to be released and issue exceptions to
any remaining pending tasks or new requests after being released.

## templates.cs

Just some simple templates for C# features that seem missing or incomplete.

## timers.cs

This is a "cron-like" lambda dispatch service for calling timed or periodic
functions in an application.

## waitgroup.cs

This provides rough emulation of the Go waitgroup sync object.


#!/bin/sh
verify=`git remote -v | head -1 | grep github.com:tychosoft`
if test -z "$verify" ; then
	echo "*** not in release repo" ; exit 1 ; fi

status=`git status | grep "^Your branch is up to date"`
if test -z "$status" ; then
    echo "*** not in release state"
    exit 2
fi

# Copyright (C) 2024 Tycho Softworks.
#
# This file is free software; as a special exception the author gives
# unlimited permission to copy and/or distribute it, with or without
# modifications, as long as this notice is preserved.
#
# This program is distributed in the hope that it will be useful, but
# WITHOUT ANY WARRANTY, to the extent permitted by law; without even the
# implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

# Project constants
PROJECT := Tychosoft.Extensions
ARCHIVE := tychoext
VERSION := 0.1.1
PATH := ./bin/Debug/$(DOTNET):${PATH}

.PHONY: debug release publish clean restore version list tag licenses deps

all:            debug           # default target debug

debug:
	@dotnet build -c Debug

release:
	@dotnet pack -c Release

publish:	release
	@.make/publish.sh github.com:tychosoft
	@git tag v$(VERSION)
	@dotnet nuget push bin/Release/$(PROJECT).$(VERSION).nupkg -k $(APIKEY) -s https://api.nuget.org/v3/index.json

clean:
	@dotnet clean

restore:
	@dotnet restore

list:	release
	@unzip -l bin/Release/$(PROJECT).$(VERSION).nupkg
	@unzip -l bin/Release/$(PROJECT).$(VERSION).snupkg

tag:	publish
	@echo "release $(VERSION) published and tagged"

deps:
	@dotnet list package

licenses:
	@dotnet-project-licenses --input $(ARCHIVE).csproj --include-transitive

# Optional make components we may add
sinclude .make/*.mk


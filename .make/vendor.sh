#!/bin/sh
# Copyright (C) 2022 Tycho Softworks.
#
# This file is free software; as a special exception the author gives
# unlimited permission to copy and/or distribute it, with or without
# modifications, as long as this notice is preserved.
#
# This program is distributed in the hope that it will be useful, but
# WITHOUT ANY WARRANTY, to the extent permitted by law; without even the
# implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

TARGET_FRAMEWORK="$1"
PROJECT_DIR=$(pwd)
VENDOR_DIR=$PROJECT_DIR/vendor
NUGET_DIR=${HOME}/.nuget/packages
PACKAGE_IDS=$(jq -r '.libraries | to_entries[] | .key' obj/project.assets.json)

rm -rf $VENDOR_DIR
mkdir -p $VENDOR_DIR
jq -r '.libraries | to_entries[] | .key' obj/project.assets.json | while read PACKAGE_ID; do
    PACKAGE_NAME=$(echo $PACKAGE_ID | cut -d"/" -f1 | tr '[:upper:]' '[:lower:]')
    PACKAGE_VERSION=$(echo $PACKAGE_ID | cut -d"/" -f2)
    PACKAGE_PATH="$NUGET_DIR/$PACKAGE_NAME/$PACKAGE_VERSION"
    if [ -d "$PACKAGE_PATH" ]; then
        echo "Vendoring $PACKAGE_NAME $PACKAGE_VERSION..."
        DEST_DIR="$VENDOR_DIR/$PACKAGE_NAME/$PACKAGE_VERSION"
        mkdir -p "$DEST_DIR"
        cp -r "$PACKAGE_PATH/"* "$DEST_DIR"
    fi
done



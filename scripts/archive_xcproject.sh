#!/bin/sh

cd `dirname $0`
cd ..

PROJECT_NAME="Unity-iPhone"
SCHEME="Unity-iPhone"
CONFIGURATION="Release"
ENV=$1
ARCHIVE_PATH="./build/${ENV}/${PROJECT_NAME}.xcarchive"

echo "**********Archive start ${ENV}**********"

xcodebuild archive -workspace "./build/${ENV}/iOS/${PROJECT_NAME}.xcworkspace" -scheme "${SCHEME}" -configuration "${CONFIGURATION}" -archivePath "${ARCHIVE_PATH}" -destination 'generic/platform=iOS'

echo "Archiving completed. Archive is located at ${ARCHIVE_PATH}"

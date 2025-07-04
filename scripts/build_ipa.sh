#!/bin/sh
cd `dirname $0`
cd ..

ENV=$1
PROJECT_NAME="Unity-iPhone"
ARCHIVE_PATH="./build/${ENV}/${PROJECT_NAME}.xcarchive"
EXPORT_PATH="./build/${ENV}/export"
EXPORT_OPTIONS_PLIST="./ExportOptions.plist"
IPA_PATH="${EXPORT_PATH}/ProductName.ipa"

echo "Build Ipa for ${ENV}"

xcodebuild -exportArchive -archivePath "${ARCHIVE_PATH}" -exportPath "${EXPORT_PATH}" -exportOptionsPlist "${EXPORT_OPTIONS_PLIST}" -allowProvisioningUpdates

if [ ! -f "${IPA_PATH}" ]; then
    echo "Failed to create .ipa file"
    exit 1
fi

echo ".ipa file created at ${IPA_PATH}"
#!/bin/sh
cd `dirname $0`
cd ..

echo "Make Distribution Manifest"
# アプリバージョンを取得
UNITY_PATH="/Applications/Unity/Hub/Editor/2021.3.31f1/Unity.app/Contents/MacOS/Unity"
PROJECT_PATH="./"
VERSION=$($UNITY_PATH -batchmode -quit -projectPath $PROJECT_PATH -executeMethod Builder.GetAppVersion) 
APPID=$1

if [ -z "$VERSION" ]; then
    VERSION=0
fi

echo "***************APPID: ${APPID}***************"
echo "***************VERSION: ${VERSION}***************"

cp ./scripts/distribution/manifest.plist ./scripts/distribution/${APPID}/manifest.plist

sed -i '' s/"{VERSION}"/"$VERSION"/g ./scripts/distribution/${APPID}/manifest.plist
sed -i '' s/"{APPID}"/"$APPID"/g ./scripts/distribution/${APPID}/manifest.plist
sed -i '' s/"{APPNAME}"/"$APPNAME"/g ./scripts/distribution/${APPID}/manifest.plist

echo "Make Distribution Manifest finished"
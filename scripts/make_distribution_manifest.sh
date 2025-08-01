#!/bin/sh
cd `dirname $0`
cd ..

echo "Make Distribution Manifest"
# アプリバージョンを取得
PROJECT_PATH="./"
APPID=$1


echo "***************APPID: ${APPID}***************"

mkdir -p ./scripts/distribution/${APPID}

cp ./scripts/distribution/manifest.plist ./scripts/distribution/${APPID}/manifest.plist

sed -i '' s/"{VERSION}"/"$VERSION"/g ./scripts/distribution/${APPID}/manifest.plist
sed -i '' s/"{APPID}"/"$APPID"/g ./scripts/distribution/${APPID}/manifest.plist
sed -i '' s/"{APPNAME}"/"$APPNAME"/g ./scripts/distribution/${APPID}/manifest.plist

echo "Make Distribution Manifest finished"
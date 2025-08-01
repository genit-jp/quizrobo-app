#!/bin/bash
set -e

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_VERSION=$(grep "m_EditorVersion:" ${PROJECT_PATH}/ProjectSettings/ProjectVersion.txt | awk '{print $2}')
UNITY_PATH="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
BUILD_PATH_DEV="$PROJECT_PATH/Build/Android/dev"
BUILD_PATH_PROD="$PROJECT_PATH/Build/Android/prod"
LOG_PATH_DEV="$PROJECT_PATH/Build/Logs/build_apk_dev.log"
LOG_PATH_PROD="$PROJECT_PATH/Build/Logs/build_apk_prod.log"

mkdir -p "$BUILD_PATH_DEV"
mkdir -p "$BUILD_PATH_PROD"
mkdir -p "$(dirname $LOG_PATH_DEV)"
mkdir -p "$(dirname $LOG_PATH_PROD)"

$UNITY_PATH \
  -quit \
  -batchmode \
  -projectPath "$PROJECT_PATH" \
  -executeMethod Builder.BuildAndroidApkProd \
  -buildTarget Android \
  -output-path "$BUILD_PATH_PROD/app.apk" \
  -logFile "$LOG_PATH_PROD"


BUNDLE_ID_PROD="jp.genit.quizrobo"
mkdir -p "$PROJECT_PATH/scripts/distribution/$BUNDLE_ID_PROD"
cp "$BUILD_PATH_PROD/app.apk" "$PROJECT_PATH/scripts/distribution/$BUNDLE_ID_PROD/app.apk"

aws s3 sync "$PROJECT_PATH/scripts/distribution/" s3://genit.dev/apps/

# APKはBuild/Android/dev, Build/Android/prodに出力されます。
# ここからfastlaneやGoogle Play Developer APIでアップロード可能です。 
#!/bin/bash
set -e

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_VERSION=$(grep "m_EditorVersion:" ${PROJECT_PATH}/ProjectSettings/ProjectVersion.txt | awk '{print $2}')
UNITY_PATH="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
BUILD_PATH_PROD="$PROJECT_PATH/Build/Android/prod"
LOG_PATH_PROD="$PROJECT_PATH/Build/Logs/build_aab_prod.log"

mkdir -p "$BUILD_PATH_PROD"
mkdir -p "$(dirname $LOG_PATH_PROD)"

$UNITY_PATH \
  -quit \
  -batchmode \
  -projectPath "$PROJECT_PATH" \
  -executeMethod Builder.BuildAndroidAabProd \
  -buildTarget Android \
  -output-path "$BUILD_PATH_PROD/app.aab" \
  -logFile "$LOG_PATH_PROD"

mkdir -p "$PROJECT_PATH/outputs"
cp "$BUILD_PATH_PROD/app.aab" "$PROJECT_PATH/outputs/release.aab"
cd "$PROJECT_PATH/scripts/fastlane"
fastlane android upload_binary

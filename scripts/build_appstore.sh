#!/bin/bash
set -e

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_VERSION=$(grep "m_EditorVersion:" ${PROJECT_PATH}/ProjectSettings/ProjectVersion.txt | awk '{print $2}')
UNITY_PATH="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
BUILD_PATH_PROD="$PROJECT_PATH/Build/iOS/prod"
LOG_PATH_PROD="$PROJECT_PATH/Build/Logs/build_ios_prod.log"
EXPORT_OPTIONS_PLIST="$PROJECT_PATH/scripts/ExportOptions-appstore.plist"

mkdir -p "$BUILD_PATH_PROD"
mkdir -p "$(dirname $LOG_PATH_PROD)"

$UNITY_PATH \
  -quit \
  -batchmode \
  -projectPath "$PROJECT_PATH" \
  -executeMethod Builder.BuildIOSProd \
  -buildTarget iOS \
  -output-dir "$BUILD_PATH_PROD" \
  -logFile "$LOG_PATH_PROD"

# XcodeプロジェクトはBuild/iOS/prodに出力されます。

# --- Xcodeビルド（ipa生成） ---
XCODE_WORKSPACE_PATH="$BUILD_PATH_PROD/Unity-iPhone.xcworkspace"
SCHEME="Unity-iPhone"
ARCHIVE_PATH="$BUILD_PATH_PROD/Unity-iPhone.xcarchive"
EXPORT_PATH="$BUILD_PATH_PROD/ipa"

xcodebuild -workspace "$XCODE_WORKSPACE_PATH" -scheme "$SCHEME" -configuration Release -archivePath "$ARCHIVE_PATH" -destination 'generic/platform=iOS' archive

xcodebuild -exportArchive -archivePath "$ARCHIVE_PATH" -exportPath "$EXPORT_PATH" -exportOptionsPlist "$EXPORT_OPTIONS_PLIST"

# ipaは$EXPORT_PATHに出力されます。ここからApp Store Connectへアップロード可能です。

mkdir -p "$PROJECT_PATH/outputs"
cp "$EXPORT_PATH"/*.ipa "$PROJECT_PATH/outputs/release.ipa"

cd "$PROJECT_PATH/scripts/fastlane"
fastlane ios upload_binary 
fastlane ios upload_metadata

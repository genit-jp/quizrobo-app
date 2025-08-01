#!/bin/bash
set -e

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
UNITY_VERSION=$(grep "m_EditorVersion:" ${PROJECT_PATH}/ProjectSettings/ProjectVersion.txt | awk '{print $2}')
UNITY_PATH="/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
BUILD_PATH_DEV="$PROJECT_PATH/Build/iOS/dev"
BUILD_PATH_PROD="$PROJECT_PATH/Build/iOS/prod"
LOG_PATH_DEV="$PROJECT_PATH/Build/Logs/build_ios_dev.log"
LOG_PATH_PROD="$PROJECT_PATH/Build/Logs/build_ios_prod.log"
EXPORT_OPTIONS_PLIST="$PROJECT_PATH/scripts/ExportOptions-adhoc.plist"
SCHEME="Unity-iPhone"

mkdir -p "$BUILD_PATH_DEV"
mkdir -p "$BUILD_PATH_PROD"
mkdir -p "$(dirname $LOG_PATH_DEV)"
mkdir -p "$(dirname $LOG_PATH_PROD)"

# dev
$UNITY_PATH \
  -quit \
  -batchmode \
  -projectPath "$PROJECT_PATH" \
  -executeMethod Builder.BuildIOSDev \
  -buildTarget iOS \
  -output-dir "$BUILD_PATH_DEV" \
  -logFile "$LOG_PATH_DEV"

# --- Xcodeビルド（ipa生成） ---
# dev
XCODE_WORKSPACE_PATH_DEV="$BUILD_PATH_DEV/Unity-iPhone.xcworkspace"
ARCHIVE_PATH_DEV="$BUILD_PATH_DEV/Unity-iPhone.dev.xcarchive"
EXPORT_PATH_DEV="$BUILD_PATH_DEV/ipa"

xcodebuild -workspace "$XCODE_WORKSPACE_PATH_DEV" -scheme "$SCHEME" -configuration Release -archivePath "$ARCHIVE_PATH_DEV" -destination 'generic/platform=iOS' archive
xcodebuild -exportArchive -archivePath "$ARCHIVE_PATH_DEV" -exportPath "$EXPORT_PATH_DEV" -exportOptionsPlist "$EXPORT_OPTIONS_PLIST"

# # ipaをoutputs/{bundleId}.ipaにコピー
BUNDLE_ID_DEV="jp.genit.quizrobo.dev"
$PROJECT_PATH/scripts/make_distribution_manifest.sh $BUNDLE_ID_DEV

cp "$EXPORT_PATH_DEV"/*.ipa "$PROJECT_PATH/scripts/distribution/$BUNDLE_ID_DEV/app.ipa"


# # devビルド後にmanifest生成

# # prod
# XCODE_WORKSPACE_PATH_PROD="$BUILD_PATH_PROD/Unity-iPhone.xcworkspace"
# ARCHIVE_PATH_PROD="$BUILD_PATH_PROD/Unity-iPhone.xcarchive"
# EXPORT_PATH_PROD="$BUILD_PATH_PROD/ipa"

# $UNITY_PATH \
#   -quit \
#   -batchmode \
#   -projectPath "$PROJECT_PATH" \
#   -executeMethod Builder.BuildIOSProd \
#   -buildTarget iOS \
#   -output-dir "$BUILD_PATH_PROD" \
#   -logFile "$LOG_PATH_PROD"

# xcodebuild -workspace "$XCODE_WORKSPACE_PATH_PROD" -scheme "$SCHEME" -configuration Release -archivePath "$ARCHIVE_PATH_PROD" archive
# xcodebuild -exportArchive -archivePath "$ARCHIVE_PATH_PROD" -exportPath "$EXPORT_PATH_PROD" -exportOptionsPlist "$EXPORT_OPTIONS_PLIST"

# # ipaをoutputs/{prod_bundleId}.ipaにコピー
# BUNDLE_ID_PROD="jp.genit.quizrobo"
# $PROJECT_PATH/scripts/make_distribution_manifest.sh $BUNDLE_ID_PROD

# cp "$EXPORT_PATH_PROD"/*.ipa "$PROJECT_PATH/scripts/distribution/$BUNDLE_ID_PROD/app.ipa"

aws s3 sync "$PROJECT_PATH/scripts/distribution/" s3://genit.dev/apps/


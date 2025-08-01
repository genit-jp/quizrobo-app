#!/bin/bash
set -e

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
cd "$PROJECT_PATH/scripts/fastlane"


# 取得したversionCodeでメタデータをアップロード
VERSION_CODE=$LATEST_VERSION_CODE fastlane android upload_metadata
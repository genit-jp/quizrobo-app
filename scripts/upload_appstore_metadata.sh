#!/bin/bash
set -e

PROJECT_PATH="$(cd "$(dirname "$0")/.." && pwd)"
cd "$PROJECT_PATH/scripts/fastlane"

fastlane ios upload_metadata
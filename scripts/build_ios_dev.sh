#!/bin/sh
cd `dirname $0`
cd ..
echo "Build Unity Project for dev"
UNITY_APP_PATH="/Applications/Unity/Hub/Editor/2021.3.31f1/Unity.app/Contents/MacOS/Unity"
UNITY_PROJECT_PATH="./"
UNITY_BUILD_NAME="Builder.BuildDev"
UNITY_LOG_PATH="./build/dev/iOS/build.log"
PROJEKT_DIR="./build/dev/iOS"

$UNITY_APP_PATH -batchmode -quit -projectPath $UNITY_PROJECT_PATH -executeMethod $UNITY_BUILD_NAME -logFile $UNITY_LOG_PATH -output-dir $PROJEKT_DIR

if [ $? -eq 1 ] ; then
    echo "Build Unity Project failed"
    exit $?
fi

echo "Build Unity Project for dev finished"

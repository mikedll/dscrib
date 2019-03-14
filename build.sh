#!/bin/bash

TARGET_RUNTIME="ubuntu.16.04-x64"
dotnet restore DScrib2/DScrib2.csproj --runtime ${TARGET_RUNTIME} || { echo 'Clean failed' ; exit 1; }

echo "Cleaning pubroot."
rm -rf ./DScrib2/pubroot

dotnet publish DScrib2/DScrib2.csproj --output pubroot --runtime ${TARGET_RUNTIME} --configuration Release || { echo 'Building release' ; exit 1; }

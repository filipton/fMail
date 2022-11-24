#!/bin/bash

dotnet publish -c Release
docker buildx build --platform linux/arm64,linux/amd64 --tag filipton/fmail:latest --push .
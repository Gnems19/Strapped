#!/bin/bash
# Deploy Unity WebGL build to GitHub Pages (gh-pages branch)
# Usage: bash deploy.sh [build-folder]
#   build-folder: path to your WebGL build output (default: build/WebGL/Strapped)

set -e

BUILD_DIR="${1:-build/WebGL/Strapped}"

if [ ! -d "$BUILD_DIR" ]; then
  echo "Error: Build folder '$BUILD_DIR' not found."
  echo ""
  echo "Build your game first in Unity:"
  echo "  File -> Build Settings -> WebGL -> Build"
  echo "  Save to: build/WebGL/Strapped"
  echo ""
  echo "Then run: bash deploy.sh"
  exit 1
fi

# Check that it looks like a WebGL build
if [ ! -f "$BUILD_DIR/index.html" ]; then
  echo "Error: No index.html found in '$BUILD_DIR'."
  echo "Make sure this is a Unity WebGL build output folder."
  exit 1
fi

REMOTE=$(git remote get-url origin)
TEMP_DIR=$(mktemp -d)

echo "Deploying '$BUILD_DIR' to gh-pages branch..."

# Copy build to temp directory
cp -r "$BUILD_DIR"/* "$TEMP_DIR"/

# Set up a fresh git repo in the temp dir and push to gh-pages
cd "$TEMP_DIR"
git init
git checkout -b gh-pages
git add -A
git commit -m "Deploy WebGL build to GitHub Pages"
git remote add origin "$REMOTE"
git push origin gh-pages --force

echo ""
echo "Deployed! Enable GitHub Pages if you haven't:"
echo "  Repo -> Settings -> Pages -> Source: 'Deploy from a branch' -> Branch: 'gh-pages' / '/ (root)'"
echo ""

# Cleanup
rm -rf "$TEMP_DIR"

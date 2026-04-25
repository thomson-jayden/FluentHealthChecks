#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SOLUTION="$SOLUTION_DIR/FluentHealthChecks.slnx"

FIX=false

for arg in "$@"; do
  case $arg in
    --fix) FIX=true ;;
    *) echo "Unknown argument: $arg"; exit 1 ;;
  esac
done

echo "Running dotnet format on $SOLUTION"

if [ "$FIX" = true ]; then
  echo "  Mode: fix (applying changes)"
  dotnet format "$SOLUTION"
else
  echo "  Mode: verify (no changes will be written)"
  dotnet format "$SOLUTION" --verify-no-changes
fi

echo "Lint passed"


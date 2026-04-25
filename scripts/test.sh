#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SOLUTION="$SOLUTION_DIR/FluentHealthChecks.slnx"

COVERAGE=false

for arg in "$@"; do
  case $arg in
    --coverage) COVERAGE=true ;;
    *) echo "Unknown argument: $arg"; exit 1 ;;
  esac
done

echo "Running tests for: $SOLUTION"

if [ "$COVERAGE" = true ]; then
  echo "  Mode: with code coverage"
  dotnet test "$SOLUTION" --no-build --configuration Release \
    --collect:"XPlat Code Coverage" \
    --results-directory "$SOLUTION_DIR/TestResults"
else
  dotnet test "$SOLUTION" --no-build --configuration Release
fi

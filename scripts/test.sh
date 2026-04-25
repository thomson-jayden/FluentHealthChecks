#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SOLUTION="$SOLUTION_DIR/FluentHealthChecks.slnx"

echo "Running tests for: $SOLUTION"

dotnet test "$SOLUTION" --no-build --configuration Release

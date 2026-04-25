#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SOLUTION="$SOLUTION_DIR/FluentHealthChecks.slnx"

echo "Running security scan on $SOLUTION"

ARGS=(list "$SOLUTION" package --vulnerable --include-transitive)

OUTPUT=$(dotnet "${ARGS[@]}" 2>&1)
echo "$OUTPUT"

if echo "$OUTPUT" | grep -q "has the following vulnerable packages"; then
  echo ""
  echo "Vulnerable packages detected — see above for details"
  exit 1
fi

echo "No known vulnerabilities found"


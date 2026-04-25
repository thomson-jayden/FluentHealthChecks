#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
SOLUTION_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
SOLUTION="$SOLUTION_DIR/FluentHealthChecks.slnx"

INCLUDE_TRANSITIVE=false

for arg in "$@"; do
  case $arg in
    --transitive) INCLUDE_TRANSITIVE=true ;;
    *) echo "Unknown argument: $arg"; exit 1 ;;
  esac
done

echo "🔒 Running security scan on $SOLUTION"

ARGS=(list "$SOLUTION" package --vulnerable)

if [ "$INCLUDE_TRANSITIVE" = true ]; then
  echo "  Scope: top-level + transitive dependencies"
  ARGS+=(--include-transitive)
else
  echo "  Scope: top-level dependencies (pass --transitive to include all)"
fi

OUTPUT=$(dotnet "${ARGS[@]}" 2>&1)
echo "$OUTPUT"

if echo "$OUTPUT" | grep -q "has the following vulnerable packages"; then
  echo ""
  echo "❌ Vulnerable packages detected — see above for details"
  exit 1
fi

echo "✅ No known vulnerabilities found"


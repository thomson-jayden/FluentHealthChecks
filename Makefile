.PHONY: all checks lint lint-fix restore security-scan build test

all: checks

checks: lint test security-scan

lint:
	bash scripts/lint.sh

lint-fix:
	bash scripts/lint.sh --fix

security-scan:
	bash scripts/security-scan.sh

restore:
	bash scripts/restore.sh

build: restore
	bash scripts/build.sh

test: build
	bash scripts/test.sh
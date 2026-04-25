.PHONY: all checks lint lint-fix security-scan build test

all: checks

checks: lint build test security-scan

lint:
	bash scripts/lint.sh

lint-fix:
	bash scripts/lint.sh --fix

security-scan:
	bash scripts/security-scan.sh

build:
	bash scripts/build.sh

test:
	bash scripts/test.sh
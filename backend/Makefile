.PHONY: help run test

help:
	@echo "Available targets:"
	@echo "  make run   - Run the backend API"
	@echo "  make test  - Run backend tests"

run:
	dotnet run --project src/EventBoard.Api

test:
	dotnet test EventBoard.sln

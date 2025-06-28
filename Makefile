# Variables
DOCKER_COMPOSE = docker-compose
REDIS_SERVICE = redis
FINANCE_SERVICE = finance-service
REDIS_CONTAINER = redis_holidays
REDIS_DATA_FILE = redis_data.txt

# Default target
.PHONY: help
help:
	@echo "Usage:"
	@echo "  make build       - Build the Redis and Finance Docker containers"
	@echo "  make up          - Start the Redis and Finance containers"
	@echo "  make down        - Stop the Redis and Finance containers"
	@echo "  make restart     - Restart the Redis and Finance containers"
	@echo "  make logs        - Show logs from all containers"
	@echo "  make redis-logs  - Show logs from the Redis container"
	@echo "  make finance-logs - Show logs from the Finance container"
	@echo "  make load-data   - Load data from redis_data.txt into Redis"
	@echo "  make clean       - Remove all containers and volumes"

# Build the Docker containers
.PHONY: build
build:
	@echo "Building Docker containers..."
	$(DOCKER_COMPOSE) build

# Start the containers
.PHONY: up
up:
	@echo "Starting the containers..."
	$(DOCKER_COMPOSE) up -d

# Stop the containers
.PHONY: down
down:
	@echo "Stopping the containers..."
	$(DOCKER_COMPOSE) down

# Restart the containers
.PHONY: restart
restart: down up
	@echo "Restarted the containers."

# Show logs from all containers
.PHONY: logs
logs:
	@echo "Showing logs from all containers..."
	$(DOCKER_COMPOSE) logs -f

# Show logs from the Redis container
.PHONY: redis-logs
redis-logs:
	@echo "Showing logs from the Redis container..."
	$(DOCKER_COMPOSE) logs -f $(REDIS_SERVICE)

# Show logs from the Finance container
.PHONY: finance-logs
finance-logs:
	@echo "Showing logs from the Finance container..."
	$(DOCKER_COMPOSE) logs -f $(FINANCE_SERVICE)

# Load data into Redis from redis_data.txt
.PHONY: load-data
load-data:
	@echo "Loading data from $(REDIS_DATA_FILE) into Redis..."
	@if [ -f $(REDIS_DATA_FILE) ]; then \
		docker exec -i $(REDIS_CONTAINER) redis-cli < $(REDIS_DATA_FILE); \
		echo "Data successfully loaded into Redis."; \
	else \
		echo "Error: $(REDIS_DATA_FILE) not found. Make sure the file exists."; \
	fi

# Clean up containers and volumes
.PHONY: clean
clean:
	@echo "Removing all containers and volumes..."
	$(DOCKER_COMPOSE) down -v
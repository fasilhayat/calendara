#!/bin/bash

echo "Starting Redis and RedisInsight initialization..."

# Wait for Redis to start
REDIS_READY=0
echo "Checking if Redis is ready..."
while [ $REDIS_READY -ne 1 ]; do
    if redis-cli ping 2>/dev/null | grep -q PONG; then
        REDIS_READY=1
    else
        echo "Waiting for Redis to be ready..."
        sleep 2
    fi
done

echo "Redis is ready!"

# Flush existing data in Redis
echo "Flushing existing Redis data..."
redis-cli FLUSHALL
echo "Existing data cleared."

# Load fresh data from redis_data.txt
if [ -f /docker-entrypoint-initdb.d/redis_data.txt ]; then
    echo "Loading data from redis_data.txt into Redis..."
    redis-cli < /docker-entrypoint-initdb.d/redis_data.txt
    echo "Data loaded successfully."
else
    echo "redis_data.txt not found. Skipping data loading." 
fi
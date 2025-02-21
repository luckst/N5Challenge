#!/bin/bash

HOST=kafka
PORT=29092

echo "Waiting for Kafka to be ready at $HOST:$PORT..."

# We'll keep trying to open a TCP connection via /dev/tcp
while ! (echo > "/dev/tcp/$HOST/$PORT") 2>/dev/null; do
  echo "Kafka is not up yet..."
  sleep 2
done

echo "Kafka is up! Proceeding..."

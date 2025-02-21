#!/bin/bash

# Adjust service name and port if needed
KAFKA_SERVICE="kafka"
KAFKA_BOOTSTRAP="kafka:29092"

echo "Listing all Kafka topics..."
docker compose exec $KAFKA_SERVICE kafka-topics.sh --list --bootstrap-server $KAFKA_BOOTSTRAP

echo ""
echo "---------------------------------------------------------"
echo "Consuming from a specific topic (permissions-topic) from beginning"
echo "---------------------------------------------------------"
docker compose exec $KAFKA_SERVICE kafka-console-consumer.sh \
    --bootstrap-server $KAFKA_BOOTSTRAP \
    --topic permissions-topic \
    --from-beginning

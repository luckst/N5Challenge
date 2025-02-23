<h3 align="center">N5 Challenge</h3>

---

<p align="center"> This is a project created for N5
    <br> 
</p>

## 📝 Table of Contents

- [About](#about)
- [Getting Started](#getting_started)
- [Running Tests](#running_tests)

## 🧐 About <a name = "about"></a>

This project was created for N5 and is built using .NET 7. It includes a web API used to request, modify and get permissions of the employees, it uses elastic search, docker, and it also contains unit tests and integration tests.

## 🏁 Getting Started <a name = "getting_started"></a>

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

To run this project, you'll need the following software:

- .NET SDK 7
- Docker
- Make sure you have installed and ran Elasticsearch
- Make sure you have installed and ran Apache Kafka and Zookeeper

### Running the Project

1. Clone the repository.
2. Navigate to the project directory.
3. Run the following command to start the services using Docker Compose:

    ```sh
    docker-compose up --build
    ```

4. The API will be available at `http://localhost:8080/swagger`.
5. The SQL Server will be available at `localhost:1433`.
6. Elasticsearch will be available at `http://localhost:9200`.
7. Kafka will be available at `localhost:9092`.

### Environment Variables

Make sure to set the following environment variables in the [docker-compose.yml](http://_vscodecontentref_/0) file:

- `ASPNETCORE_ENVIRONMENT=Development`
- `DB_Server=sql`
- `ElasticSearch__Url=http://elasticsearch:9200`
- `ElasticSearch__UserName=`
- `ElasticSearch__Password=MyElasticPass123`
- `ElasticSearch__DisableCertificateValidation=true`
- `SA_PASSWORD=Test123!`
- `ELASTIC_PASSWORD=MyElasticPass123`
- `Kafka__ProducerSettings__BootstrapServers=kafka:29092`
- `Kafka__ConsumerSettings__BootstrapServers=kafka:29092`

## 🧪 Running Tests <a name = "running_tests"></a>

### Integration Tests

The project includes integration tests that use the real database. To run the integration tests:

1. Make sure the Docker services are running:
    ```sh
    docker-compose up -d
    ```

2. Run the tests using the .NET CLI:
    ```sh
    dotnet test
    ```

   Or run them from your IDE of choice.

Important Notes for Integration Tests:
- The tests require a running SQL Server instance (provided by docker-compose)
- Tests will automatically clean up any data they create
- Tests are executed in a specific order to maintain data consistency
- Each test suite uses unique identifiers to avoid conflicts


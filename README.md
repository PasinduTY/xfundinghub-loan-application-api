# XFundingHub – Loan Application API

A RESTful ASP.NET Core Web API for managing loan applications, including application creation, retrieval, and controlled status transitions.

## Overview

XFundingHub provides APIs for:

* Creating loan applications
* Retrieving an existing loan application
* Changing the status of a loan application
* Validating loan application business rules
* Persisting applications using Entity Framework Core and SQL Server

The application follows a layered architecture with separate Domain, Application, Infrastructure, API, Unit Test, and Integration Test projects.

## Architecture

```text
XFundingHub.API
        |
        v
XFundingHub.Application
        |
        v
XFundingHub.Domain

XFundingHub.Application
        |
        v
XFundingHub.Infrastructure
        |
        v
SQL Server
```

### Projects

```text
XFundingHub/
├── XFundingHub.API/
├── XFundingHub.Application/
├── XFundingHub.Domain/
├── XFundingHub.Infrastructure/
├── XFundingHub.UnitTests/
└── XFundingHub.IntegrationTests/
```

### Responsibilities

**Domain**

Contains the core business rules and entities, including:

* `LoanApplication`
* `ApplicationStatus`
* Status transition validation
* Loan application validation rules

**Application**

Contains application-level services, DTOs, and repository abstractions.

**Infrastructure**

Contains persistence-related implementations such as:

* Entity Framework Core `DbContext`
* SQL Server configuration
* Repository implementations

**API**

Contains HTTP controllers and API request/response handling.

**UnitTests**

Tests domain and application-level behavior using test doubles.

**IntegrationTests**

Tests the API and persistence behavior using the actual application infrastructure.

## Technologies

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server / LocalDB
* xUnit
* Microsoft.NET.Test.Sdk
* Coverlet
* Swagger/OpenAPI

## Prerequisites

Install:

* .NET 10 SDK
* SQL Server LocalDB or a compatible SQL Server instance
* Visual Studio 2022 or another compatible .NET IDE

Verify the .NET installation:

```bash
dotnet --version
```

## Database Configuration

The API uses the `DefaultConnection` connection string.

The default development configuration uses SQL Server LocalDB:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=XFundingHubDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Update the connection string if a different SQL Server instance is being used.

## Running the API

Navigate to the API project:

```bash
cd XFundingHub.API
```

Run the application:

```bash
dotnet run
```

The API will start using the configured ASP.NET Core launch settings.

OpenAPI is enabled in the Development environment.

## Running the Tests

From the solution root:

```bash
dotnet test
```

This runs both unit and integration tests.

The current test suite contains:

* Unit tests for domain/application behavior
* Integration tests for repository behavior
* Integration tests for API endpoints

## API Endpoints

### Create Loan Application

```http
POST /api/v1/loan-applications
```

Example request:

```json
{
  "customerId": "C1001",
  "amount": 10000,
  "currency": "GBP",
  "termMonths": 12
}
```

Successful response:

```http
201 Created
```

Example response:

```json
{
  "applicationId": "LA1001",
  "customerId": "C1001",
  "amount": 10000,
  "currency": "GBP",
  "termMonths": 12,
  "status": "SUBMITTED"
}
```

### Get Loan Application

```http
GET /api/v1/loan-applications/{applicationId}
```

Example:

```http
GET /api/v1/loan-applications/LA1001
```

Successful response:

```http
200 OK
```

If the application does not exist:

```http
404 Not Found
```

### Change Application Status

```http
PATCH /api/v1/loan-applications/{applicationId}/status
```

Example:

```json
{
  "status": "UNDER_REVIEW"
}
```

Successful response:

```http
200 OK
```

## Loan Application Validation

The domain validates the following rules:

* Customer ID must not be null, empty, or whitespace
* Amount must be at least `10,000`
* Currency must be `GBP`
* Currency comparison is case-insensitive
* Term must be at least `12` months

No maximum amount or maximum term is defined by the current requirements.

## Application Status Workflow

A loan application starts in:

```text
SUBMITTED
```

The permitted transitions are:

```text
SUBMITTED
    |
    v
UNDER_REVIEW
   / \
  v   v
APPROVED  REJECTED
   |
   v
DISBURSED
```

Permitted transitions:

```text
SUBMITTED → UNDER_REVIEW

UNDER_REVIEW → APPROVED

UNDER_REVIEW → REJECTED

APPROVED → DISBURSED
```

Invalid status transitions are rejected by the domain business rules and exposed by the API as:

```http
409 Conflict
```

An unsupported status value is rejected as:

```http
400 Bad Request
```

## HTTP Response Codes

| Scenario                    |       HTTP Status |
| --------------------------- | ----------------: |
| Application created         |     `201 Created` |
| Application retrieved       |          `200 OK` |
| Status changed successfully |          `200 OK` |
| Invalid request/validation  | `400 Bad Request` |
| Application not found       |   `404 Not Found` |
| Invalid status transition   |    `409 Conflict` |

## Testing

The project includes unit and integration tests covering:

* Application ID generation
* Application creation
* Application persistence
* Validation rules
* Application retrieval
* Valid status transitions
* Invalid status transitions
* Unsupported status values
* Missing applications
* API response status codes
* API response status formatting

Run the complete test suite with:

```bash
dotnet test
```

## Current Test Status

The complete test suite currently passes:

```text
Total:   55
Failed:  0
Succeeded: 55
Skipped:  0
```

## Scope

The implementation focuses on the loan application API and its core business rules.

The current scope does not require additional authentication, external FX integration, Kafka/RabbitMQ messaging, cloud deployment, Docker, or a frontend application.

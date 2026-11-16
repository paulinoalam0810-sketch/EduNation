# Student Management API

A RESTful API for student management, built with **C#, ASP.NET Core 10, Entity Framework Core, and SQL Server**, following Clean Architecture principles.

## Overview

Student Management API provides functionality to manage student records through a structured RESTful architecture.

The project focuses on maintainability, separation of concerns, data validation, centralized exception handling, and automated unit testing.

## Technologies

- C#
- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- xUnit
- Moq
- Git and GitHub

## Architecture

The solution follows Clean Architecture principles and separates responsibilities into five projects:

| Project | Responsibility |
|---|---|
| `Students.Domain` | Core entities and business rules |
| `Students.Application` | Use cases, abstractions, and application logic |
| `Students.Infrastructure` | Database access and persistence |
| `Students.API` | HTTP endpoints and API configuration |
| `Students.Application.Tests` | Automated unit tests |

## Features

- Create student records.
- Retrieve a student by ID.
- Retrieve all active students.
- Update student information.
- Deactivate student records.
- Validate student information.
- Handle duplicate email conflicts.
- Centralized exception handling.
- Structured error responses using Problem Details.
- Asynchronous database operations.
- Automated unit testing with xUnit and Moq.

## Testing

The application includes unit tests for application use cases using **xUnit** and **Moq**.

Current test coverage includes:

- `CreateStudentUseCase`
- `GetStudentByIdUseCase`
- `GetAllStudentsUseCase`

Tests cover successful operations, validation failures, missing records, and empty collections.

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server or SQL Server Express
- Visual Studio 2022 or a compatible IDE

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/paulinoalam0810-sketch/Student-Management-API.git
   ```

2. Open the solution in Visual Studio.

3. Configure the SQL Server connection string in the API project's configuration.

4. Apply Entity Framework Core migrations:

   ```bash
   dotnet ef database update --project Students.Infrastructure --startup-project Students.API
   ```

5. Run the API:

   ```bash
   dotnet run --project Students.API
   ```

## API Base Route

```text
/api/students
```

The API supports student creation, retrieval, updating, and deactivation.

## Purpose

This project demonstrates practical knowledge of backend development with C# and .NET, including RESTful API design, relational database integration, Clean Architecture, dependency injection, exception handling, and automated testing.

---

**Author:** Alam Paulino

**GitHub:** [paulinoalam0810-sketch](https://github.com/paulinoalam0810-sketch)

# ReqresUserService

This project demonstrates a clean and testable .NET component that integrates with an external REST API. It uses the [Reqres](https://reqres.in) API as a mock provider to fetch and display user information.


## Project Goals

- Build a reliable class library that interacts with a public API
- Handle user listing and detail retrieval
- Support retry logic, caching, and structured error handling
- Be easily testable and reusable in larger platforms

## Setup Instructions

### 1. Clone the Repository

git clone https://github.com/devjohn12356/ReqrestUserService
cd ReqresUserService

### 2. Build Project

dotnet build

### 3. Run the Console Application

dotnet run --project ExternalUserService.ConsoleApp

### 4. Run Unit Test

dotnet test

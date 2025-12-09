
# Employee Management System – TalentoPlus S.A.S.

## Project Description

The **TalentoPlus S.A.S. Employee Management System** aims to centralize and automate the management of the company's employees. Currently, employee information is handled in a disorganized manner through multiple Excel files. This system solves that problem by providing a web platform for employee management, along with a REST API that allows employees to access their own information.

## Features

### 1. Web Application (HR Administrator)
The web application is exclusively aimed at the **Human Resources Administrator**. The administrator accesses the employee management module through authentication using **ASP.NET Core Identity**. Features include:

- **Employee Management**: Full CRUD operations to create, edit, list, and delete employees.
- **Excel Import**: The application allows importing employee data from an Excel file.
- **Generate Resume in PDF**: Administrators can generate employee resumes in PDF format.

### 2. Dashboard with Artificial Intelligence
The application’s dashboard includes the following information cards:

- Total number of registered employees.
- Number of employees on vacation.
- Active employees.

Additionally, **Artificial Intelligence** is incorporated for natural language queries, such as:

- "How many employees belong to the Technology department?"
- "How many employees are Inactive?"

### 3. REST API
The **REST API** allows employees to query their own information and generate their resume in PDF format. This API includes:

- **Public Endpoints (no JWT required)**:
    - List departments.
    - Self-register an employee.
    - Employee login.

- **Protected Endpoints (require JWT)**:
    - Query employee information.
    - Download Resume in PDF.

## Technologies Used

- **ASP.NET Core**: Backend and web application.
- **PostgreSQL**: Database.
- **Swagger**: API documentation.
- **EF Core**: ORM for database interaction.
- **iTextSharp**: PDF generation (Resume).
- **Docker**: Containerization of the solution.

## Steps to Run the Solution

### Restore NuGet packages:
In the root of the project, run the following command to restore all the necessary NuGet packages:
```bash
dotnet restore
```

### Build the project:
```bash
dotnet build
```

### Run the application:
To run the application locally:
For the API, go to `/talentoplus/src/Api`
For Web.Admin, go to `/talentoplus/src/Web.Admin`
```bash
dotnet run
```

### Open the application:
Once the application is running, open your browser and go to:
```
 http://localhost:5026
```

Or access Swagger at:
```
  http://localhost:5164/sgwagger
```

## Environment Variable Configuration

Make sure to configure the following environment variables before running the application:

### Database Variables
```bash
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=talentoplusdb;User=root;Password=1234;"
  },
  "Jwt": {
    "Key": "F3a9H8kLmN2pQr5tV8xYzB1qW2eR7uT4sP9dM6cV",
    "Issuer": "edutrack-api"
  },
  "OpenAI": {
    "ApiKey": ""
  },
  "Smtp": {
    "Host": "sandbox.smtp.mailtrap.io",
    "Port": 587,
    "Username": "99722b3c760ae7",
    "Password": "8d515107f44bff",
    "From": "noreply@edutrack.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

# Overview

FintcsApi is a financial technology API built with ASP.NET Core that provides user authentication and management services. The system implements JWT-based authentication with user registration and login capabilities, storing user data with flexible JSON-based details storage.

# User Preferences

Preferred communication style: Simple, everyday language.

# System Architecture

## Backend Framework
- **ASP.NET Core Web API** - Modern, cross-platform web framework chosen for its performance, built-in dependency injection, and robust middleware pipeline
- **Entity Framework Core** - ORM for database operations, providing type-safe database queries and automatic migration support
- **Controller-based API** - RESTful API design with attribute routing for clear endpoint organization

## Authentication & Security
- **JWT (JSON Web Tokens)** - Stateless authentication mechanism allowing scalable user sessions without server-side storage
- **BCrypt Password Hashing** - Industry-standard password hashing with built-in salt generation for secure password storage
- **Claims-based Authorization** - Flexible role and permission system using JWT claims

## Data Storage Design
- **SQL Server Database** - Relational database with Entity Framework Code First approach for schema management
- **Hybrid Data Model** - Structured user authentication data with flexible JSON storage for user details, allowing schema evolution without migrations
- **Connection String Configuration** - Database connection managed through appsettings.json with trusted connection for local development

## Configuration Management
- **ASP.NET Core Configuration** - Hierarchical configuration system using appsettings.json for environment-specific settings
- **JWT Configuration** - Centralized JWT settings including secret key, issuer, and audience validation parameters

## API Architecture
- **RESTful Design** - Standard HTTP verbs and status codes for predictable API behavior
- **DTO Pattern** - Data Transfer Objects for clean separation between API contracts and internal models
- **Async/Await Pattern** - Non-blocking I/O operations for better scalability and performance

# External Dependencies

## Core Framework Dependencies
- **Microsoft.AspNetCore** - Web API framework and hosting
- **Microsoft.EntityFrameworkCore** - Object-relational mapping and database operations
- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server database provider

## Authentication Libraries
- **Microsoft.IdentityModel.Tokens** - JWT token validation and security
- **System.IdentityModel.Tokens.Jwt** - JWT token creation and parsing
- **BCrypt.Net** - Password hashing and verification

## Database
- **SQL Server** - Primary database engine (configured for localhost with trusted connection)
- Database name: FintcsApiDB

## Configuration
- **Microsoft.Extensions.Configuration** - Configuration management and binding
- **System.Text.Json** - JSON serialization for flexible user details storage
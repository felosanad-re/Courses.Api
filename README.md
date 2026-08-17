Course Platform — Backend

A RESTful Web API for an e-learning platform built with ASP.NET Core and designed using Clean Architecture principles.

The API provides the core business logic for authentication, users, courses, enrollments, learning progress, ratings, payments, and instructor management.

Features
Authentication and Authorization using ASP.NET Core Identity and JWT
Role-based access control
Course and lecture management
Student enrollment system
Learning progress tracking
Course rating and review system
Stripe payment integration
Stripe webhook handling
Redis caching and temporary data storage
Zoom API integration
Zoom Webhooks
Email notifications
Searching, filtering, sorting, and pagination
Student and instructor statistics
Application logging using Serilog
Architecture

The project follows Clean Architecture with a clear separation of concerns between:

Domain
Application
Infrastructure
API

The backend also implements Generic Repository, Unit of Work, and Specification patterns to provide a maintainable and scalable application structure.

Technologies
ASP.NET Core / .NET
Entity Framework Core
SQL Server
ASP.NET Core Identity
JWT
Redis
Stripe
Zoom API
Serilog
AutoMapper
Swagger / OpenAPI
Frontend

The API is consumed by an Angular 17 frontend application. 
Angular Repo: https://github.com/felosanad-re/Courses

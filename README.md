# Movie Ticket Booking Api
### A gRPC Web Api for managing movies, sessions, tickets and visitors. The data is persisted in Couchbase.

## Functionality
# User authentication and authorization
# Email notification on registration
# Movies, sessions, halls and tickets management
# Automatic data cleanup via recurring jobs

## Dependencies
* `AspNetCore.HealthChecks.UI.Client` - detailed health checks information
* `AutoMapper.Extensions.Microsoft.DependencyInjection` - data mapping
* `Couchbase.Extensions.DependencyInjection` - Couchbase DI integration
* `CouchbaseNetClient` - Couchbase client support
* `FluentEmail.Razor` - Razor email templates support
* `FluentEmail.Smtp` - email sending via SMTP
* `Grpc.AspNetCore` - gRPC support for ASP.NET Core
* `Grpc.AspNetCore.HealthChecks` - gRPC health checks integration
* `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT authentication
* `Microsoft.AspNetCore.Grpc.JsonTranscoding` - JSON transcoding support for gRPC
* `Microsoft.AspNetCore.Grpc.Swagger` - gRPC Swagger integration
* `Microsoft.Extensions.Caching.Abstractions` - caching abstractions
* `Quartz` - jobs scheduling
* `Quartz.AspNetCore` - Quartz integration for ASP.NET Core
* `System.IdentityModel.Tokens.Jwt` - JSON Web Tokens support

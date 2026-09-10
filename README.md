# OmnisNexus

OmnisNexus is a Blazor Server community messaging application built for the WGU D424 Software Engineering Capstone. The app lets authenticated users create and join communities, organize discussions into channels, send and manage messages, search community conversations, and view member activity.

## Features

- User registration, login, account management, and authentication through ASP.NET Core Identity
- Community creation, editing, joining, leaving, and deletion
- Channel creation, editing, navigation, and deletion
- Message sending, editing, deletion, timestamps, and edited indicators
- Community-wide message search grouped by channel
- Role-based permissions for owners, moderators, and members
- Member reports with role, join date, and message count
- Home dashboard statistics for joined communities, owned communities, sent messages, and edited messages
- Automated EF Core migrations at application startup
- Unit tests for core services

## Tech Stack

- .NET 10
- Blazor Server with interactive server components
- ASP.NET Core Identity
- Entity Framework Core
- PostgreSQL through `Npgsql.EntityFrameworkCore.PostgreSQL`
- xUnit, Moq, and EF Core InMemory for tests
- Bootstrap and custom CSS
- Railway deployment support through GitLab CI

## Project Structure

```text
Components/              Blazor pages, layouts, account UI, and reusable UI components
Data/                    Identity user and EF Core application DbContext
Migrations/              EF Core database migrations
Models/                  Community, channel, membership, message, role, and view models
Services/                Business logic and state services
OmnisNexus.Tests/        Unit tests and test database helpers
wwwroot/                 Static assets, CSS, Bootstrap files, and favicon
Program.cs               Application startup, service registration, middleware, and migrations
```

## Prerequisites

- .NET 10 SDK
- PostgreSQL database
- EF Core CLI tools, if you need to create or update migrations manually

Install the EF Core CLI if it is not already available:

```powershell
dotnet tool install --global dotnet-ef
```

## Configuration

The application reads its database connection from `ConnectionStrings:DefaultConnection`. The checked-in `appsettings.json` leaves this value blank so local and production secrets are not committed.

For local development, store the connection string in user secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=omnisnexus;Username=postgres;Password=your_password"
```

For deployment, set the same configuration key through the host environment. On many platforms this can be provided as:

```text
ConnectionStrings__DefaultConnection
```

The app also supports a `PORT` environment variable and binds to `http://0.0.0.0:{PORT}` when it is present.

## Run Locally

Restore dependencies:

```powershell
dotnet restore
```

Build the solution:

```powershell
dotnet build
```

Run the app:

```powershell
dotnet run
```

The default launch profiles use:

- `http://localhost:5120`
- `https://localhost:7004`

EF Core migrations are applied automatically during application startup. Make sure the configured PostgreSQL database exists and the configured user has permission to create and update tables.

## Testing

Run the unit test project:

```powershell
dotnet test
```

The tests use the EF Core InMemory provider and focus on service-layer behavior such as permissions, community management, channel management, messaging, and error handling.

## Database Model

OmnisNexus stores four main domain entities in addition to ASP.NET Core Identity tables:

- `Community`: a user-owned discussion space
- `Channel`: a named conversation area inside a community
- `Membership`: a user's relationship to a community and assigned role
- `Message`: a user-authored message in a channel

Relationships use cascade deletion for memberships, channels, and messages. Community ownership is restricted so owner records are not accidentally removed through community deletes.

## Roles and Permissions

OmnisNexus defines three community roles:

- `Owner`: can manage the community, manage members, manage channels, and delete messages
- `Moderator`: can manage channels and delete messages
- `Member`: can participate in channels and manage their own messages

Message authors can edit their own messages. Owners and moderators can delete messages for moderation.

## Deployment

The repository includes a GitLab CI configuration that deploys the `production` branch to Railway:

```yaml
railway up --ci --service "omnis-nexus"
```

Required deployment configuration:

- `RAILWAY_TOKEN` in GitLab CI variables
- A valid PostgreSQL connection string configured for the application
- A writable `/app/keys` directory for ASP.NET Core Data Protection keys in the deployed container

## Development Notes

- Keep connection strings and credentials out of committed files.
- Add or update EF Core migrations when the domain model changes.
- Run `dotnet test` before merging changes that affect services, permissions, messaging, or database behavior.
- Keep business rules in services where possible so behavior remains testable.

[![.NET](https://github.com/benjaminsampica/DynamoLeagueBlazor/actions/workflows/dotnet.yml/badge.svg)](https://github.com/benjaminsampica/DynamoLeagueBlazor/actions/workflows/dotnet.yml)

# Introduction

Dynamo League is a fantasy football league based off the Yahoo fantasy football league with heavily customized rule sets.
It is comprised of two separate stand-alone applications - a client UI written using Blazor WebAssembly and an API server written using ASP.NET Core.

The League operates around teams and players. Teams, managed by users, are added and removed every season depending on participation. Players are actual NFL players and are imported by site managers.

A new season begins every May and continues until the end of December. At the end, points are tallied and rewards are distributed based on how that team's players performed in the real NFL.

# Getting Started

Click [here](./docs/running-locally.md) to be taken to how to run the app locally.

Running & contributing to Dynamo League Blazor requires the following:
- .NET 7 SDK
- IIS Express
- Docker (Linux)
- WASM tools (can be installed via `dotnet workload install wasm-tools`)

By default, two test accounts are created with administrator permissions with the following login information:

Username: `test@gmail.com`
Password: `hunter2`
Team - Space Force

Username: `test2@gmail.com`
Password: `hunter2`
Team - The Donald

# Contributing

Please make sure all tests pass before submitting a new pull request.

## Adding a Migration

New migrations can be added to the database by:

1. Installing the dotnet ef tools via `dotnet tool install --global dotnet-ef`
2. Running the following command with a command line while inside the `/src/Server` folder

 `dotnet ef migrations add {YourMigrationName} -o ./Infrastructure/Migrations --context ApplicationDbContext --project DynamoLeagueBlazor.Server.csproj`

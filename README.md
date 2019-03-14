
## DScrib2

.NET web application to peruse Amazon.com product reviews. User emails are included in requests to Amazon
(since Amazon doesn't provide an API).

![alt text](https://github.com/mikedll/dscrib2/raw/d58704960f6edcc10e49bac7be892055e9774c92/sample.png)

Tech used:

  - ASP.NET Core 2.1
  - Entity Framework Core 2.1.x
  - Google.Apis.Auth for leveraging Google sign-ins.
  - Newtonsoft.Json
  - AngleSharp

This project started as an ASP.NET MVC 5 project.

## Linux Build Commands

    # Restore dependencies
    > dotnet restore DScrib2/DSCrib2.csproj --runtime ubuntu.16.04-x64
    
    # Clean
    > dotnet clean DScrib2/DSCrib2.csproj
    
    # Build release    
    > dotnet build DScrib2/DSCrib2.csproj --runtime ubuntu.16.04-x64 --configuration Release
    
    # Publish to a folder. Has issues.
    > rm -rf ./DScrib2/pubroot
    > dotnet publish DScrib2/DSCrib2.csproj --output pubroot --runtime ubuntu.16.04-x64 --configuration Release

## Setup

Copy `config.json.example` and fill in reasonable values.

The `Tests` project should run without crashing and create new data in your database.

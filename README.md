
## DScrib2

ASP.NET Core application to peruse Amazon.com product reviews. User emails are included in requests to Amazon
(since Amazon doesn't provide an API).

![alt text](https://github.com/mikedll/dscrib2/raw/d58704960f6edcc10e49bac7be892055e9774c92/sample.png)

## Local Dev Setup

Copy `devsecrets.json.example` to `devsecrets.json` and fill in reasonable values (even in the DScrib2 project).

The `Tests` project is for exercising the code but you have to babysit it right now.

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

## Mac build and run

    > dotnet build DScrib2/DSCrib2.csproj

    # Start the server.
    > dotnet bin/Debug/netcoreapp2.1/DScrib2.dll dotnet

You don't have to publish it.

## Tech Used

  - ASP.NET Core 2.1
  - Entity Framework Core 2.1.x
  - Google.Apis.Auth for leveraging Google sign-ins.
  - Newtonsoft.Json
  - AngleSharp

This project started as an ASP.NET MVC 5 project.

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution files
COPY ECommerce_WebApi.slnx ./
COPY src/Application/Application.csproj Application/
COPY src/Domain/Domain.csproj Domain/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/
COPY src/WebApi/WebApi.csproj WebApi/

# Restore dependencies
RUN dotnet restore WebApi/WebApi.csproj --configfile /root/.nuget/NuGet/NuGet.Config

# Copy the rest of the source code
COPY src/Application/ Application/
COPY src/Domain/ Domain/
COPY src/Infrastructure/ Infrastructure/
COPY src/WebApi/ WebApi/

# Build and publish the WEBAPI project
RUN dotnet publish WebApi/WebApi.csproj -c Release -o /app/publish --configfile /root/.nuget/NuGet/NuGet.Config

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /app/publish .
EXPOSE 8080

# Run the app
ENTRYPOINT ["dotnet", "WebApi.dll"]
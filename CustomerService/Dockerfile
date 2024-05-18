#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["CustomerService/CustomerService.csproj", "CustomerService/"]
COPY ["Middleware/Middleware.csproj", "Middleware/"]
RUN dotnet restore "./CustomerService/CustomerService.csproj"

# Copy the entire source and build the project
COPY . .
WORKDIR "/src/CustomerService"
RUN dotnet build "./CustomerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CustomerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "CustomerService.dll"]
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copy the .csproj file from the root of the build context
COPY ["Mycompany.Finance.Calendar.csproj", "./"]
# Restore dependencies
RUN dotnet restore "./Mycompany.Finance.Calendar.csproj"
# Copy all files from the build context
COPY . .
# Set the working directory to the project folder
WORKDIR "/src"
# Build the project
RUN dotnet build "./Mycompany.Finance.Calendar.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
# Publish the application
RUN dotnet publish "./Mycompany.Finance.Calendar.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
# Copy the published output from the previous stage
COPY --from=publish /app/publish .
# Set the entry point to start the application
ENTRYPOINT ["dotnet", "Mycompany.Finance.Calendar.dll"]
# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy all projects
COPY ["backend/GoodHamburger.API/GoodHamburger.API.csproj", "backend/GoodHamburger.API/"]
COPY ["backend/GoodHamburger.Application/GoodHamburger.Application.csproj", "backend/GoodHamburger.Application/"]
COPY ["backend/GoodHamburger.Domain/GoodHamburger.Domain.csproj", "backend/GoodHamburger.Domain/"]
COPY ["backend/GoodHamburger.Infrastructure/GoodHamburger.Infrastructure.csproj", "backend/GoodHamburger.Infrastructure/"]

# Restore
RUN dotnet restore "backend/GoodHamburger.API/GoodHamburger.API.csproj"

# Copy everything else
COPY . .

# Build and Publish
WORKDIR "/src/backend/GoodHamburger.API"
RUN dotnet publish "GoodHamburger.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port (Render will use this)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "GoodHamburger.API.dll"]

# Use the official .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY EntityFrameWork_Pro/*.csproj ./EntityFrameWork_Pro/
RUN dotnet restore ./EntityFrameWork_Pro/EntityFrameWork_Pro.csproj

# Copy everything else and build
COPY EntityFrameWork_Pro/ ./EntityFrameWork_Pro/
WORKDIR /app/EntityFrameWork_Pro
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/EntityFrameWork_Pro/out .

# Expose port
EXPOSE 8080

# Create startup script to handle PORT env var
RUN echo '#!/bin/sh\nexport ASPNETCORE_URLS="http://+:${PORT:-8080}"\nexec dotnet EntityFrameWork_Pro.dll' > /app/start.sh && chmod +x /app/start.sh

ENTRYPOINT ["/app/start.sh"]

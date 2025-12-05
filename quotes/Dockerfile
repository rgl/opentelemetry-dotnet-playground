# syntax=docker/dockerfile:1.20

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN <<'EOF'
#!/bin/bash
set -euxo pipefail
apt-get update
apt-get install -y --no-install-recommends curl
rm -rf /var/lib/apt/lists/*
EOF
WORKDIR /app
COPY --from=builder /app/bin/Release/net10.0/publish ./
ENTRYPOINT ["/app/Quotes"]

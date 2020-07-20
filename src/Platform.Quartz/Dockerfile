FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore -r linux-musl-x64

COPY . .
RUN dotnet publish -c Release -o /app -r linux-musl-x64 --no-restore

FROM masstransit/platform:7.0.2
WORKDIR /app
COPY --from=build /app ./

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore -r linux-musl-x64

COPY . .
RUN dotnet publish -c Release -o /app -r linux-musl-x64 --no-restore

FROM masstransit/platform:7.1.3
RUN apk add --no-cache tzdata
WORKDIR /app
COPY --from=build /app ./


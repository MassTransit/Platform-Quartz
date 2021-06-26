FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY *.csproj .
RUN dotnet restore -r linux-musl-x64

COPY . .
RUN dotnet publish -c Release -o /app -r linux-musl-x64 --no-restore

FROM masstransit/platform:7.2.1
RUN apk add --no-cache tzdata
WORKDIR /app
COPY --from=build /app ./


FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY ./src .
RUN dotnet publish ./Platform.Quartz/Platform.Quartz.csproj -c Release -o /app -r linux-musl-x64

FROM masstransit/platform:7.2.3
RUN apk add --no-cache tzdata
WORKDIR /app
COPY --from=build /app ./


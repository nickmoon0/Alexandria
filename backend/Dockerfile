# Create a stage for building the application.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source
COPY . .

ARG TARGETARCH

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Run tests
RUN dotnet test /source/Alexandria.sln

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS development
COPY . /source
WORKDIR /source/src/Alexandria.Api
CMD dotnet run --no-launch-profile

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=build /app .

ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser
    
USER appuser

ENTRYPOINT ["dotnet", "Alexandria.Api.dll"]

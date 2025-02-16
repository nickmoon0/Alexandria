# Build Stage
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /source
COPY . .

ARG TARGETARCH

RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "/source/src/Alexandria.CoreApi/Alexandria.CoreApi.csproj" -a ${TARGETARCH/amd64/x64} --use-current-runtime --self-contained false -o /app

# Run tests
RUN dotnet test "/source/Alexandria.sln"

# Dev Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS development
COPY . /source
WORKDIR /source/src/Alexandria.CoreApi
CMD dotnet run --no-launch-profile

# Release Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
RUN apk add --no-cache acl

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
    
RUN mkdir -p /fileuploads
RUN chown -R appuser:appuser /fileuploads
# Owner read/write, no execute or list directory
RUN setfacl -d -m u:appuser:rw /fileuploads \
    && setfacl -d -m g::--- /fileuploads \
    && setfacl -d -m o::--- /fileuploads \
    && setfacl -m u:appuser:rw /fileuploads \
    && setfacl -m g::--- /fileuploads \
    && setfacl -m o::--- /fileuploads

USER appuser

ENTRYPOINT ["dotnet", "Alexandria.CoreApi.dll"]
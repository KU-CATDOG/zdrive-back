# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY ZDrive/*.csproj .
RUN dotnet restore --use-current-runtime  

# copy everything else and build app
COPY ZDrive/. .
RUN dotnet publish --no-restore --no-self-contained --use-current-runtime -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app .
RUN mkdir database
RUN mkdir wwwroot
ENTRYPOINT ["dotnet", "zdrive-back.dll"]
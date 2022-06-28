#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

ARG PAT

RUN dotnet new nugetconfig
RUN dotnet nuget add source "https://pkgs.dev.azure.com/asos/_packaging/PE-ASOS/nuget/v2/" -n PrivateFeed -u docker -p ${PAT} --store-password-in-clear-text --configfile nuget.config

COPY ["SimpleApi/SimpleApi.csproj", "SimpleApi/"]
RUN dotnet restore "SimpleApi/SimpleApi.csproj" --source "https://pkgs.dev.azure.com/asos/_packaging/PE-ASOS/nuget/v2/"
COPY . .
WORKDIR "/src/SimpleApi"
RUN dotnet build "SimpleApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SimpleApi.dll"]
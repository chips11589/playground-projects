#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MessageProcessor/MessageProcessor.csproj", "MessageProcessor/"]
RUN dotnet restore "MessageProcessor/MessageProcessor.csproj"
COPY . .
WORKDIR "/src/MessageProcessor"
RUN dotnet build "MessageProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageProcessor.dll"]
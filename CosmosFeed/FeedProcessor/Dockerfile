#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["FeedProcessor/FeedProcessor.csproj", "FeedProcessor/"]
COPY ["Data/Data.csproj", "Data/"]
RUN dotnet restore "FeedProcessor/FeedProcessor.csproj"
COPY . .
WORKDIR "/src/FeedProcessor"
RUN dotnet build "FeedProcessor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FeedProcessor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FeedProcessor.dll"]
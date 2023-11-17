#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

ENV DB_Server "172.17.0.5"

WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["N5.Challenge.API/N5.Challenge.API.csproj", "N5.Challenge.API/"]
COPY ["N5.Challenge.Infrasctructure/N5.Challenge.Infrasctructure.csproj", "N5.Challenge.Infrasctructure/"]
COPY ["N5.Challenge.Domain/N5.Challenge.Domain.csproj", "N5.Challenge.Domain/"]
RUN dotnet restore "N5.Challenge.API/N5.Challenge.API.csproj"
COPY . .
WORKDIR "/src/N5.Challenge.API"
RUN dotnet build "N5.Challenge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "N5.Challenge.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "N5.Challenge.API.dll"]
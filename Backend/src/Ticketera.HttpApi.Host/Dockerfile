#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/Ticketera.HttpApi.Host/Ticketera.HttpApi.Host.csproj", "src/Ticketera.HttpApi.Host/"]
COPY ["src/Ticketera.Application/Ticketera.Application.csproj", "src/Ticketera.Application/"]
COPY ["src/Ticketera.Domain/Ticketera.Domain.csproj", "src/Ticketera.Domain/"]
COPY ["src/Ticketera.Domain.Shared/Ticketera.Domain.Shared.csproj", "src/Ticketera.Domain.Shared/"]
COPY ["src/Ticketera.Application.Contracts/Ticketera.Application.Contracts.csproj", "src/Ticketera.Application.Contracts/"]
COPY ["src/Ticketera.EntityFrameworkCore/Ticketera.EntityFrameworkCore.csproj", "src/Ticketera.EntityFrameworkCore/"]
COPY ["src/Ticketera.HttpApi/Ticketera.HttpApi.csproj", "src/Ticketera.HttpApi/"]
RUN dotnet restore "src/Ticketera.HttpApi.Host/Ticketera.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/Ticketera.HttpApi.Host"
RUN dotnet build "Ticketera.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ticketera.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ticketera.HttpApi.Host.dll"]
#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV ASPNETCORE_ENVIRONMENT=Development

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["CCIIdentity/CCIIdentity.csproj", "CCIIdentity/"]
COPY ["CCI.Common/CCI.Common.csproj", "CCI.Common/"]
COPY ["CCI.Domain/CCI.Domain.csproj", "CCI.Domain/"]
COPY ["CCI.Model/CCI.Model.csproj", "CCI.Model/"]
COPY ["CCI.Repository/CCI.Repository.csproj", "CCI.Repository/"]
COPY ["CCI.Service/CCI.Service.csproj", "CCI.Service/"]
RUN dotnet restore "CCIIdentity/CCIIdentity.csproj"
COPY . .
WORKDIR "/src/CCIIdentity"
RUN dotnet build "CCIIdentity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CCIIdentity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CCIIdentity.dll"]
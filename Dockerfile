#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
RUN apt-get update -qq && apt-get -y install libgdiplus libc6-dev
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Reports.csproj", "."]
RUN dotnet restore "./Reports.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Reports.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Reports.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN chmod 755 /app/Rotativa/Linux/wkhtmltopdf
ENTRYPOINT ["dotnet", "Reports.dll"]
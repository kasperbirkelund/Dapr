FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5226

ENV ASPNETCORE_URLS=http://+:5226

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src

COPY ["src/Pal.Api/Pal.Api.csproj", "src/Pal.Api/"]
RUN dotnet restore "src/Pal.Api/Pal.Api.csproj"
COPY . .
WORKDIR "/src/src/Pal.Api"
RUN dotnet build "Pal.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pal.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pal.Api.dll"]

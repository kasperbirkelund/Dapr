FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 5221

ENV ASPNETCORE_URLS=http://+:5221

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["src/Palprimes.Common/Palprimes.Common.csproj", "src/Palprimes.Common/"]
COPY ["src/Palprimes.Api/Palprimes.Api.csproj", "src/Palprimes.Api/"]
RUN dotnet restore "src/Palprimes.Api/Palprimes.Api.csproj"
COPY . .
WORKDIR "/src/src/Palprimes.Api"
RUN dotnet build "Palprimes.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Palprimes.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Palprimes.Api.dll"]

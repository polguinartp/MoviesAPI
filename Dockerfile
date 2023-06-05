FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["MoviesAPI/MoviesAPI.csproj", "MoviesAPI/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "MoviesAPI/MoviesAPI.csproj"
COPY . .
WORKDIR "/src/MoviesAPI"
RUN dotnet build "MoviesAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoviesAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoviesAPI.dll"]
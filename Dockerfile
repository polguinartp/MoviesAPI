FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS build
WORKDIR /src

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY ./publish .

ENTRYPOINT ["dotnet", "MoviesAPI.dll"]

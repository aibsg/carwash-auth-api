FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /api

COPY ./*.csproj .
RUN dotnet restore
COPY . .
RUN dotnet build carwash-auth-api.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish carwash-auth-api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "carwash-auth-api.dll"]
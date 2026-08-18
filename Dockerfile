



FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish "MVC Project.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "MVC Project.dll"]
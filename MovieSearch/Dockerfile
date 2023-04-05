FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MovieSearch/MovieSearch.csproj", "MovieSearch/"]
RUN dotnet restore "MovieSearch/MovieSearch.csproj"
COPY . .
WORKDIR "/src/MovieSearch"
RUN dotnet build "MovieSearch.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieSearch.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieSearch.dll"]

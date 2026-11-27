FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["Todo_Application.sln", "./"]
COPY ["Todo_Application/Todo_Application.csproj", "Todo_Application/"]
RUN dotnet restore "Todo_Application.sln"

COPY . .

WORKDIR "/src/Todo_Application"
RUN dotnet build "Todo_Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Todo_Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Todo_Application.dll"]
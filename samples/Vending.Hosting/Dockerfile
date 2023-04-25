FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 11111
EXPOSE 30000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["samples/Vending.Hosting/Vending.Hosting.csproj", "samples/Vending.Hosting/"]
COPY ["src/SiloX.Orleans.Clustering.AdoNet/SiloX.Orleans.Clustering.AdoNet.csproj", "src/SiloX.Orleans.Clustering.AdoNet/"]
COPY ["src/SiloX.Orleans.Clustering/SiloX.Orleans.Clustering.csproj", "src/SiloX.Orleans.Clustering/"]
COPY ["src/SiloX.Orleans.Clustering.InMemory/SiloX.Orleans.Clustering.InMemory.csproj", "src/SiloX.Orleans.Clustering.InMemory/"]
COPY ["src/SiloX.Orleans.Clustering.Redis/SiloX.Orleans.Clustering.Redis.csproj", "src/SiloX.Orleans.Clustering.Redis/"]
COPY ["src/SiloX.Orleans.EventSourcing.EventStore/SiloX.Orleans.EventSourcing.EventStore.csproj", "src/SiloX.Orleans.EventSourcing.EventStore/"]
COPY ["src/SiloX.Orleans.EventSourcing/SiloX.Orleans.EventSourcing.csproj", "src/SiloX.Orleans.EventSourcing/"]
COPY ["src/SiloX.Orleans.EventSourcing.InMemory/SiloX.Orleans.EventSourcing.InMemory.csproj", "src/SiloX.Orleans.EventSourcing.InMemory/"]
COPY ["src/SiloX.Orleans.Persistence.AdoNet/SiloX.Orleans.Persistence.AdoNet.csproj", "src/SiloX.Orleans.Persistence.AdoNet/"]
COPY ["src/SiloX.Orleans.Persistence/SiloX.Orleans.Persistence.csproj", "src/SiloX.Orleans.Persistence/"]
COPY ["src/SiloX.Orleans.Persistence.EventStore/SiloX.Orleans.Persistence.EventStore.csproj", "src/SiloX.Orleans.Persistence.EventStore/"]
COPY ["src/SiloX.Orleans.Persistence.InMemory/SiloX.Orleans.Persistence.InMemory.csproj", "src/SiloX.Orleans.Persistence.InMemory/"]
COPY ["src/SiloX.Orleans.Persistence.Redis/SiloX.Orleans.Persistence.Redis.csproj", "src/SiloX.Orleans.Persistence.Redis/"]
COPY ["src/SiloX.Orleans.Reminders.AdoNet/SiloX.Orleans.Reminders.AdoNet.csproj", "src/SiloX.Orleans.Reminders.AdoNet/"]
COPY ["src/SiloX.Orleans.Reminders/SiloX.Orleans.Reminders.csproj", "src/SiloX.Orleans.Reminders/"]
COPY ["src/SiloX.Orleans.Reminders.InMemory/SiloX.Orleans.Reminders.InMemory.csproj", "src/SiloX.Orleans.Reminders.InMemory/"]
COPY ["src/SiloX.Orleans.Reminders.Redis/SiloX.Orleans.Reminders.Redis.csproj", "src/SiloX.Orleans.Reminders.Redis/"]
COPY ["src/SiloX.Orleans.Server/SiloX.Orleans.Server.csproj", "src/SiloX.Orleans.Server/"]
COPY ["src/SiloX.Orleans.Streaming.EventStore/SiloX.Orleans.Streaming.EventStore.csproj", "src/SiloX.Orleans.Streaming.EventStore/"]
COPY ["src/SiloX.Orleans.Streaming/SiloX.Orleans.Streaming.csproj", "src/SiloX.Orleans.Streaming/"]
COPY ["src/SiloX.Orleans.Streaming.InMemory/SiloX.Orleans.Streaming.InMemory.csproj", "src/SiloX.Orleans.Streaming.InMemory/"]
COPY ["src/SiloX.Orleans.Transactions/SiloX.Orleans.Transactions.csproj", "src/SiloX.Orleans.Transactions/"]
COPY ["samples/Vending.Domain.Abstractions/Vending.Domain.Abstractions.csproj", "samples/Vending.Domain.Abstractions/"]
COPY ["src/SiloX.Domain.Abstractions/SiloX.Domain.Abstractions.csproj", "src/SiloX.Domain.Abstractions/"]
COPY ["samples/Vending.Domain/Vending.Domain.csproj", "samples/Vending.Domain/"]
COPY ["samples/Vending.Domain.EntityFrameworkCore/Vending.Domain.EntityFrameworkCore.csproj", "samples/Vending.Domain.EntityFrameworkCore/"]
COPY ["samples/Vending.Projection.Abstractions/Vending.Projection.Abstractions.csproj", "samples/Vending.Projection.Abstractions/"]
COPY ["samples/Vending.Projection/Vending.Projection.csproj", "samples/Vending.Projection/"]
COPY ["samples/Vending.Projection.EntityFrameworkCore/Vending.Projection.EntityFrameworkCore.csproj", "samples/Vending.Projection.EntityFrameworkCore/"]
RUN dotnet restore "samples/Vending.Hosting/Vending.Hosting.csproj"
COPY . .
WORKDIR "/src/samples/Vending.Hosting"
RUN dotnet build "Vending.Hosting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vending.Hosting.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vending.Hosting.dll"]

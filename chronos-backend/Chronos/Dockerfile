# ==========================================
# ETAPA 1: Build da aplicação
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências (otimiza o cache do Docker)
COPY ["Chronos.csproj", "./"]
RUN dotnet restore "Chronos.csproj"

# Copia todo o restante do código fonte
COPY . .
WORKDIR "/src/."

# Compila o projeto em modo Release e gera a publicação na pasta /app/publish
RUN dotnet publish "Chronos.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# ETAPA 2: Imagem final para execução (Runtime)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia os arquivos publicados da etapa anterior
COPY --from=build /app/publish .

# Expõe a porta padrão que a aplicação .NET vai escutar no container
EXPOSE 8080

# Define a porta padrão via variável de ambiente do ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Comando para iniciar a aplicação
ENTRYPOINT ["dotnet", "Chronos.dll"]
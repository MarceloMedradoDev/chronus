# ==========================================
# Script para Execução Local (Sem Docker)
# ==========================================

Write-Host "Iniciando o ecossistema Chronos localmente..." -ForegroundColor Cyan

# 1. Configurar variáveis de ambiente do Backend para a sessão atual (opcional se já usar appsettings)
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:JWT_SECRET_KEY = "JHD7812hd91HSA8192hdsu812HSAU812!!aAjsd92182jsd82h1jsAJS12hs81j2"
$env:JWT_ISSUER = "Chronos.Api"
$env:JWT_AUDIENCE = "Chronos.Client"
$env:DB_CONNECTION = "Server=DELL7000\TESTE;Database=Chronos;User Id=sa;Password=job2026;TrustServerCertificate=True;"
$env:AUTH_PM_LOGIN = "03589823593"
$env:AUTH_PM_PASS = "Sistemas@99"

# 2. Iniciar o Backend (.NET)
Write-Host "Subindo o Backend (.NET)..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd C:\Workspace\Chronos\chronos-backend\Chronos; dotnet run"

# 3. Iniciar o Frontend (Angular)
Write-Host "Subindo o Frontend (Angular)..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd C:\Workspace\Chronos\chronos-frontend; npm start"
Start-Sleep -Seconds 5

# 4. Iniciar o Selenium (Nota: O Selenium local geralmente roda via WebDriver do navegador ou Selenium Standalone se tiver Java)
Write-Host "Aviso: Para o Selenium, certifique-se de que o ChromeDriver/GeckoDriver está no PATH ou que sua aplicação de testes está configurada para chamá-lo." -ForegroundColor Yellow

Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd C:\Workspace\Chronos\chronos-selenium\chronos-selenium; dotnet run"

Write-Host "Ambiente local iniciado! Verifique as novas janelas do PowerShell." -ForegroundColor Cyan
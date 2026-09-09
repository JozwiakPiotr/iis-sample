# CI/CD
## wypróbować github self-hosted runners
`deploy.ps1`
```powershell
param (
    [Parameter(Mandatory=$true)]
    [string]$ServicePassword
)

$UserName = "MyServiceUser"
$ServiceName = "MyAwesomeService"
$ServiceExePath = "C:\App\MyService.exe"
$UrlAcl = "http://+:8080/MyService/"

# 1. Idempotentne tworzenie użytkownika lokalnego
if (-Not (Get-LocalUser -Name $UserName -ErrorAction SilentlyContinue)) {
    Write-Host "Tworzenie użytkownika: $UserName"
    $SecurePassword = ConvertTo-SecureString $ServicePassword -AsPlainText -Force
    New-LocalUser -Name $UserName -Password $SecurePassword -Description "Konto dla usługi" -PasswordNeverExpires $true
} else {
    Write-Host "Użytkownik $UserName już istnieje."
}

# 2. Idempotentna rezerwacja URL (URLACL)
$AclCheck = netsh http show urlacl url=$UrlAcl | Out-String
if ($AclCheck -notmatch "Reserved URL") {
    Write-Host "Rezerwowanie adresu URL: $UrlAcl"
    # Używamy cmd /c, aby poprawnie przekazać parametry do netsh
    cmd.exe /c "netsh http add urlacl url=$UrlAcl user=$UserName"
} else {
    Write-Host "Adres URL $UrlAcl jest już zarezerwowany."
}

# 3. Kopiowanie nowych plików aplikacji (zakładając, że są w obecnym katalogu roboczym)
# Należy zatrzymać usługę przed nadpisaniem plików, jeśli już istnieje
$Service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($Service -and $Service.Status -eq 'Running') {
    Write-Host "Zatrzymywanie usługi $ServiceName przed aktualizacją..."
    Stop-Service -Name $ServiceName -Force
}

Write-Host "Kopiowanie plików aplikacji do C:\App\..."
if (!(Test-Path "C:\App")) { New-Item -ItemType Directory -Force -Path "C:\App" }
Copy-Item -Path ".\bin\Release\net8.0\win-x64\publish\*" -Destination "C:\App\" -Recurse -Force

# 4. Idempotentne tworzenie lub aktualizacja usługi Windows
# Narzędzie sc.exe jest najbardziej niezawodne do ustawiania poświadczeń. Uwaga: spacja po znaku równości jest wymagana!
if (-Not $Service) {
    Write-Host "Tworzenie nowej usługi: $ServiceName"
    sc.exe create $ServiceName binPath= $ServiceExePath start= auto obj= ".\$UserName" password= $ServicePassword
} else {
    Write-Host "Aktualizacja konfiguracji istniejącej usługi: $ServiceName"
    sc.exe config $ServiceName binPath= $ServiceExePath start= auto obj= ".\$UserName" password= $ServicePassword
}

# 5. Uruchomienie usługi
Write-Host "Uruchamianie usługi $ServiceName..."
Start-Service -Name $ServiceName
```
```yaml
name: Deploy Windows Service

on:
  push:
    branches:
      - main

jobs:
  deploy:
    runs-on: self-hosted # Uruchamia zadanie na Twoim własnym agencie Windows Server
    
    steps:
      - name: Pobranie kodu repozytorium
        uses: actions/checkout@v4

      - name: Budowanie aplikacji (.NET przykładowo)
        run: dotnet publish -c Release -r win-x64 --self-contained true

      - name: Uruchomienie skryptu wdrożeniowego
        env:
          SERVICE_PASSWORD: ${{ secrets.SERVICE_PASSWORD }}
        run: |
          .github\scripts\deploy.ps1 -ServicePassword $env:SERVICE_PASSWORD
        shell: powershell
```
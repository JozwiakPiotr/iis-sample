# Wdrażanie aplikacji na windows server
## Upload binarek przez sftp
Wymagany wcześniej skonfigurowany serwer ssh
`~/.ssh/config`
```
Host 192.168.101.*
    IdentitiesOnly yes
    PubkeyAuthentication no
```
W menadżerze plików wejśc na `sftp://Administrator@192.168.101.3/C:/` i skopiować

## instalacja runtime
pobrać dla WEB01 i API01 odpowiednio HostBundle (uwaga, dla WEB01 najpierw zainstalować IIS, potem HostBundle, na koniec zrestartować IIS)

## backend API - windows service
### certyfikat
```bash
$cert = New-SelfSignedCertificate `
  -DnsName "backend.contoso.lab" `
  -CertStoreLocation "Cert:\LocalMachine\My" `
  -FriendlyName "Backend API - backend.contoso.lab" `
  -KeyAlgorithm RSA `
  -KeyLength 2048 `
  -HashAlgorithm SHA256 `
  -NotAfter (Get-Date).AddYears(2)

Export-Certificate `
  -Cert $cert `
  -FilePath "C:\Download\backend.contoso.lab.cer"
```

### windows service
```bash
# nie wiem czy to konieczne, spróbować następnym razem bez tego
Install-WindowsFeature RSAT-AD-PowerShell
```

```bash
New-ADUser -Name "svc-backend" `
           -SamAccountName "svc-backend" `
           -UserPrincipalName "svc-backend@contoso.lab" `
           -AccountPassword ('Zaq12wsx' | ConvertTo-SecureString -AsPlainText -Force) `
           -Enabled $true `
           -PasswordNeverExpires $true

$servicePath = 'C:\Services\Backend'
$serviceUser = 'CONTOSO\svc-backend'
$password    = 'Zaq12wsx' | ConvertTo-SecureString -AsPlainText -Force
$cred        = [System.Management.Automation.PSCredential]::new($serviceUser, $password)

$acl = Get-Acl -Path $servicePath
$acl.AddAccessRule([System.Security.AccessControl.FileSystemAccessRule]::new(
    $serviceUser,
    'ReadAndExecute',
    'ContainerInherit, ObjectInherit',
    'None',
    'Allow'
))
Set-Acl -Path $servicePath -AclObject $acl

$dllPath    = Join-Path $servicePath 'BackendService.dll'

$serviceParams = @{
    Name           = 'BackendService'
    DisplayName    = 'Backend API'
    BinaryPathName = "dotnet $dllPath --contentRoot $servicePath --urls=https://+:5001" # ciekawe jak to wtedy działa --urls=https://backend.contoso.lab:5001"
    Credential     = $cred
    StartupType    = 'Automatic'
}

New-Service @serviceParams

netsh http add urlacl url=https://+:5001/ user="CONTOSO\svc-backend"
netsh http add sslcert ipport=0.0.0.0:5001 certhash=3F969797530D0C12B347E3E8BEB15C4D5F1A95E8 certstorename=MY appid="{20179b1c-94d5-4e2b-bdac-5770b6f604e1}"

# netsh http add urlacl url=https://backend.contoso.lab:5001/ user="CONTOSO\svc-backend"
# netsh http add sslcert hostnameport=backend.contoso.lab:5001 certhash=3F969797530D0C12B347E3E8BEB15C4D5F1A95E8 certstorename=MY appid="{20179b1c-94d5-4e2b-bdac-5770b6f604e1}"
```
#### Uprawnienia Log on as a service
Na komputerze z GUI (PC01) zainstalować RSAT Group Policy Management

`Settings > System > Optional features > View features > See available features > search: "RSAT: Group Polic Management Tools"`

Po zainstalowaniu

`Group Policy Management > lewe menu > Forest: contoso.lab > Domains > contoso.lab > Group Policy Objects > Prawy Klik > New`

Nadajemy nazwę `Service Logon Rights` i tworzymy, następnie

`Group Policy Management > Service Logon Rights > edit > Computer Configuration > Policies > Windows Settings > Security Settings > Local Policies > User Rights Assignment > prawe okno > Log on as a service > lewy dwuklik`

klikamy Define thes policy settings, następnie Add User or Group i dodajemy użytkownika

następnie linkujemy GPO do OU (constoso.lab)

`contoso.lab > prawy klik > Link an existing GPO > Service Logon Rights > OK`

na koniec `gpupdate /force` i możemy uruchomić usługę

## frontend
### certyfikat
```bash
$cert = New-SelfSignedCertificate `
  -Type SSLServerAuthentication `
  -DnsName "frontend.contoso.lab" `
  -CertStoreLocation "Cert:\LocalMachine\My" `
  -FriendlyName "Frontend IIS - frontend.contoso.lab" `
  -KeyAlgorithm RSA `
  -KeyLength 2048 `
  -HashAlgorithm SHA256 `
  -NotAfter (Get-Date).AddYears(2)

Export-Certificate `
  -Cert $cert `
  -FilePath "C:\Download\frontend.contoso.lab.cer" `
  -Force
```

### IIS
#### instalacja IIS

`Install-WindowsFeature -name Web-Server -IncludeManagementTools` albo też `Enable-WindowsOptionalFeature -Online -FeatureName "IIS-WebServerRole" -All` albo 
```bash 
import-module servermanager
add-windowsfeature web-server -includeallsubfeature
```
chuj wie które poprawne, nie pamiętam które działało

następnie zaisntalować Hosting Bundle, potem zrestartować IIS 
```bash
net stop was /y
net start w3svc
```

#### Tworzenie strony w IIS

```bash
$certStoreLocation = "Cert:\LocalMachine\My"
$thumbprint = (gci $certStoreLocation | where { $_.Subject -match "frontend.contoso.lab" }).thumbprint
$siteName = "frontend"
$sitePath = "C:\frontend"
$bindingInformation = "*:443:"

Import-Module WebAdministration

# tutaj jakaś stary sposób:
# New-WebAppPool -Name $siteName
# New-Website -Name $SiteName -Port 80 -PhysicalPath $Path -ApplicationPool $SiteName
# poniżej jest chyba nowszy, nie wymaga osobnego robienie WebAppPool

New-IISSite -Name $siteName -PhysicalPath $sitePath -BindingInformation $bindingInformation -CertificateThumbPrint $thumbprint -CertStoreLocation $certStoreLocation -Protocol https
```

---
## DNS
```bash
# DC01
Add-DnsServerResourceRecordA -ZoneName "contoso.lab" -Name "backend" -IPv4Address "192.168.101.3"
Add-DnsServerResourceRecordA -ZoneName "contoso.lab" -Name "frontend" -IPv4Address "192.168.101.4"
```

## Instalacja certów
```bash
# API01 i WEB01
Enable-NetFirewallRule -DisplayGroup "File and Printer Sharing"
Get-SmbShare -Name "C$"
Get-Service -Name LanmanServer
```
```bash
Import-Certificate -FilePath "\\API01\c$\Download\backend.contoso.lab.cer" -CertStoreLocation "Cert:\LocalMachine\Root"
Import-Certificate -FilePath "\\WEB01\c$\Download\frontend.contoso.lab.cer" -CertStoreLocation "Cert:\LocalMachine\Root"
```

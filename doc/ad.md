# Konfiguracja Active Directory
## Kontroler domeny
`DC01`
```bash
Install-WindowsFeature AD-Domain-Services -IncludeManagementTools

$dsrmPassword = ConvertTo-SecureString "Zaq12wsx" -AsPlainText -Force

Install-ADDSForest `
    -DomainName "contoso.lab" `
    -InstallDns `
    -SafeModeAdministratorPassword $dsrmPassword `
    -Force
```

## Dołączanie do domeny

```bash
$adapters = Get-NetAdapter |
    Where-Object {
        $_.Status -eq "Up" -and
        $_.HardwareInterface -eq $true
    }

foreach ($adapter in $adapters) {
    Set-DnsClientServerAddress `
        -InterfaceIndex $adapter.ifIndex `
        -ServerAddresses "192.168.101.2"
}

ipconfig /flushdns
nslookup contoso.lab

$domainCredential = Get-Credential "CONTOSO\Administrator"

Add-Computer `
    -DomainName "contoso.lab" `
    -Credential $domainCredential `
    -Restart
```


```bash
Import-Module ActiveDirectory

$groupName = "Negocjatorzy"
$userName = "john"
$userPassword =  ConvertTo-SecureString "Zaq12wsx" -AsPlainText -Force

New-ADGroup `
    -Name $groupName `
    -SamAccountName $groupName `
    -GroupScope Global `
    -GroupCategory Security `
    -Path "CN=Users,DC=contoso,DC=lab"

New-ADUser `
    -Name "John" `
    -GivenName "John" `
    -SamAccountName $userName `
    -UserPrincipalName "$userName@contoso.lab" `
    -AccountPassword $userPassword `
    -Enabled $true `
    -ChangePasswordAtLogon $false `
    -Path "CN=Users,DC=contoso,DC=lab"

Add-ADGroupMember `
    -Identity $groupName `
    -Members $userName
```


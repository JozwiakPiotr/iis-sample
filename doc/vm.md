# Przygotowanie wirtualnej maszyny Windows Server 2025
## Do pobrania
- [ISO win2k25](https://software-static.download.prss.microsoft.com/dbazure/998969d5-f34g-4e03-ac9d-1f9786c66749/26100.32230.260111-0550.lt_release_svc_refresh_SERVER_EVAL_x64FRE_en-us.iso)
- [ISO virtio-win 0.1.302](https://fedorapeople.org/groups/virt/virtio-win/direct-downloads/archive-virtio/virtio-win-0.1.302-1/virtio-win-0.1.302.iso)

## Zasoby
- CPU: 2
- RAM: 4 GB
- HDD: 40 GB

## Sieć
### DHCP
warto pomyśleć nad jawnym ustawianiem MAC i przypisania do niego IP w DHCP

adresy MAC w qemu mają format `52:54:00:xx:xx:xx`
`definicja sieci, sekcja DHCP'
```xml
  <dhcp>
    <range start="xxx.xxx.xxx.xxx" end="xxx.xxx.xxx.xxx" />
    <host mac="52:54:00:xx:xx:xx" name="hostname" ip="xxx.xxx.xxx.xxx" />
  </dhcp>
```
```bash
virt-install \
# ...
--network network=default,model=virtio,mac=52:54:00:12:34:56
```
### DNS
fajne rozwiązanie split dns
```bash
sudo mkdir -p /etc/systemd/resolved.conf.d/
sudo tee /etc/systemd/resolved.conf.d/network-split.conf << 'EOF'
[Resolve]
DNS=192.168.101.1
Domains=~network
EOF
sudo systemctl restart systemd-resolved
resolvectl status
```

## Instalacja agenta qemu i sterowników VirtIO
Zamntować obraz ISO virtio-win, a następnie
```
D:
./virtio-win-guest-tools.exe
```
Przejść przez instalator w formie GUI. Sprawdzić czy agent działa
```
Get-Service qemu-ga
```
Dodać w libvirt maszynie wirtualnej channel z name - org.qemu.guest_agent.0
## Zdalny dostęp
Preferować RDP ponieważ ssh jest bezużyteczne windows

### RDP
```
Set-ItemProperty -Path 'HKLM:\System\CurrentControlSet\Control\Terminal Server' -Name "fDenyTSConnections" -Value 0
set-service TermService -StartupType 'Automatic'
start-service TermService
Enable-NetFirewallRule -DisplayGroup "Remote Desktop"
```

### SSH
`Set-Service -Name sshd -StartupType 'Automatic'`
Host nie będzie się mógł połączyć do windows server ponieważ zakfalifikuje go do profilu public a połączenia ssh są możliwe tylko w profilu domain i private, dlatego ustawiamy to połączenie jako private.
```bash
# To ustawia profil private na wszystkich interfejsach, w moim przypadku jest jeden, chociaż to nie zadziała gdy podłączym się do domeny
Set-NetConnectionProfile -NetworkCategory Private
# drugi sposób działa zawsze
Set-NetFirewallRule -Name "OpenSSH-Server-In-TCP" -Profile Any
```
ssh będzie próbowało się logować każdym certem z maszyny jaki znajdzie, co spowoduje `Too many authentication failures` dlatego:
```
ssh -o PubkeyAuthentication=no Administrator@192.168.101.222
```

## Reset
W celu uniknięcia konfliktu SID `C:\Windows\System32\Sysprep\sysprep.exe /generalize /oobe /shutdown`
Usunąć VM i zostawić dysk .qcow2.

### Wybróbować reset
Plik `C:\Windows\System32\Sysprep\unattend.xml`
```xml
<?xml version="1.0" encoding="utf-8"?>
<unattend xmlns="urn:schemas-microsoft-com:unattend">
    <settings pass="oobeSystem">
        <component name="Microsoft-Windows-Shell-Setup" processorArchitecture="amd64" publicKeyToken="31bf3856ad364e35" language="neutral" versionScope="nonSxS">
            <OOBE>
                <HideEULAPage>true</HideEULAPage>
                <HideOnlineAccountScreens>true</HideOnlineAccountScreens>
                <HideWirelessSetupInOOBE>true</HideWirelessSetupInOOBE>
                <ProtectYourPC>3</ProtectYourPC>
            </OOBE>
        </component>
    </settings>
    <settings pass="specialize">
        <component name="Microsoft-Windows-Shell-Setup" processorArchitecture="amd64" publicKeyToken="31bf3856ad364e35" language="neutral" versionScope="nonSxS">
            <!-- Pusty znacznik ComputerName lub jego brak wymusi pytanie o hostname w OOBE -->
        </component>
    </settings>
</unattend>
```
```bash
C:\Windows\System32\Sysprep\sysprep.exe /generalize /oobe /shutdown /unattend:C:\Windows\System32\Sysprep\unattend.xml
```

# Windows 11
## Bypass
```
Shift + f10
start ms-cxh:localonly
```
## Zainstalować
- virtio-win z ISO
- dodać channel name - org.qemu.guest_agent.0

## sysprep

### Wyłączyć BitLocker
`Disable-BitLocker -MountPoint "C:"`

### Uruchomić sysprep

`C:\Windows\System32\Sysprep\unattend.xml`
```xml
<?xml version="1.0" encoding="utf-8"?>
<unattend xmlns="urn:schemas-microsoft-com:unattend">
    <settings pass="oobeSystem">
        <component name="Microsoft-Windows-Shell-Setup" processorArchitecture="amd64" publicKeyToken="31bf3856ad364e35" language="neutral" versionScope="nonSxS" xmlns:wcm="http://schemas.microsoft.com/WMIConfig/2002/State" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
            <OOBE>
                <HideEULAPage>true</HideEULAPage>
                <HideOnlineAccountScreens>true</HideOnlineAccountScreens>
                <ProtectYourPC>1</ProtectYourPC>
            </OOBE>
        </component>
    </settings>
</unattend>
```
```bash
cd C:\Windows\System32\Sysprep
sysprep.exe /generalize /oobe /shutdown /unattend:unattend.xml
```
Następnie usunąć maszynę i zostawić dysk qcow2

## Ponowne uruchomienie 
- `Rename-Computer {twoja nazwa}`

## Rezultat
Słabo. Cały proces po ponownym uruchomieniu jest dość długi i czasochłonny. A i tak nie można ustawić nazwy hosta.

Podsumowanie:
- ponowna konfiguracja języka systemu, klawiatury itp
- system nie próbuje zalogować mnie do konta microsoft
- hasło + 3 pytania pomocniczne
- nie trzeba akceptować EULA, metryk itp
- nie można ustawić nazwy komputera

# Środowisko

| host | IP
| - | - |
| DC01 | 192.168.101.2
| API01 | 192.168.101.3
| WEB01 | 192.168.101.4
| PC01 | 192.168.101.5

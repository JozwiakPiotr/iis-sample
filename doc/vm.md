# Przygotowanie wirtualnej maszyny Windows Server 2025
## Do pobrania
- [ISO win2k25](https://software-static.download.prss.microsoft.com/dbazure/998969d5-f34g-4e03-ac9d-1f9786c66749/26100.32230.260111-0550.lt_release_svc_refresh_SERVER_EVAL_x64FRE_en-us.iso)
- [ISO virtio-win 0.1.302](https://fedorapeople.org/groups/virt/virtio-win/direct-downloads/archive-virtio/virtio-win-0.1.302-1/virtio-win-0.1.302.iso)

## Zasoby
- CPU: 2
- RAM: 4 GB
- HDD: 40 GB

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
## SSH
`Set-Service -Name sshd -StartupType 'Automatic'`

## Reset
W celu uniknięcia konfliktu SID `C:\Windows\System32\Sysprep\sysprep.exe /generalize /oobe /shutdown`
Usunąć VM i zostawić dysk .qcow2.


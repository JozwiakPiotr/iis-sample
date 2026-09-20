```bash
$browser="C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
& $browser --proxy-server="http://192.168.101.1:8080" --auth-server-allowlist="*.contoso.lab" "http://192.168.101.1:8081"
```

Nie można użyć bo jakiekolwiek proxy https psuje NTLM

Deszyfrowanie ruchu *RPC* (*Netlogon Secure Channel*) przesyłany najczęściej przez *SMB* lub TCP API01 - DC01 jest zbyt czasochłonne. Trzeba by użyć narzędzi takich jak *Mimikatz*, wpiąć się w pamięć procesu *LSASS* na API01

```bash
Enable-NetFirewallRule -DisplayGroup "Remote Event Log Management"
```

dostaje błąd
```
0xC000035B

STATUS_BAD_BINDINGS
	

The client's supplied SSPI channel bindings were incorrect.
```

## Próba wyłączenia CBT

[KB5021989: Extended Protection for Authentication](https://support.microsoft.com/en-us/servicing/os/windows/2022/11/kb5021989-extended-protection-for-authentication?utm_source=gemini)
[Authentication failure from non-Windows NTLM or Kerberos servers](https://learn.microsoft.com/en-us/troubleshoot/windows-server/windows-security/authentication-fails-non-windows-ntlm-kerberos-server?utm_source=gemini)
[TlsFeaturesObserve.csproj](https://github.com/dotnet/aspnetcore/blob/main/src/Servers/HttpSys/samples/TlsFeaturesObserve/TlsFeaturesObserve.csproj)
[ExtendedProtectionPolicy Class](https://learn.microsoft.com/en-us/dotnet/api/system.security.authentication.extendedprotection.extendedprotectionpolicy?view=net-10.0)
[Extended Protection for Authentication](https://www.microsoft.com/en-us/msrc/blog/2009/12/extended-protection-for-authentication)

[STATUS_BAD_BINDINGS](https://efmsoft.com/what-is/amp/?code=0xC000035B)
[LDAP Signing and Channel Binding](https://www.decryptiondigest.com/blog/ldap-signing-channel-binding-hardening-guide)
[Hardening LDAP](https://www.mcbsys.com/blog/2020/07/hardening-ldap-in-server-2016-essentials/)
przeczytać to! [(NTLM) Authentication Protocol](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-nlmp/b38c36ed-2804-4868-a9ff-8dd3182128e4)

sprawdzić `netsh trace start`

`sp HKLM:\System\CurrentControlSet\Control\LSA SuppressExtendedProtection 3`
`HKLM\SYSTEM\CurrentControlSet\Services\HTTP\Parameters`

Metoda A: Identyczny certyfikat TLS na Proxy i Backendzie


# Słowa kluczowe
- ExtendedProtectionPolicy
- channel binding token (CBT)
- RPC (Netlogon Secure Channel)
- SMB ([MS-SMB2] własnościowy protokół Micorosoft dostępny w Open Specifications)
- Mimikatz
- LSASS
- Netlogon
- SSPI
- RFC 5056, RFC 5929 rfc7235
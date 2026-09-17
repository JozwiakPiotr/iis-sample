# Spis treści

- [vm.md](doc/vm.md)
- [ad.md](doc/ad.md)
- [cicd.md](doc/cicd.md)

# Aktualny problem
`przeglądarka > NTLM > frontend > ocelot > backend` wyskakuje okienko logowania pomimo że ustawiłem `--auth-server-allowlist="*.contoso.lab"` i pomimo wpisania poprawnych poświadczeń dostaje 401. Takie zachowanie nie występuje na produkcji gdzie nie ma zrobionych aktualizacji.

# TODO
## WWW-Authenticate
NTLM po prostu implementuje uwierzytelnianie HTTP typu challenge-response, Microsoft dał go do open protocols [`[MS-NTHT]`](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-ntht/395fbc77-deed-4eb4-9d76-d2c82a6e22f8)

copilot pisze > NTLM w HTTP jest stanowy i powiązany z fizycznym połączeniem TCP (ang. connection-oriented authentication). Po poprawnym uwierzytelnieniu całe gniazdo TCP jest traktowane jako zautoryzowane, co rodziło ogromne problemy przy proxy, multiplexingu połączeń, czy w nowszych wersjach protokołu (HTTP/2 i HTTP/3).

- [ ] Zrozumieć uwierzytelnianie HTTP [developer.mozilla](https://developer.mozilla.org/en-US/docs/Web/HTTP/Guides/Authentication#authentication_schemes), [RFC](https://datatracker.ietf.org/doc/html/rfc7235)
- [ ] utworzyć apache z uwierzytelnianiem digest
- [ ] przeanalizować ruch wireshark + mitmproxy, trzeba by dodać cert i https do serwera
- [ ] przeczytać NTLM [`[MS-NTHT]`](https://learn.microsoft.com/en-us/openspecs/windows_protocols/ms-ntht/395fbc77-deed-4eb4-9d76-d2c82a6e22f8)
- [ ] przeczytać o NTLM w [dokumentacji windows server](https://learn.microsoft.com/en-us/troubleshoot/windows-server/windows-security/authentication-package-listed-as-ntlmv1-security-audit-event)
- [ ] co to Extended Protection for Authentication? [link1](https://support.microsoft.com/en-us/servicing/os/windows/2022/11/kb5021989-extended-protection-for-authentication) [link2](https://learn.microsoft.com/en-us/security-updates/SecurityAdvisories/2009/973811)
- [ ] spróbować [`HttpAuthenticationHardeningLevel.Legacy`](https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/breaking-changes/11/httpsys-channel-binding-token-enabled.md)


## Cele
- [ ] zapoznać się z aktualizacjami CVE
- [ ] narysować sytuacje w diagramie (NTLM, Kerberos, LDAP w kontekście mojego scenariusza)
- [ ] spróbować postawić windows server sprzed aktualizacji
- [ ] przetestować jeszcze uwierzytelnianie po stronie IIS
- [ ] YARP
- [ ] gMSA (Group Managed Service Accounts)
- [ ] bramka uwierzytelniająca zamiast uwierzytelniania każdej usługi z osobna

# Douczyć się
- [ ] [Security Support Provider Interface](https://learn.microsoft.com/en-us/windows-server/security/windows-authentication/security-support-provider-interface-architecture)
- [ ] [Microsoft Edge identity support and configuration](https://learn.microsoft.com/en-us/deployedge/microsoft-edge-security-identity) 
- [ ] [Integrated Windows Authentication](https://specopssoft.com/blog/configuring-chrome-and-firefox-for-windows-integrated-authentication/)

- [ ] logi kerberos jeśl się da na API01
- [ ] uruchomić mitmproxy i podejrzeć rozmowę przeglądarka z PC01 z API01

# Done 
## Troubleshooting
- [x] przejrzeć logi przeglądarki
    logi mało mówią, chyba że nie potrafię włączyć odpowiedniego poziomu logowania
## Inicjalizacja
- [x] zainstalować runtime asp.net
- [x] na razie zrobić self signed cert (dla API01 i WEB01) i skopiować je przez udziały SMB (+ konfiguracja firewall dla SMB)
- [x] skonfigurować DNS backend, frontend
### backend
- [x] zwykle konto domenowe
- [x] utworzyć usługę
- [x] skonfigurować httpsys przez netsh
- [x] skonfigurować firewall
### frontend
- [x] skonfigurować IIS

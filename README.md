# Spis treści

- [vm.md](doc/vm.md)
- [ad.md](doc/ad.md)
- [cicd.md](doc/cicd.md)

# Aktualny problem
url `https://backend.contoso.lab:5001/api/users`
dla przeglądarki backend zwaraca challenge (popup logowania)
dla irm $url -UseDefaultCredentials działa bez problemu
podejrzewam że przeglądarka nie dołącza poświadczeń

# TODO
- [ ] [Security Support Provider Interface](https://learn.microsoft.com/en-us/windows-server/security/windows-authentication/security-support-provider-interface-architecture)
- [ ] [Microsoft Edge identity support and configuration](https://learn.microsoft.com/en-us/deployedge/microsoft-edge-security-identity) 
- [ ] [Integrated Windows Authentication](https://specopssoft.com/blog/configuring-chrome-and-firefox-for-windows-integrated-authentication/)
- [ ] przejrzeć logi przeglądarki
- [ ] logi kerberos jeśl się da na API01
- [ ] uruchomić mitmproxy i podejrzeć rozmowę przeglądarka z PC01 z API01

# Done 
- [x] zainstalować runtime asp.net
- [x] zmienić w frontend żeby uderzało na backend.contoso.lab i co ważne na port 5001 zamiast 443
- [x] na razie zrobić self signed cert (dla API01 i WEB01) i skopiować je przez udziały SMB (+ konfiguracja firewall dla SMB)
- [x] skonfigurować DNS backend, frontend
## backend
- [x] zwykle konto domenowe
- [x] utworzyć usługę
- [x] skonfigurować httpsys przez netsh
- [x] skonfigurować firewall
## frontend
- [x] skonfigurować IIS

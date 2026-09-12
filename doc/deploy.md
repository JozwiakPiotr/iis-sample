# Wdrażanie aplikacji na windows server
Wymagany wcześniej skonfigurowany serwer ssh
`~/.ssh/config`
```
Host 192.168.101.*
    IdentitiesOnly yes
    PubkeyAuthentication no
```
W menadżerze plików wejśc na `sftp://Administrator@192.168.101.3/C:/` i skopiować
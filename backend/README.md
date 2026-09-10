Backend deployment (Windows Service + HttpSys)

Cel:
- host: https://backend.contoso.pl
- endpoint: /api/users (GET/POST/DELETE)

1. Publish:
   cd backend
   dotnet restore
   dotnet publish -c Release -o C:\deploy\backend

2. DNS i certyfikat:
   - Rekord DNS: backend.contoso.pl -> IP serwera backend
   - Certyfikat TLS w LocalMachine\My
   - Pobierz thumbprint certyfikatu (bez spacji)

3. Rezerwacja URL i binding certyfikatu dla HttpSys (PowerShell/CMD jako Administrator):
   netsh http add urlacl url=https://backend.contoso.pl:443/ user="NT AUTHORITY\NETWORK SERVICE"
   netsh http add sslcert hostnameport=backend.contoso.pl:443 certhash=<THUMBPRINT> certstorename=MY appid="{5b4f7c21-f2a1-4a56-9f77-9f9f31f7a001}"

4. Instalacja usługi Windows:
   sc.exe create BackendService binPath= "C:\deploy\backend\BackendService.exe" start= auto
   sc.exe start BackendService

5. Update deployment:
   sc.exe stop BackendService
   dotnet publish -c Release -o C:\deploy\backend
   sc.exe start BackendService

6. Rollback:
   - trzymaj poprzedni katalog publish, np. C:\deploy\backend_prev
   - stop service
   - podmień katalog na poprzednią wersję
   - start service

API endpoints:
- GET /api/users
- POST /api/users body: { "name": "Jan" }
- DELETE /api/users/{id}

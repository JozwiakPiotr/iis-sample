Frontend deployment (IIS + ASP.NET Core + Ocelot)

Cel:
- host: https://frontend.contoso.pl
- reverse proxy: https://frontend.contoso.pl/api/users -> https://backend.contoso.pl/api/users

1. Build frontend static assets (React):
   cd frontend/ClientApp
   npm ci
   npm run build

2. Publish ASP.NET Core host (FrontendApp):
   cd ../FrontendApp
   dotnet restore
   dotnet publish -c Release -o C:\deploy\frontend

3. IIS prerequisites (server):
   - Zainstalowany Hosting Bundle dla .NET 8
   - Włączony moduł IIS: ASP.NET Core Module V2 (instalowany z Hosting Bundle)
   - Rekord DNS: frontend.contoso.pl -> IP serwera IIS
   - Certyfikat TLS przypięty do bindingu https dla frontend.contoso.pl

4. Konfiguracja IIS site:
   - Site name: frontend
   - Physical path: C:\deploy\frontend
   - Binding: https, host: frontend.contoso.pl, port: 443, właściwy certyfikat
   - App Pool: No Managed Code, Integrated

5. Ocelot routing:
   - Plik produkcyjny: frontend/FrontendApp/ocelot.Production.json
   - Upstream: /api/{everything}
   - Downstream host: backend.contoso.pl:443 (https)

   W środowisku Development używany jest plik `ocelot.Development.json`,
   który kieruje ruch do `http://localhost:5001`.

6. Update deployment:
   - zbuduj ponownie ClientApp (npm run build)
   - dotnet publish -c Release -o C:\deploy\frontend
   - podmień pliki w C:\deploy\frontend
   - wykonaj recycle app pool lub restart site

Diagnostyka:
- Logi aplikacji ASP.NET Core: stdout logs (po włączeniu w web.config)
- Event Viewer: Windows Logs/Application (IIS AspNetCoreModuleV2)

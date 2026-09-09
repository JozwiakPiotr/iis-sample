Projekt "frontend" i "backend".

Struktura:
- backend/BackendService - ASP.NET Core WebAPI hostowany na HttpSys jako Windows Service
- frontend/FrontendApp - ASP.NET Core hostujący statyczne pliki React w wwwroot i Ocelot jako reverse proxy
- frontend/ClientApp - źródła React + vite

Szybkie uruchomienie deweloperskie (Windows):
1) Backend:
   cd backend
   dotnet new sln -n backend
   dotnet sln add BackendService/BackendService.csproj
   dotnet restore BackendService/BackendService.csproj
   dotnet run --project BackendService/BackendService.csproj

2) Frontend:
   cd frontend/ClientApp
   npm install
   npm run build
   cd ..
   dotnet new sln -n frontend
   dotnet sln add FrontendApp/FrontendApp.csproj
   dotnet run --project FrontendApp/FrontendApp.csproj

IIS: utworzyć site frontend.contoso.pl wskazujący do opublikowanego FrontendApp (wwwroot). Backend publikować i hostować jako Windows Service. Ocelot w FrontendApp przekierowuje /api/* do backend.contoso.pl.

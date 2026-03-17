# Budget Tracker

Een full-stack webapplicatie om persoonlijke inkomsten en uitgaven bij te houden.

![Dashboard](BudgetTracker.API/wwwroot/screenshot.png)

## Technologieën

- **Backend:** ASP.NET Core 10 (C#)
- **Database:** SQL Server
- **Authenticatie:** JWT tokens
- **Frontend:** HTML, CSS, JavaScript, Chart.js
- **Testing:** xUnit, Entity Framework InMemory
- **Documentatie:** Scalar (OpenAPI)
- **Containerisatie:** Docker

## Features

- Registreren en inloggen met JWT authenticatie
- Inkomsten en uitgaven bijhouden per categorie
- Maandoverzicht met grafieken
- Transacties filteren per maand
- Volledige CRUD voor transacties en categorieën

## Lokaal opstarten

### Vereisten
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Stappen

1. Clone de repository
```bash
   git clone https://github.com/s1211660/budget-tracker.git
   cd budget-tracker
```

2. Pas de connection string aan in `BudgetTracker.API/appsettings.json`

3. Start de applicatie
```bash
   cd BudgetTracker.API
   dotnet run
```

4. Ga naar `https://localhost:7298`

## Opstarten met Docker
```bash
docker build -t budget-tracker .
docker run -p 8080:8080 budget-tracker
```

## API Endpoints

| Method | Endpoint | Omschrijving |
|--------|----------|-------------|
| POST | /api/auth/register | Registreren |
| POST | /api/auth/login | Inloggen |
| GET | /api/categories | Categorieën ophalen |
| POST | /api/categories | Categorie aanmaken |
| GET | /api/transactions | Transacties ophalen |
| POST | /api/transactions | Transactie aanmaken |
| DELETE | /api/transactions/{id} | Transactie verwijderen |
| GET | /api/summary | Maandoverzicht |

Volledige documentatie beschikbaar via `/scalar/v1`

## Tests uitvoeren
```bash
cd BudgetTracker.Tests
dotnet test
```

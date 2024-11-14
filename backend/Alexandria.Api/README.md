# API Setup
---
### Setup API
1. Create DockerEnv folder in `backend` root
2. Create two files within DockerEnv: `Api.env` and `MySql.env` 
   - Api.Env content:
   ```
   ConnectionStrings__AppDbContext=[Connection string]
   ```
   - MySql.Env content:
   ```
   MYSQL_PASSWORD=[Secure Password]
   MYSQL_ROOT_PASSWORD=[Secure Password]
   ```
3. in `appsettings.Development.json` set the following connection string:
   ```json
   {
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning"
      }
    },
    "ConnectionStrings": {
      "AppDbContext": "[MySql string]"
    }
   }
   ```
### Start Containers and Setup Database
1. CD into `Alexandria.Api`
2. Run `docker compose up`
3. Run `dotnet ef database update`
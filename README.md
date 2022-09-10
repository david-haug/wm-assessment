# wm-assessment-api

1. Run sql install scripts found in project WM.Assessment.Infrastucture under SqlDataAccess > install_scripts.sql
2. Add connection string for WMA_CONNECTION to appsettings.json in WM.Assessment.Api:<br>
  ```
  "ConnectionStrings": {
    "WMA_CONNECTION": "Server={serverName};Database={databaseName};User Id={username};Password={password};"
  }
  ```

3. Swagger documentation available at {your_app_url}/swagger

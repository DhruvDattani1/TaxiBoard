
# 🗂 Project Plan

## **Phase 1 – Data & Database Setup (Updated)**

1. **Retrieve Taxi Data**
   * Download 1 month of NYC Taxi data (`yellow_tripdata_2025-01.parquet`) and the `taxi_zone_lookup.csv` file from the TLC CloudFront dataset.  
   * Keep both files under the `data/` folder for local reproducibility (< 1–2 GB recommended for dev).

2. **Transform Data & Load**
   * Run the Python ETL script `parquet_2_csv_load.py`:
     * Reads the Parquet file → cleans and casts columns (`VendorID`, `RatecodeID`, `payment_type`, etc.).
     * Converts the dataset to a CSV for bulk import.
     * Connects to PostgreSQL using `psycopg2`.
     * Creates and populates lookup tables: `vendors`, `rate_codes`, `payment_types`, and `taxi_zones` (lookup auto-loaded from CSV).
     * Creates the main `yellow_tripdata` fact table and loads all rows via `COPY`.
     * Adds foreign key constraints for data integrity.

3. **Setup Postgres**
   * Install PostgreSQL locally (or via Docker).  
   * Create the database and user:
     ```sql
     CREATE DATABASE nyc_yellow_taxi_db;
     CREATE USER yellow_taxi_user WITH PASSWORD 'yellow_taxi_user';
     GRANT ALL PRIVILEGES ON DATABASE nyc_yellow_taxi_db TO yellow_taxi_user;
     GRANT USAGE, CREATE ON SCHEMA public TO yellow_taxi_user;
     ALTER SCHEMA public OWNER TO yellow_taxi_user;
     ```
   * Update `pg_hba.conf` to use `md5` authentication and restart Postgres after changes.

4. **Schema**
   * Schema matches the official NYC TLC model (`DB_Notes.md`):  
     * Lookup tables → `vendors`, `rate_codes`, `payment_types`, `taxi_zones`.  
     * Fact table → `yellow_tripdata` with timestamps, numeric types, and foreign keys linking to lookups.  
     * Add indexes on `pickup_datetime` or `RatecodeID` as needed for query speed.

5. **Data Exploration**
   * Verify successful load and run initial queries:
     ```sql
     SELECT COUNT(*) FROM yellow_tripdata;
     SELECT AVG(trip_distance), AVG(total_amount) FROM yellow_tripdata;
     SELECT DATE(tpep_pickup_datetime), COUNT(*) 
     FROM yellow_tripdata 
     GROUP BY 1 ORDER BY 1;
     ```
   * **Deliverable:** PostgreSQL database populated with real NYC Yellow Taxi data, lookup tables linked via foreign keys, and sample queries validated.


---

## **Phase 2 – Backend API (ASP.NET Core)**

6. **Environment Setup**

   * Install .NET SDK (latest LTS (9.0)).
   * Install Entity Framework Core CLI (ORM Tooling)
   * Install EF Core, Npgsql (Postgres provider).
   * Install Utility Packages (Swagger, Memory Cache, Serilog, DotNetEnv) 
   * Create `.env` file and set environment variables 
   * Create `appsettings.Development.json` for local configuration
   * add to .gitignore

7. **Scaffold Project**

   * `dotnet new webapi -o TaxiApi`.
   * Navigate into project directory and run the app 
   * Add `.env` and `appsettings.Development.local.json` configuration files 
   * Update `Program.cs` to load environment variables using DotNetEnv 
   * Verify API runs on localhost

8. **Entity Framework Core Setup**

   * Create `TaxiBoardContext` class in the `/Data` directory.  
   * Register `TaxiContext` in `Program.cs` using `builder.Services.AddDbContext<TaxiContext>()`.   
   * Confirm EF Core connects successfully before adding models. And can query the DB

9. **Models**

   * Define one C# model per table under `/Models` — e.g., `Vendor`, `RateCode`, `PaymentType`, `TaxiZone`, and `YellowTripData`.  
   * Use **attribute-based mapping** (`[Key]`, `[ForeignKey]`, `[Required]`, `[StringLength]`) to mirror the Postgres schema and enforce validation.  
   * Map PostgreSQL column types directly to C# types (`integer → int`, `timestamp → DateTime`, `numeric → decimal`, etc.).  
   * Keep properties non-nullable when the dataset uses special values like `99` or `0` for “unknown,” so the models reflect the raw data accurately.  
   * Add a **surrogate primary key** (e.g., `Id`) to `YellowTripData` so EF Core can uniquely track each record.  
   * All models share the same **DbContext** (`TaxiBoardContext` in `/Data`) and are exposed through `DbSet<>` properties.  
   * Define foreign keys through **navigation properties**; EF Core reads these to build joins automatically.  
   * Include an empty or extended **`OnModelCreating(ModelBuilder)`** method in the context to fine-tune relationships (`HasOne`, `WithMany`, etc.) beyond attributes.  
   * Run `dotnet ef migrations add InitTaxiModels` to generate the initial migration and confirm EF Core recognizes the schema (skip `database update` if the DB already exists).  

10. **Controllers**

* Implement domain-specific controllers aligned with planned frontend sections: `TripsController`, `ZonesController`, `PaymentTypesController`, `VendorsController`, and `AnalyticsController` (planned).
* Use LINQ queries on `TaxiBoardContext` for efficient joins, projections, and filtering.
* Map query results to DTOs to define clear, decoupled response shapes.
* Apply pagination and filtering logic where relevant (e.g., date range, zone, passenger count).
* Inject the DbContext through constructor-based dependency injection for testability.
* Return lightweight, stateless JSON responses that follow REST conventions.
* Keep controllers cohesive — one per functional dataset or page, not per database table.
* Verify and explore all endpoints through Swagger to ensure correct routing and response structure.
 


11. **Serialization & Format**

   * Configure JSON serialization (camelCase, date formatting).  
   * Add `[Produces("application/json")]` to controllers if needed.  
   * Ensure all endpoints follow REST conventions (nouns, stateless, proper status codes).

12. **Documentation & Testing**

   * Enable Swagger/OpenAPI (`Swashbuckle.AspNetCore`) and verify endpoints in the browser (`/swagger`).  
   * Use Postman or curl for endpoint validation and filter testing.  
   * Optionally add unit tests later with `xUnit` or `NUnit` to verify controller logic.


---

## **Phase 3 – Frontend (Vue.js)**

13. **Setup Vue**

    * `npm create vite@latest taxi-ui --template vue`.
    * Install Axios, Chart.js (or Recharts alternative for Vue).
14. **Components**

    * Table component: trips list with pagination.
    * Chart component: trips per day (bar chart).
15. **API Integration**

    * Axios calls to `/api/trips`.
    * Handle filters (date range, passenger count).
16. **Basic Styling**

    * Use Tailwind or Vuetify for clean look.
17. **Testing**

    * Run UI + API locally together.
    * Deliverable: UI fetches & displays live API data.

---

## **Phase 4 – Containerization**

18. **Dockerize Backend**

    * Dockerfile: build + run ASP.NET Core API.
19. **Dockerize Frontend**

    * Dockerfile: build Vue app → serve with Nginx.
20. **Compose**

    * `docker-compose.yml`:

      * Postgres (volume + env vars).
      * Backend (depends_on Postgres).
      * Frontend (depends_on Backend).
21. **Verify**

    * `docker-compose up` should spin up full stack.
    * Deliverable: 3-container local app.

---

## **Phase 5 – Deployment (Azure)**

22. **Azure Registry**

    * Push frontend + backend images to ACR.
23. **Azure Database**

    * Provision PostgreSQL Flexible Server.
    * Import schema & data (dump from local).
24. **Deployment**

    * Deploy API + frontend to Azure App Service or Container Apps.
    * Configure env vars (DB connection strings).
25. **CI/CD**

    * Setup Azure Pipeline (YAML) to build + push + deploy on git push.
    * Deliverable: Publicly accessible app on Azure.

---

## **Phase 6 – Extras (Optional Polish)**

26. **Auth**

    * Add JWT or Azure AD for protected routes.
27. **Performance**

    * Add Redis cache for common queries.
    * Enable ASP.NET response caching.
28. **Monitoring**

    * Add health check endpoints (`/health`).
    * Log with Serilog → Grafana dashboards.
29. **E2E Tests**

    * Cypress/Playwright tests that hit UI + API.
30. **Docs**

    * Write README: setup, architecture, screenshots.
    * Deliverable: Production-ready portfolio project.

---


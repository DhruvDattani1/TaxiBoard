# Retrieve parquet file
curl -o yellow_tripdata_2025-01.parquet https://d37ci6vzurychx.cloudfront.net/trip-data/yellow_tripdata_2025-01.parquet

curl -o taxi_zone_lookup.csv https://d37ci6vzurychx.cloudfront.net/misc/taxi_zone_lookup.csv

# Setup Postgres DB
sudo -u postgres psql
CREATE DATABASE nyc_yellow_taxi_db;
CREATE USER yellow_taxi_user WITH PASSWORD 'yellow_taxi_user';
\c nyc_yellow_taxi_db
GRANT ALL PRIVILEGES ON DATABASE nyc_yellow_taxi_db TO yellow_taxi_user;
GRANT USAGE, CREATE ON SCHEMA public TO yellow_taxi_user;
ALTER SCHEMA public OWNER TO yellow_taxi_user;
\q

# Modify pg_hba.conf to use password auth instead of ident
sudo nano /var/lib/pgsql/data/pg_hba.conf
# Change lines with "ident" to "md5":
# host    all    all    127.0.0.1/32    md5
# host    all    all    ::1/128         md5

# Restart PostgreSQL
sudo systemctl restart postgresql

# Run the script (from project root or scripts folder)
cd ~/Projects/Taxi_Data_API/scripts
python parquet_2_csv_load.py

# In case you need to reset/drop the table
sudo -u postgres psql -d nyc_yellow_taxi_db -c "DROP TABLE IF EXISTS yellow_tripdata;"





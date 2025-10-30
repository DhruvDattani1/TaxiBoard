import pandas as pd
import psycopg2
from pathlib import Path

DB_NAME = "nyc_yellow_taxi_db"
DB_USER = "yellow_taxi_user"
DB_PASSWORD = "yellow_taxi_user"
DB_HOST = "localhost"
DB_PORT = "5432"
# I am setting this up locally that is why my password is here. When this project is hosted on AZURE my creds will not be exposed

base_dir = Path(__file__).resolve().parent.parent.parent
parquet_path = base_dir / "data" / "yellow_tripdata_2025-01.parquet"
csv_path = base_dir / "data" / "yellow_tripdata_2025-01.csv"
lookup_csv_path = base_dir / "data" / "taxi_zone_lookup.csv"

print(f"Reading {parquet_path} ...")
df = pd.read_parquet(parquet_path)

float_int_columns = ["VendorID", "passenger_count", "RatecodeID", "payment_type", "PULocationID", "DOLocationID"]
for col in float_int_columns:
    if col in df.columns:
        if col == "VendorID":
            df[col] = df[col].astype("Int64")
        elif col == "RatecodeID":
            df[col] = df[col].fillna(99).astype(int)
        elif col == "payment_type":
            df[col] = df[col].fillna(5).astype(int)
        elif col in ["PULocationID", "DOLocationID"]:
            df[col] = df[col].astype("Int64")
        elif col == "passenger_count":
            df[col] = df[col].fillna(1).astype(int)




print(f"Writing CSV to {csv_path} ...")
df.to_csv(csv_path, index=False)

print("Conversion complete!\n")

print("Connecting to postgres\n")

conn = psycopg2.connect(
    dbname=DB_NAME,
    user=DB_USER,
    password=DB_PASSWORD,
    host=DB_HOST,
    port=DB_PORT
)
conn.autocommit = True
cur = conn.cursor()
print("Connected to Postgres\n")

print("Creating vendors table...")
cur.execute("""
CREATE TABLE IF NOT EXISTS vendors (
    vendor_id INTEGER PRIMARY KEY,
    vendor_name VARCHAR(100)
);
""")

cur.execute("SELECT COUNT(*) FROM vendors")
if cur.fetchone()[0] == 0:
    cur.execute("""
    INSERT INTO vendors VALUES
    (1, 'Creative Mobile Technologies, LLC'),
    (2, 'Curb Mobility, LLC'),
    (6, 'Myle Technologies Inc'),
    (7, 'Helix');
    """)
    print("Vendors table populated\n")
else:
    print("Vendors table already populated\n")

print("Creating rate_codes table...")
cur.execute("""
CREATE TABLE IF NOT EXISTS rate_codes (
    rate_code_id INTEGER PRIMARY KEY,
    rate_description VARCHAR(50)
);
""")

cur.execute("SELECT COUNT(*) FROM rate_codes")
if cur.fetchone()[0] == 0:
    cur.execute("""
    INSERT INTO rate_codes VALUES
    (1, 'Standard rate'),
    (2, 'JFK'),
    (3, 'Newark'),
    (4, 'Nassau or Westchester'),
    (5, 'Negotiated fare'),
    (6, 'Group ride'),
    (99, 'Null/unknown');
    """)
    print("Rate codes table populated\n")
else:
    print("Rate codes table already populated\n")

print("Creating payment_types table...")
cur.execute("""
CREATE TABLE IF NOT EXISTS payment_types (
    payment_type_id INTEGER PRIMARY KEY,
    payment_description VARCHAR(50)
);
""")

cur.execute("SELECT COUNT(*) FROM payment_types")
if cur.fetchone()[0] == 0:
    cur.execute("""
    INSERT INTO payment_types VALUES
    (0, 'Flex Fare trip'),
    (1, 'Credit card'),
    (2, 'Cash'),
    (3, 'No charge'),
    (4, 'Dispute'),
    (5, 'Unknown'),
    (6, 'Voided trip');
    """)
    print("Payment types table populated\n")
else:
    print("Payment types table already populated\n")


print("Creating taxi_zones table...")
cur.execute("""
CREATE TABLE IF NOT EXISTS taxi_zones (
    LocationID INTEGER PRIMARY KEY,
    Borough VARCHAR(50),
    Zone VARCHAR(100),
    service_zone VARCHAR(50)
);
""")


cur.execute("SELECT COUNT(*) FROM taxi_zones")
if cur.fetchone()[0] == 0:
    print(f"Loading taxi zone lookup from {lookup_csv_path}...")
    with open(lookup_csv_path, "r", encoding="utf-8") as f:
        cur.copy_expert(
            "COPY taxi_zones FROM STDIN WITH CSV HEADER",
            f
        )
    cur.execute("SELECT COUNT(*) FROM taxi_zones")
    zone_count = cur.fetchone()[0]
    print(f"Taxi zones loaded: {zone_count:,}\n")
else:
    print("Taxi zones table already populated\n")

print("Creating yellow_tripdata table...")
table_schema = """
CREATE TABLE IF NOT EXISTS yellow_tripdata (
    VendorID INTEGER,
    tpep_pickup_datetime TIMESTAMP,
    tpep_dropoff_datetime TIMESTAMP,
    passenger_count INTEGER,
    trip_distance NUMERIC(10,2),
    RatecodeID INTEGER, 
    store_and_fwd_flag CHAR(1),
    PULocationID INTEGER,
    DOLocationID INTEGER,
    payment_type INTEGER,
    fare_amount NUMERIC(10,2),
    extra NUMERIC(10,2),
    mta_tax NUMERIC(10,2),
    tip_amount NUMERIC(10,2),
    tolls_amount NUMERIC(10,2),
    improvement_surcharge NUMERIC(10,2),
    total_amount NUMERIC(10,2),
    congestion_surcharge NUMERIC(10,2),
    Airport_fee NUMERIC(10,2),
    cbd_congestion_fee NUMERIC(10,2)
);
"""

cur.execute(table_schema)
print("Yellow tripdata table created\n")

print("Loading trip data CSV into postgres...")
with open(csv_path, "r", encoding="utf-8") as f:
    cur.copy_expert(
        "COPY yellow_tripdata FROM STDIN WITH CSV HEADER",
        f
    )


cur.execute("SELECT COUNT(*) FROM yellow_tripdata")
count = cur.fetchone()[0]
print(f"Trip data loaded successfully! Total rows: {count:,}\n")

print("Adding foreign key constraints...")

cur.execute("""
SELECT 1 FROM information_schema.table_constraints 
WHERE constraint_name = 'fk_vendor' AND table_name = 'yellow_tripdata'
""")
if not cur.fetchone():
    cur.execute("""
    ALTER TABLE yellow_tripdata 
    ADD CONSTRAINT fk_vendor 
    FOREIGN KEY (VendorID) REFERENCES vendors(vendor_id);
    """)
    print("✓ Vendor foreign key added")
else:
    print("✓ Vendor foreign key already exists")

cur.execute("""
SELECT 1 FROM information_schema.table_constraints 
WHERE constraint_name = 'fk_rate_code' AND table_name = 'yellow_tripdata'
""")
if not cur.fetchone():
    cur.execute("""
    ALTER TABLE yellow_tripdata 
    ADD CONSTRAINT fk_rate_code 
    FOREIGN KEY (RatecodeID) REFERENCES rate_codes(rate_code_id);
    """)
    print("✓ Rate code foreign key added")
else:
    print("✓ Rate code foreign key already exists")

cur.execute("""
SELECT 1 FROM information_schema.table_constraints 
WHERE constraint_name = 'fk_payment_type' AND table_name = 'yellow_tripdata'
""")
if not cur.fetchone():
    cur.execute("""
    ALTER TABLE yellow_tripdata 
    ADD CONSTRAINT fk_payment_type 
    FOREIGN KEY (payment_type) REFERENCES payment_types(payment_type_id);
    """)
    print("✓ Payment type foreign key added")
else:
    print("✓ Payment type foreign key already exists")

cur.execute("""
SELECT 1 FROM information_schema.table_constraints 
WHERE constraint_name = 'fk_pickup_location' AND table_name = 'yellow_tripdata'
""")
if not cur.fetchone():
    cur.execute("""
    ALTER TABLE yellow_tripdata 
    ADD CONSTRAINT fk_pickup_location 
    FOREIGN KEY (PULocationID) REFERENCES taxi_zones(LocationID);
    """)
    print("✓ Pickup location foreign key added")
else:
    print("✓ Pickup location foreign key already exists")

cur.execute("""
SELECT 1 FROM information_schema.table_constraints 
WHERE constraint_name = 'fk_dropoff_location' AND table_name = 'yellow_tripdata'
""")
if not cur.fetchone():
    cur.execute("""
    ALTER TABLE yellow_tripdata 
    ADD CONSTRAINT fk_dropoff_location 
    FOREIGN KEY (DOLocationID) REFERENCES taxi_zones(LocationID);
    """)
    print("✓ Dropoff location foreign key added\n")
else:
    print("✓ Dropoff location foreign key already exists\n")

print("All foreign keys configured!")

cur.close()
conn.close()

print("Connection terminated")
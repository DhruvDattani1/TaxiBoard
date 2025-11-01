# 🗂️ NYC Yellow Taxi Database Schema

This schema models the official NYC Taxi & Limousine Commission (TLC) yellow trip dataset.  
It’s normalized into lookup (dimension) tables and a central fact table (`yellow_tripdata`).

---

## 1. vendors

**Purpose:** Lookup table for taxi service providers.

| Column | Type | Description |
|---------|------|-------------|
| `vendor_id` | `INTEGER PRIMARY KEY` | Unique ID for each taxi vendor. |
| `vendor_name` | `VARCHAR(100)` | Company name (e.g. CMT, Curb Mobility). |

---

## 2. rate_codes

**Purpose:** Describes the rate code used for the trip.

| Column | Type | Description |
|---------|------|-------------|
| `rate_code_id` | `INTEGER PRIMARY KEY` | Rate code on the meter. |
| `rate_description` | `VARCHAR(50)` | Meaning of the rate code (Standard, JFK, etc.). |

---

## 3. payment_types

**Purpose:** Identifies the method of payment used for a trip.

| Column | Type | Description |
|---------|------|-------------|
| `payment_type_id` | `INTEGER PRIMARY KEY` | Payment method ID. |
| `payment_description` | `VARCHAR(50)` | e.g., Credit card, Cash, Dispute, Unknown. |

---

## 4. taxi_zones

**Purpose:** Maps NYC Taxi zone IDs to boroughs and service zones.

| Column | Type | Description |
|---------|------|-------------|
| `LocationID` | `INTEGER PRIMARY KEY` | Unique zone ID. |
| `Borough` | `VARCHAR(50)` | Borough name (e.g. Manhattan, Queens). |
| `Zone` | `VARCHAR(100)` | Neighborhood or taxi zone name. |
| `service_zone` | `VARCHAR(50)` | Classification: Yellow Zone, Boro Zone, Airports, EWR, etc. |

---

## 5. yellow_tripdata

**Purpose:** Fact table containing all individual taxi trip records.

| Column | Type | Description |
|---------|------|-------------|
| `VendorID` | `INTEGER` | FK → `vendors.vendor_id` |
| `tpep_pickup_datetime` | `TIMESTAMP` | Trip start time. |
| `tpep_dropoff_datetime` | `TIMESTAMP` | Trip end time. |
| `passenger_count` | `INTEGER` | Number of passengers. |
| `trip_distance` | `NUMERIC(10,2)` | Distance in miles. |
| `RatecodeID` | `INTEGER` | FK → `rate_codes.rate_code_id` |
| `store_and_fwd_flag` | `CHAR(1)` | 'Y' if trip record was stored before sending. |
| `PULocationID` | `INTEGER` | FK → `taxi_zones.LocationID` (pickup zone). |
| `DOLocationID` | `INTEGER` | FK → `taxi_zones.LocationID` (dropoff zone). |
| `payment_type` | `INTEGER` | FK → `payment_types.payment_type_id` |
| `fare_amount` | `NUMERIC(10,2)` | Base fare amount. |
| `extra` | `NUMERIC(10,2)` | Additional charges (e.g. night surcharge). |
| `mta_tax` | `NUMERIC(10,2)` | $0.50 MTA tax. |
| `tip_amount` | `NUMERIC(10,2)` | Tip paid by passenger. |
| `tolls_amount` | `NUMERIC(10,2)` | Total tolls charged. |
| `improvement_surcharge` | `NUMERIC(10,2)` | $0.30 improvement fee. |
| `total_amount` | `NUMERIC(10,2)` | Total charged to passenger. |
| `congestion_surcharge` | `NUMERIC(10,2)` | NYC congestion fee. |
| `Airport_fee` | `NUMERIC(10,2)` | Airport access fee. |
| `cbd_congestion_fee` | `NUMERIC(10,2)` | Central Business District fee. |

---

## 🔗 Relationships

**Foreign Keys:**
- `yellow_tripdata.VendorID` → `vendors.vendor_id`
- `yellow_tripdata.RatecodeID` → `rate_codes.rate_code_id`
- `yellow_tripdata.payment_type` → `payment_types.payment_type_id`
- `yellow_tripdata.PULocationID` → `taxi_zones.LocationID`
- `yellow_tripdata.DOLocationID` → `taxi_zones.LocationID`

---

### 🧩 Relationship Diagram

          vendors (vendor_id)
                │
                ▼
         yellow_tripdata
                ▲
                │
 rate_codes ────┘
                │
 payment_types ─┘
                │
 taxi_zones ────┬── PULocationID → Pickup
                 └── DOLocationID → Dropoff

---

## Relationships enforced via EF core OnModelCreating method

**Vendor Relationship**
- trip belongs to one vendor
- vendor can have many trips
- VendorId is foreign key

**RateCode Relationship**
- each trip has one rate code
- one rate code can appear in many trips

**PaymentType Relationship**
- each trip has one payment m,ethod
- one  payment method can be used for many trips

**Pickup Zone Relationship**
- Defines the pickup location link between a trip and taxi zone.
- one rate code can appear in many trips
- prevent deleting a zone

**Dropoff Zone Relationship**
- same as pickup


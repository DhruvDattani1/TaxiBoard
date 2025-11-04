### **Controller Overview**

#### **TripsController**

Handles the core trip dataset powering the main data views. Supports pagination, date range filters, and passenger count queries. Joins related tables (zones, vendors, payment types) to return complete trip details through DTOs.

#### **ZonesController**

Provides access to NYC taxi zone data, including borough and service zone information. Useful for populating dropdowns, filters, and contextual analytics on trip origins and destinations.

#### **PaymentTypesController**

Serves static reference data describing available payment methods (e.g., Credit Card, Cash, No Charge). Used to map payment type IDs to human-readable names in frontend visualizations and reports.

#### **VendorsController**

Exposes vendor (taxi provider) metadata such as `vendor_id` and `vendor_name`. Enables breakdowns by provider for operational comparisons, performance summaries, or vendor-level filtering.


/* Reporter Role Access */

IF DATABASE_PRINCIPAL_ID('Reporter') IS NULL
BEGIN
    CREATE ROLE [Reporter]
END

GRANT SELECT ON SCHEMA :: DashboardReporting TO Reporter

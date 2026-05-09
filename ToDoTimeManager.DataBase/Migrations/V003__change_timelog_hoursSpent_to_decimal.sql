-- =============================================================
-- Migration: Change TimeLogs.HoursSpent from TIME to DECIMAL(6,2)
--
-- TIME stores time-of-day (max 23:59:59) which prevents logging
-- more than 24 hours. DECIMAL(6,2) stores hours as a numeric
-- value (e.g. 1.5 = 1h 30m), supporting up to 9999.99 hours.
--
-- WARNING: Existing TIME values are converted to fractional hours.
-- =============================================================

BEGIN TRANSACTION;

ALTER TABLE [dbo].[TimeLogs]
    ADD [HoursSpent_New] DECIMAL(6, 2) NULL;

UPDATE [dbo].[TimeLogs]
SET    [HoursSpent_New] = CAST(DATEDIFF(MINUTE, '00:00:00', [HoursSpent]) AS DECIMAL(6, 2)) / 60;

ALTER TABLE [dbo].[TimeLogs]
    DROP COLUMN [HoursSpent];

EXEC sp_rename 'TimeLogs.HoursSpent_New', 'HoursSpent', 'COLUMN';

ALTER TABLE [dbo].[TimeLogs]
    ALTER COLUMN [HoursSpent] DECIMAL(6, 2) NOT NULL;

COMMIT TRANSACTION;

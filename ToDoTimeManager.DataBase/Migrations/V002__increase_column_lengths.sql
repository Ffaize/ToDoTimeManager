-- =============================================================
-- Migration: Align column lengths with DTO validation constraints
--
-- ToDos.Title:          NVARCHAR(100) → NVARCHAR(200)
-- TimeLogs.LogDescription: NVARCHAR(200) → NVARCHAR(1000)
-- =============================================================

BEGIN TRANSACTION;

ALTER TABLE [dbo].[ToDos]
    ALTER COLUMN [Title] NVARCHAR(200) NULL;

ALTER TABLE [dbo].[TimeLogs]
    ALTER COLUMN [LogDescription] NVARCHAR(1000) NULL;

COMMIT TRANSACTION;

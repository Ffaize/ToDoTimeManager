-- =============================================================
-- Migration: Remap legacy Admin role (INT 1) to new Admin (INT 4)
--
-- Before this migration UserRole column values:
--   0 = User, 1 = Admin
-- After this migration:
--   0 = User, 1 = Developer, 2 = ProjectManager, 3 = Manager, 4 = Admin
--
-- Run BEFORE deploying the new application build.
-- =============================================================

BEGIN TRANSACTION;

UPDATE [dbo].[Users]
SET    UserRole = 4
WHERE  UserRole = 1;

COMMIT TRANSACTION;

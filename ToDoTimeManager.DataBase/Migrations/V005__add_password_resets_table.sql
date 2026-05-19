-- Migration: Add PasswordResets table for forgot-password flow
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'PasswordResets')
BEGIN
    CREATE TABLE [dbo].[PasswordResets]
    (
        Id        UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
        UserId    UNIQUEIDENTIFIER NOT NULL REFERENCES [dbo].[Users] (Id) ON DELETE CASCADE,
        Code      NVARCHAR(128)    NOT NULL,
        ExpiresAt DATETIME         NOT NULL,
        CONSTRAINT [UQ_PasswordResets_UserId] UNIQUE (UserId)
    );
END

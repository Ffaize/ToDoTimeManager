CREATE TABLE [dbo].[UsersSecrets]
(
    Id                    UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID() PRIMARY KEY,
    UserId                UNIQUEIDENTIFIER NOT NULL,
    PasswordSalt          NVARCHAR(128)    NOT NULL,
    RefreshToken          NVARCHAR(512)    NULL,
    RefreshTokenExpiresAt DATETIME         NULL,
    CONSTRAINT [FK_UsersSecrets_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_UsersSecrets_UserId] UNIQUE ([UserId])
)

CREATE TABLE [dbo].[UserTotpSecrets]
(
    UserId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Secret NVARCHAR(64)     NOT NULL,
    CONSTRAINT FK_UserTotpSecrets_Users FOREIGN KEY (UserId)
        REFERENCES [dbo].[Users] (Id) ON DELETE CASCADE
)

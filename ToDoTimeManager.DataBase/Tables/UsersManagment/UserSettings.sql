CREATE TABLE [dbo].[UserSettings]
(
    UserId             UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    IsTwoFactorEnabled BIT              NOT NULL DEFAULT 0,
    TwoFactorMethod    TINYINT          NOT NULL DEFAULT 0,
    CONSTRAINT FK_UserSettings_Users FOREIGN KEY (UserId) REFERENCES [dbo].[Users] (Id) ON DELETE CASCADE
)

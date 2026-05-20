CREATE PROCEDURE [dbo].[sp_Users_Create] @Id UNIQUEIDENTIFIER,
                                         @Username NVARCHAR(255),
                                         @Email NVARCHAR(255),
                                         @Password NVARCHAR(MAX),
                                         @UserRole INT,
                                         @OAuthProvider INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[Users] (Id, Username, Email, Password, UserRole, OAuthProvider)
    VALUES (@Id, @Username, @Email, @Password, @UserRole, @OAuthProvider);

    INSERT INTO [dbo].[UserSettings] (UserId, IsTwoFactorEnabled)
    VALUES (@Id, 0);
END

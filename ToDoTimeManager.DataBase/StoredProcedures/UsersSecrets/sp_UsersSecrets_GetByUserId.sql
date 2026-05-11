CREATE PROCEDURE [dbo].[sp_UsersSecrets_GetByUserId] @UserId UNIQUEIDENTIFIER
AS
    SET NOCOUNT ON;
SELECT Id, UserId, PasswordSalt, RefreshToken, RefreshTokenExpiresAt
FROM [dbo].[UsersSecrets]
WHERE UserId = @UserId;

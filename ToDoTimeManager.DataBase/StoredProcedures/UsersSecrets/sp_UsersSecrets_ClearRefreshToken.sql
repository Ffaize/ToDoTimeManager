CREATE PROCEDURE [dbo].[sp_UsersSecrets_ClearRefreshToken] @UserId UNIQUEIDENTIFIER
AS
UPDATE [dbo].[UsersSecrets]
SET RefreshToken          = NULL,
    RefreshTokenExpiresAt = NULL
WHERE UserId = @UserId;

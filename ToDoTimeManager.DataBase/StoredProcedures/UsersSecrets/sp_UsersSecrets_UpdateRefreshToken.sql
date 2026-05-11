CREATE PROCEDURE [dbo].[sp_UsersSecrets_UpdateRefreshToken] @UserId               UNIQUEIDENTIFIER,
                                                            @RefreshToken          NVARCHAR(512),
                                                            @RefreshTokenExpiresAt DATETIME
AS
UPDATE [dbo].[UsersSecrets]
SET RefreshToken          = @RefreshToken,
    RefreshTokenExpiresAt = @RefreshTokenExpiresAt
WHERE UserId = @UserId;

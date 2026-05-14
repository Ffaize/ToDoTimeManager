CREATE PROCEDURE [dbo].[sp_UsersSecrets_GetPasswordSaltByUserId] @UserId UNIQUEIDENTIFIER
AS
    SET NOCOUNT ON;
SELECT PasswordSalt
FROM [dbo].[UsersSecrets]
WHERE UserId = @UserId;

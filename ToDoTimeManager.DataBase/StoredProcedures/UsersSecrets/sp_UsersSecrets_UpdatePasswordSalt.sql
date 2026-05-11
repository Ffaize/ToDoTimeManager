CREATE PROCEDURE [dbo].[sp_UsersSecrets_UpdatePasswordSalt] @UserId       UNIQUEIDENTIFIER,
                                                            @PasswordSalt NVARCHAR(128)
AS
UPDATE [dbo].[UsersSecrets]
SET PasswordSalt = @PasswordSalt
WHERE UserId = @UserId;

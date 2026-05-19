CREATE PROCEDURE [dbo].[sp_UsersSecrets_UpdatePassword] @UserId       UNIQUEIDENTIFIER,
                                                       @PasswordHash NVARCHAR(MAX),
                                                       @PasswordSalt NVARCHAR(128)
AS
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE [dbo].[Users]
        SET Password = @PasswordHash
        WHERE Id = @UserId;

        UPDATE [dbo].[UsersSecrets]
        SET PasswordSalt = @PasswordSalt
        WHERE UserId = @UserId;
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH

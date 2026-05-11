CREATE PROCEDURE [dbo].[sp_TwoFactorCodes_Upsert] @Id        UNIQUEIDENTIFIER,
                                                  @UserId    UNIQUEIDENTIFIER,
                                                  @Code      NVARCHAR(128),
                                                  @ExpiresAt DATETIME
AS
    BEGIN TRANSACTION;
    BEGIN TRY
        EXEC [dbo].[sp_TwoFactorCodes_DeleteByUserId] @UserId;
        INSERT INTO [dbo].[TwoFactorCodes] (Id, UserId, Code, ExpiresAt)
        VALUES (@Id, @UserId, @Code, @ExpiresAt);
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH

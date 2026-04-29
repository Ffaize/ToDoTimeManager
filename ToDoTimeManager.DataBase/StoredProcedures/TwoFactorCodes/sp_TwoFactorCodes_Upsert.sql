CREATE PROCEDURE [dbo].[sp_TwoFactorCodes_Upsert] @Id        UNIQUEIDENTIFIER,
                                                  @UserId    UNIQUEIDENTIFIER,
                                                  @Code      NVARCHAR(7),
                                                  @ExpiresAt DATETIME
AS
    SET NOCOUNT ON;
    DELETE FROM [dbo].[TwoFactorCodes] WHERE UserId = @UserId;
    INSERT INTO [dbo].[TwoFactorCodes] (Id, UserId, Code, ExpiresAt)
    VALUES (@Id, @UserId, @Code, @ExpiresAt);

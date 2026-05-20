CREATE PROCEDURE [dbo].[sp_UserTotpSecrets_Upsert]
    @UserId UNIQUEIDENTIFIER,
    @Secret NVARCHAR(64)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM [dbo].[UserTotpSecrets] WHERE UserId = @UserId)
        UPDATE [dbo].[UserTotpSecrets]
        SET    Secret = @Secret
        WHERE  UserId = @UserId;
    ELSE
        INSERT INTO [dbo].[UserTotpSecrets] (UserId, Secret)
        VALUES (@UserId, @Secret);
END

CREATE PROCEDURE [dbo].[sp_UserTotpSecrets_GetByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId, Secret
    FROM   [dbo].[UserTotpSecrets]
    WHERE  UserId = @UserId;
END

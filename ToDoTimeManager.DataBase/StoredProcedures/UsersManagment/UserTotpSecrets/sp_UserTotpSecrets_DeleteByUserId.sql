CREATE PROCEDURE [dbo].[sp_UserTotpSecrets_DeleteByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[UserTotpSecrets]
    WHERE UserId = @UserId;
END

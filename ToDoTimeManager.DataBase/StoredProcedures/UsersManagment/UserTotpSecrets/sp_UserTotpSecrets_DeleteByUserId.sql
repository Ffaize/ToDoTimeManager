CREATE PROCEDURE [dbo].[sp_UserTotpSecrets_DeleteByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE FROM [dbo].[UserTotpSecrets]
    WHERE UserId = @UserId;
END

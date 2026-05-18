CREATE PROCEDURE [dbo].[sp_UserSettings_GetByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserId,
           IsTwoFactorEnabled
    FROM [dbo].[UserSettings]
    WHERE UserId = @UserId;
END

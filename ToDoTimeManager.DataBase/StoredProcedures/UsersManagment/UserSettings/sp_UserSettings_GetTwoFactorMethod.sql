CREATE PROCEDURE [dbo].[sp_UserSettings_GetTwoFactorMethod]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TwoFactorMethod
    FROM   [dbo].[UserSettings]
    WHERE  UserId = @UserId;
END

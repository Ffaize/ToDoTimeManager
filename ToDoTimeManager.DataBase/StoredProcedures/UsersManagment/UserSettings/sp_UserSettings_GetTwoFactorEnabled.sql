CREATE PROCEDURE [dbo].[sp_UserSettings_GetTwoFactorEnabled]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IsTwoFactorEnabled
    FROM [dbo].[UserSettings]
    WHERE UserId = @UserId;
END

CREATE PROCEDURE [dbo].[sp_UserSettings_SetTwoFactorMethod]
    @UserId          UNIQUEIDENTIFIER,
    @TwoFactorMethod TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[UserSettings]
    SET    TwoFactorMethod = @TwoFactorMethod
    WHERE  UserId = @UserId;
END

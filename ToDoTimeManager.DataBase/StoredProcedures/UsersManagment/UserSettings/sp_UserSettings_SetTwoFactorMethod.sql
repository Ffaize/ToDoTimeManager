CREATE PROCEDURE [dbo].[sp_UserSettings_SetTwoFactorMethod]
    @UserId          UNIQUEIDENTIFIER,
    @TwoFactorMethod TINYINT
AS
BEGIN
    UPDATE [dbo].[UserSettings]
    SET    TwoFactorMethod = @TwoFactorMethod
    WHERE  UserId = @UserId;
END

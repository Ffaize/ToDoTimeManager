CREATE PROCEDURE [dbo].[sp_UserSettings_Update]
    @UserId             UNIQUEIDENTIFIER,
    @IsTwoFactorEnabled BIT,
    @TwoFactorMethod    TINYINT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[UserSettings]
    SET IsTwoFactorEnabled = @IsTwoFactorEnabled,
        TwoFactorMethod    = @TwoFactorMethod
    WHERE UserId = @UserId;
END

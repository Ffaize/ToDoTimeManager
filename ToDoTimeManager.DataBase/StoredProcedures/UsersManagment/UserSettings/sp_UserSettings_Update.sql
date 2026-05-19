CREATE PROCEDURE [dbo].[sp_UserSettings_Update]
    @UserId            UNIQUEIDENTIFIER,
    @IsTwoFactorEnabled BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[UserSettings]
    SET IsTwoFactorEnabled = @IsTwoFactorEnabled
    WHERE UserId = @UserId;
END

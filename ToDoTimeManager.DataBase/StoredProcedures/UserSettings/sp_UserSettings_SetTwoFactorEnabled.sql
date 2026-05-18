CREATE PROCEDURE [dbo].[sp_UserSettings_SetTwoFactorEnabled]
    @UserId    UNIQUEIDENTIFIER,
    @IsEnabled BIT
AS
BEGIN
    UPDATE [dbo].[UserSettings]
    SET IsTwoFactorEnabled = @IsEnabled
    WHERE UserId = @UserId;
END

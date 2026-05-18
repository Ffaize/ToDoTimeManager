CREATE PROCEDURE [dbo].[sp_UserSettings_SetTwoFactorEnabled]
    @UserId    UNIQUEIDENTIFIER,
    @IsEnabled BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[UserSettings]
    SET IsTwoFactorEnabled = @IsEnabled
    WHERE UserId = @UserId;
END

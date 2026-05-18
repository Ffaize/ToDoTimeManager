CREATE PROCEDURE [dbo].[sp_UserSettings_SetTwoFactorEnabled]
    @UserId    UNIQUEIDENTIFIER,
    @IsEnabled BIT
AS
BEGIN
IF NOT EXISTS(Select * from [dbo].[UserSettings] WHERE UserId = @UserId)
BEGIN 
    INSERT INTO [dbo].[UserSettings] (UserId, IsTwoFactorEnabled)
    VALUES (@UserId, @IsEnabled);
END 
ELSE 
BEGIN
    UPDATE [dbo].[UserSettings]
    SET IsTwoFactorEnabled = @IsEnabled
    WHERE UserId = @UserId;
END 

END

CREATE PROCEDURE [dbo].[sp_Users_UpdateAvatar]
    @Id     UNIQUEIDENTIFIER,
    @Avatar NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Users
    SET Avatar = @Avatar
    WHERE Id = @Id;
END

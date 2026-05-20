CREATE PROCEDURE [dbo].[sp_Users_GetbyUsername] @UserName NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,
           Username,
           Email,
           Password,
           UserRole,
           OAuthProvider
    FROM dbo.Users
    WHERE Username = @UserName;
END

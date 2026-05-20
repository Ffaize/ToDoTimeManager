CREATE PROCEDURE [dbo].[sp_Users_GetAll]
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
    ORDER BY UserName ASC;
END

CREATE PROCEDURE [dbo].[sp_Users_GetByEmail] @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,
           Username,
           Email,
           Password,
           UserRole
    FROM dbo.Users
    WHERE Email = @Email;
END

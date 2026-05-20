CREATE PROCEDURE [dbo].[sp_Users_GetById] @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,
           Username,
           Email,
           Password,
           UserRole,
           OAuthProvider
    FROM [dbo].[Users]
    WHERE Id = @Id;
END

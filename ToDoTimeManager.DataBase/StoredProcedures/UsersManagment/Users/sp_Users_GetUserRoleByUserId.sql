CREATE PROCEDURE [dbo].[sp_Users_GetUserRoleByUserId] @UserId UNIQUEIDENTIFIER
AS
    SET NOCOUNT ON;
SELECT UserRole
FROM [dbo].[Users]
WHERE Id = @UserId;

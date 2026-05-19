CREATE PROCEDURE [dbo].[sp_PasswordResets_GetByUserId] @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Code, ExpiresAt
    FROM [dbo].[PasswordResets]
    WHERE UserId = @UserId;
END

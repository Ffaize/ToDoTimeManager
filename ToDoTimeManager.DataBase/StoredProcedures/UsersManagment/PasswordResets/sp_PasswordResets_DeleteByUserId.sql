CREATE PROCEDURE [dbo].[sp_PasswordResets_DeleteByUserId] @UserId UNIQUEIDENTIFIER
AS
BEGIN
    DELETE FROM [dbo].[PasswordResets] WHERE UserId = @UserId;
END

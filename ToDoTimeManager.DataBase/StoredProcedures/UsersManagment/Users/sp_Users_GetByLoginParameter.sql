CREATE PROCEDURE [dbo].[sp_Users_GetByLoginParameter] @LoginParameter NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT TOP (1)
        Id,
        Username,
        Email,
        Password,
        UserRole,
        OAuthProvider
    FROM dbo.Users
    WHERE Username = @LoginParameter
       OR Email = @LoginParameter
    ORDER BY CASE WHEN Email = @LoginParameter THEN 0 ELSE 1 END, Id;
END

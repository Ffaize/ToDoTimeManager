CREATE PROCEDURE [dbo].[sp_Users_GetByEmail]
	@Email NVARCHAR(256)
AS
	SET NOCOUNT ON;
	SELECT
        Id,
        UserName,
        Email,
        Password,
        UserRole
	FROM 
		Users
	WHERE 
		Email = @Email;

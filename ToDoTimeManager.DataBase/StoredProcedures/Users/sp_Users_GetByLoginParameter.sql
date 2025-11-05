CREATE PROCEDURE [dbo].[sp_Users_GetByLoginParameter]
	@LoginParameter NVARCHAR(256)
	AS
	SET NOCOUNT ON;
	SELECT
		Id,
		UserName,
		Email,
		Password,
		UserRole
	FROM dbo.Users
	WHERE UserName = @LoginParameter OR Email = @LoginParameter;

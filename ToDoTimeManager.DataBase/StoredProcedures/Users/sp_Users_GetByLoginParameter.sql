CREATE PROCEDURE [dbo].[sp_Users_GetByLoginParameter]
	@LoginParameter NVARCHAR(256)
	AS
	SET NOCOUNT ON;
	SELECT TOP (1)
		Id,
		UserName,
		Email,
		Password,
		UserRole
	FROM dbo.Users
	WHERE UserName = @LoginParameter OR Email = @LoginParameter
	ORDER BY
		CASE WHEN Email = @LoginParameter THEN 0 ELSE 1 END,
		Id;

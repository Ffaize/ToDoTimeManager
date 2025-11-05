CREATE PROCEDURE [dbo].[sp_Users_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		Id,
		UserName,
		Email,
		Password,
		UserRole
	FROM 
		dbo.Users
	ORDER BY 
		UserName ASC;
END

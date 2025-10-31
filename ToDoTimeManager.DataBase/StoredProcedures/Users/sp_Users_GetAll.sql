CREATE PROCEDURE [dbo].[sp_Users_GetAll]
AS
BEGIN
	SET NOCOUNT ON;
	SELECT 
		UserID,
		UserName,
		Email,
		CreatedAt,
		IsActive
	FROM 
		dbo.Users
	ORDER BY 
		UserName ASC;
END

CREATE PROCEDURE [dbo].[sp_Users_GetbyUsername]
	@UserName NVARCHAR(256)
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
		WHERE
			UserName = @UserName;
	END

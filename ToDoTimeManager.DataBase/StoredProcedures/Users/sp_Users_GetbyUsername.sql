CREATE PROCEDURE [dbo].[sp_Users_GetbyUsername]
	@UserName NVARCHAR(256)
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
		WHERE
			UserName = @UserName;
	END

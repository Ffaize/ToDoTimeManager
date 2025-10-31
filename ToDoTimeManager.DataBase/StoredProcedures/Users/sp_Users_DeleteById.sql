CREATE PROCEDURE [dbo].[sp_Users_DeleteById]
	@UserID UniqueIdentifier
AS
	BEGIN
		Update dbo.TimeLogs
			SET UserID = '00000000-0000-0000-0000-000000000000'
		WHERE UserID = @UserID;

		UPDATE dbo.TimeLogs
			SET UserId = '00000000-0000-0000-0000-000000000000'
		WHERE UserId = @UserID;

		DELETE FROM 
			dbo.Users
		WHERE
			UserID = @UserID;
	END

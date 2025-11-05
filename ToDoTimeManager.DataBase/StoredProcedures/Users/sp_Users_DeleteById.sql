CREATE PROCEDURE [dbo].[sp_Users_DeleteById]
	@Id UniqueIdentifier
AS
	BEGIN
		Update dbo.TimeLogs
			SET UserID = '00000000-0000-0000-0000-000000000000'
		WHERE UserID = @Id;

		UPDATE dbo.TimeLogs
			SET UserId = '00000000-0000-0000-0000-000000000000'
		WHERE UserId = @Id;

		DELETE FROM 
			dbo.Users
		WHERE
			Id = @Id;
	END

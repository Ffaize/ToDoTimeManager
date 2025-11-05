CREATE PROCEDURE [dbo].[sp_Users_Update]
	@Id UniqueIdentifier,
	@UserName NVARCHAR(256),
	@Email NVARCHAR(256),
	@Password NVARCHAR(512),
	@UserRole BIT
AS
	BEGIN
		SET NOCOUNT ON;
		UPDATE dbo.Users
		SET 
			UserName = @UserName,
			Email = @Email,
			Password = @Password,
			UserRole = @UserRole
		WHERE 
			Id = @Id;
	END

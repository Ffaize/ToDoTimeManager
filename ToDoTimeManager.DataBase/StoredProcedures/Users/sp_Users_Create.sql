CREATE PROCEDURE [dbo].[sp_Users_Create]
	@Id UNIQUEIDENTIFIER,
	@Username NVARCHAR(255),
	@Email NVARCHAR(255),
	@Password NVARCHAR(MAX),
	@UserRole INT
AS
	INSERT INTO [dbo].[Users] (
		Id, 
		Username, 
		Email, 
		Password, 
		UserRole
	)
	VALUES (
		@Id, 
		@Username, 
		@Email, 
		@Password, 
		@UserRole
	)

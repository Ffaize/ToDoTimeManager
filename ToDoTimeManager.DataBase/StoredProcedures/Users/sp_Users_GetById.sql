CREATE PROCEDURE [dbo].[sp_Users_GetById]
	@Id UNIQUEIDENTIFIER
AS
	SELECT 
		Id, 
		Username, 
		Email, 
		Password, 
		UserRole
	FROM [dbo].[Users]
	WHERE Id = @Id


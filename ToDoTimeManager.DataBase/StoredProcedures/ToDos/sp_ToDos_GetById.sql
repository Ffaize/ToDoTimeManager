CREATE PROCEDURE [dbo].[sp_ToDos_GetById]
	@Id UNIQUEIDENTIFIER
AS
	SELECT 
		Id,
		Title,
		Description,
		CreatedAt,
		DueDate,
		Status,
		AssignedTo
	FROM [dbo].[ToDos]
	WHERE Id = @Id
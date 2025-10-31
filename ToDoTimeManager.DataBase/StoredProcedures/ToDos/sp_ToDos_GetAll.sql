CREATE PROCEDURE [dbo].[sp_ToDos_GetAll]
AS 
	SELECT
		Id,
		Title,
		Description,
		CreatedAt,
		DueDate,
		Status,
		AssignedTo
	FROM
		[dbo].[ToDos]
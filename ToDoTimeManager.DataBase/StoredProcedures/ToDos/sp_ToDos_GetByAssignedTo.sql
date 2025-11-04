CREATE PROCEDURE [dbo].[sp_ToDos_GetByAssignedTo]
	@AssignedTo UNIQUEIDENTIFIER
AS
	SELECT
		Id,
		Title,
		Description,
		CreatedAt,
		DueDate,
		Status,
		AssignedTo,
		NumberedId
		FROM [dbo].[ToDos]
		WHERE AssignedTo = @AssignedTo


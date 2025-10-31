CREATE PROCEDURE [dbo].[sp_TimeLogs_GetById]
	@Id UNIQUEIDENTIFIER
AS
	SELECT 
		Id,
		ToDoId,
		UserId,
		HoursSpent,
		LogDate,
		LogDescription
	FROM [dbo].[TimeLogs]
	WHERE Id = @Id


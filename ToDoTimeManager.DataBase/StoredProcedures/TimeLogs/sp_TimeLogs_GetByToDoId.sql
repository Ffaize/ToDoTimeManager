CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByToDoId]
	@ToDoId UNIQUEIDENTIFIER
AS
	SELECT 
		Id,
		UserId,
		ToDoId,
		StartTime,
		EndTime,
		DurationInMinutes,
		Description,
		CreatedAt
		FROM [dbo].[TimeLogs]
		WHERE ToDoId = @ToDoId


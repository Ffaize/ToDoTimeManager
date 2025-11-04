CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByUserIdAndToDoId]
	@UserId UNIQUEIDENTIFIER,
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
		WHERE UserId = @UserId AND ToDoId = @ToDoId
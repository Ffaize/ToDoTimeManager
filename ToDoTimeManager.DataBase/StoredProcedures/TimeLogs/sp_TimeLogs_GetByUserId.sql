CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByUserId]
	@UserId UNIQUEIDENTIFIER
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
		WHERE UserId = @UserId

CREATE PROCEDURE [dbo].[sp_TimeLogs_GetAll]
AS 
	SELECT
		Id,
		UserId,
		ToDoId,
		StartTime,
		EndTime,
		Description
	FROM
		[dbo].[TimeLogs]

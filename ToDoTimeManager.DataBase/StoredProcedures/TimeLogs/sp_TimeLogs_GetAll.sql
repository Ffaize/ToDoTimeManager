CREATE PROCEDURE [dbo].[sp_TimeLogs_GetAll]
AS 
	SELECT
        [Id],
        [UserId],
        [ToDoId],
        [HoursSpent],
        [LogDescription],
        [LogDate]
	FROM
		[dbo].[TimeLogs]

CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByToDoId]
	@ToDoId UNIQUEIDENTIFIER
AS
	SELECT
        [Id],
        [UserId],
        [ToDoId],
        [HoursSpent],
        [LogDescription],
        [LogDate]
		FROM [dbo].[TimeLogs]
		WHERE ToDoId = @ToDoId


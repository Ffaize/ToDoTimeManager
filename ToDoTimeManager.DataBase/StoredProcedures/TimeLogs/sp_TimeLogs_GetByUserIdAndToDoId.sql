CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByUserIdAndToDoId]
	@UserId UNIQUEIDENTIFIER,
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
		WHERE [UserId] = @UserId AND [ToDoId] = @ToDoId
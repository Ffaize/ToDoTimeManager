CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByUserId]
	@UserId UNIQUEIDENTIFIER
AS
	SELECT
        [Id],
        [UserId],
        [ToDoId],
        [HoursSpent],
        [LogDescription],
        [LogDate]
		FROM [dbo].[TimeLogs]
		WHERE UserId = @UserId

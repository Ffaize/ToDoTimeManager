CREATE PROCEDURE [dbo].[sp_ToDos_GetCountByUserIdAndStatus]
	@UserId UNIQUEIDENTIFIER,
	@ToDoStatus INT
	AS
	BEGIN
	SET NOCOUNT ON;
		SELECT *
		FROM ToDos
		WHERE [AssignedTo] = @UserId
		  AND [Status] = @ToDoStatus;
	END
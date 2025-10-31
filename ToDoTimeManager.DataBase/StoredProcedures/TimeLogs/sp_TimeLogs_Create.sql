CREATE PROCEDURE [dbo].[sp_TimeLogs_Create]
	@Id UNIQUEIDENTIFIER,
	@ToDoId UNIQUEIDENTIFIER,
	@UserId UNIQUEIDENTIFIER,
	@HoursSpent TIME,
	@LogDate DATETIME,
	@LogDescription NVARCHAR(200)
AS
	INSERT INTO [dbo].[TimeLogs] (
		Id,
		ToDoId,
		UserId,
		HoursSpent,
		LogDate,
		LogDescription
	)
	VALUES (
		@Id,
		@ToDoId,
		@UserId,
		@HoursSpent,
		@LogDate,
		@LogDescription
	)

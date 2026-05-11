CREATE PROCEDURE [dbo].[sp_TimeLogs_Update] @Id UNIQUEIDENTIFIER,
                                            @ToDoId UNIQUEIDENTIFIER,
                                            @UserId UNIQUEIDENTIFIER,
                                            @HoursSpent DECIMAL(6, 2),
                                            @LogDate DATETIME,
                                            @LogDescription NVARCHAR(1000)
AS
UPDATE [dbo].[TimeLogs]
SET ToDoId         = @ToDoId,
    UserId         = @UserId,
    HoursSpent     = @HoursSpent,
    LogDate        = @LogDate,
    LogDescription = @LogDescription
WHERE Id = @Id

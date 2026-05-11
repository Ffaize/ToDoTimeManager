CREATE PROCEDURE [dbo].[sp_TimeLogs_Create] @Id UNIQUEIDENTIFIER,
                                            @ToDoId UNIQUEIDENTIFIER,
                                            @UserId UNIQUEIDENTIFIER,
                                            @HoursSpent DECIMAL(6, 2),
                                            @LogDate DATETIME,
                                            @LogDescription NVARCHAR(1000)
AS
INSERT INTO [dbo].[TimeLogs] (Id,
                              ToDoId,
                              UserId,
                              HoursSpent,
                              LogDate,
                              LogDescription)
VALUES (@Id,
        @ToDoId,
        @UserId,
        @HoursSpent,
        @LogDate,
        @LogDescription)

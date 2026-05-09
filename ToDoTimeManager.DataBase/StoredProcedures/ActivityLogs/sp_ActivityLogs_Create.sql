CREATE PROCEDURE [dbo].[sp_ActivityLogs_Create] @Id           UNIQUEIDENTIFIER,
                                               @ToDoId       UNIQUEIDENTIFIER,
                                               @UserId       UNIQUEIDENTIFIER,
                                               @Type         INT,
                                               @Description  NVARCHAR(500),
                                               @ActivityTime DATETIME
AS
INSERT INTO [dbo].[ActivityLogs] ([Id], [ToDoId], [UserId], [Type], [Description], [ActivityTime])
VALUES (@Id, @ToDoId, @UserId, @Type, @Description, @ActivityTime)

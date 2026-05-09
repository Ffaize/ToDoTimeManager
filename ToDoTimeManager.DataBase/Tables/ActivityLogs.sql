CREATE TABLE [dbo].[ActivityLogs]
(
    Id           UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ToDoId       UNIQUEIDENTIFIER NULL,
    UserId       UNIQUEIDENTIFIER NOT NULL,
    Type         INT              NOT NULL,
    Description  NVARCHAR(500)    NOT NULL,
    ActivityTime DATETIME         NOT NULL,
    CONSTRAINT [FK_ActivityLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_ActivityLogs_ToDos] FOREIGN KEY ([ToDoId]) REFERENCES [ToDos] ([Id])
)

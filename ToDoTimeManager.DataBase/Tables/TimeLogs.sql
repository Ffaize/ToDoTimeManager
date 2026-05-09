CREATE TABLE [dbo].[TimeLogs]
(
    Id             UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ToDoId         UNIQUEIDENTIFIER NOT NULL,
    UserId         UNIQUEIDENTIFIER NOT NULL,
    HoursSpent     DECIMAL(6, 2)    NOT NULL,
    LogDate        DATETIME         NOT NULL,
    LogDescription NVARCHAR(1000)   NULL,
    CONSTRAINT [FK_TimeLogs_Users] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_TimeLogs_ToDos] FOREIGN KEY ([ToDoId]) REFERENCES [ToDos] ([Id])
)

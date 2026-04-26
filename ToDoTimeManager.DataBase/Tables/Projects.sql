CREATE TABLE [dbo].[Projects]
(
    Id          UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    Name        NVARCHAR(100)    NOT NULL,
    Description NVARCHAR(500)    NULL,
    CreatedAt   DATETIME         NOT NULL,
    CreatedBy   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [FK_Projects_Users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [Users]([Id])
)

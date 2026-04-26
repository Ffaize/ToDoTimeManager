CREATE TABLE [dbo].[TeamMembers]
(
    Id     UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    TeamId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    Role   INT              NOT NULL DEFAULT 0,
    CONSTRAINT [FK_TeamMembers_Teams] FOREIGN KEY ([TeamId]) REFERENCES [Teams]([Id]),
    CONSTRAINT [FK_TeamMembers_Users] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]),
    CONSTRAINT [UQ_TeamMembers_TeamId_UserId] UNIQUE ([TeamId], [UserId])
)

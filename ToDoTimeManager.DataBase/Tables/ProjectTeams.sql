CREATE TABLE [dbo].[ProjectTeams]
(
    Id               UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    ProjectId        UNIQUEIDENTIFIER NOT NULL,
    TeamId           UNIQUEIDENTIFIER NOT NULL,
    ProjectManagerId UNIQUEIDENTIFIER NULL,
    CONSTRAINT [FK_ProjectTeams_Projects]        FOREIGN KEY ([ProjectId])        REFERENCES [Projects]([Id]),
    CONSTRAINT [FK_ProjectTeams_Teams]           FOREIGN KEY ([TeamId])           REFERENCES [Teams]([Id]),
    CONSTRAINT [FK_ProjectTeams_Users_Manager]   FOREIGN KEY ([ProjectManagerId]) REFERENCES [Users]([Id]),
    CONSTRAINT [UQ_ProjectTeams_ProjectId_TeamId] UNIQUE ([ProjectId], [TeamId])
)

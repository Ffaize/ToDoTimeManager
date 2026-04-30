CREATE PROCEDURE [dbo].[sp_ProjectTeams_Create]
    @Id               UNIQUEIDENTIFIER,
    @ProjectId        UNIQUEIDENTIFIER,
    @TeamId           UNIQUEIDENTIFIER,
    @ProjectManagerId UNIQUEIDENTIFIER = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[ProjectTeams] (Id, ProjectId, TeamId, ProjectManagerId)
    VALUES (@Id, @ProjectId, @TeamId, @ProjectManagerId);
END

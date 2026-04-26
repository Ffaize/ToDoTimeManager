CREATE PROCEDURE [dbo].[sp_ProjectTeams_Create]
    @Id        UNIQUEIDENTIFIER,
    @ProjectId UNIQUEIDENTIFIER,
    @TeamId    UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[ProjectTeams] (Id, ProjectId, TeamId)
    VALUES (@Id, @ProjectId, @TeamId);
END

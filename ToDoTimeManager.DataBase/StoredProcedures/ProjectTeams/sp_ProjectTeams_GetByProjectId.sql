CREATE PROCEDURE [dbo].[sp_ProjectTeams_GetByProjectId]
    @ProjectId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ProjectId, TeamId, ProjectManagerId
    FROM [dbo].[ProjectTeams]
    WHERE ProjectId = @ProjectId;
END

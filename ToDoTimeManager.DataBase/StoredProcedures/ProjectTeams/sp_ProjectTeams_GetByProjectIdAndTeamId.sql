CREATE PROCEDURE [dbo].[sp_ProjectTeams_GetByProjectIdAndTeamId]
    @ProjectId UNIQUEIDENTIFIER,
    @TeamId    UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, ProjectId, TeamId
    FROM [dbo].[ProjectTeams]
    WHERE ProjectId = @ProjectId AND TeamId = @TeamId;
END

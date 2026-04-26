CREATE PROCEDURE [dbo].[sp_ProjectTeams_DeleteByProjectIdAndTeamId]
    @ProjectId UNIQUEIDENTIFIER,
    @TeamId    UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM [dbo].[ProjectTeams]
    WHERE ProjectId = @ProjectId AND TeamId = @TeamId;
END

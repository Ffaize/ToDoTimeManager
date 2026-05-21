CREATE PROCEDURE [dbo].[sp_Projects_GetIdsByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT p.Id
    FROM [dbo].[Projects] p
    WHERE p.CreatedBy = @UserId
       OR EXISTS (
           SELECT 1
           FROM   [dbo].[ProjectTeams] pt
           JOIN   [dbo].[TeamMembers]  tm ON tm.TeamId = pt.TeamId AND tm.UserId = @UserId
           WHERE  pt.ProjectId = p.Id
       );
END

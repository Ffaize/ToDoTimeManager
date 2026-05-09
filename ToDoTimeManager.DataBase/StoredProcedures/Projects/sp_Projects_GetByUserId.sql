CREATE PROCEDURE [dbo].[sp_Projects_GetByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT
        p.Id,
        p.Name,
        p.Description,
        p.CreatedAt,
        p.CreatedBy,
        p.Type,
        COUNT(pt2.Id) AS TeamCount
    FROM [dbo].[Projects] p
    LEFT  JOIN [dbo].[ProjectTeams] pt2 ON pt2.ProjectId = p.Id
    WHERE p.CreatedBy = @UserId
       OR EXISTS (
           SELECT 1
           FROM   [dbo].[ProjectTeams] pt
           JOIN   [dbo].[TeamMembers]  tm ON tm.TeamId = pt.TeamId AND tm.UserId = @UserId
           WHERE  pt.ProjectId = p.Id
       )
    GROUP BY p.Id, p.Name, p.Description, p.CreatedAt, p.CreatedBy, p.Type
    ORDER BY p.Name ASC;
END

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
        COUNT(pt2.Id) AS TeamCount
    FROM [dbo].[Projects] p
    INNER JOIN [dbo].[ProjectTeams] pt  ON pt.ProjectId  = p.Id
    INNER JOIN [dbo].[TeamMembers]  tm  ON tm.TeamId     = pt.TeamId AND tm.UserId = @UserId
    LEFT  JOIN [dbo].[ProjectTeams] pt2 ON pt2.ProjectId = p.Id
    GROUP BY p.Id, p.Name, p.Description, p.CreatedAt, p.CreatedBy
    ORDER BY p.Name ASC;
END

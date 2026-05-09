CREATE PROCEDURE [dbo].[sp_Projects_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.Id,
        p.Name,
        p.Description,
        p.CreatedAt,
        p.CreatedBy,
        p.Type,
        COUNT(pt.Id) AS TeamCount
    FROM [dbo].[Projects] p
    LEFT JOIN [dbo].[ProjectTeams] pt ON pt.ProjectId = p.Id
    GROUP BY p.Id, p.Name, p.Description, p.CreatedAt, p.CreatedBy, p.Type
    ORDER BY p.Name ASC;
END

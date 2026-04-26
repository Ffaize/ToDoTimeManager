CREATE PROCEDURE [dbo].[sp_Projects_GetById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        p.Id,
        p.Name,
        p.Description,
        p.CreatedAt,
        p.CreatedBy,
        COUNT(pt.Id) AS TeamCount
    FROM [dbo].[Projects] p
    LEFT JOIN [dbo].[ProjectTeams] pt ON pt.ProjectId = p.Id
    WHERE p.Id = @Id
    GROUP BY p.Id, p.Name, p.Description, p.CreatedAt, p.CreatedBy;
END

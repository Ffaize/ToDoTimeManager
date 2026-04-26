CREATE PROCEDURE [dbo].[sp_Teams_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        t.Id,
        t.Name,
        t.Description,
        t.CreatedAt,
        t.CreatedBy,
        COUNT(tm.Id) AS MemberCount
    FROM [dbo].[Teams] t
    LEFT JOIN [dbo].[TeamMembers] tm ON tm.TeamId = t.Id
    GROUP BY t.Id, t.Name, t.Description, t.CreatedAt, t.CreatedBy
    ORDER BY t.Name ASC;
END

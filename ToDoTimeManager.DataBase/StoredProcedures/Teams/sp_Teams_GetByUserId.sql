CREATE PROCEDURE [dbo].[sp_Teams_GetByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        t.Id,
        t.Name,
        t.Description,
        t.CreatedAt,
        t.CreatedBy,
        COUNT(tm2.Id) AS MemberCount
    FROM [dbo].[Teams] t
    INNER JOIN [dbo].[TeamMembers] tm  ON tm.TeamId  = t.Id AND tm.UserId = @UserId
    LEFT JOIN  [dbo].[TeamMembers] tm2 ON tm2.TeamId = t.Id
    GROUP BY t.Id, t.Name, t.Description, t.CreatedAt, t.CreatedBy
    ORDER BY t.Name ASC;
END

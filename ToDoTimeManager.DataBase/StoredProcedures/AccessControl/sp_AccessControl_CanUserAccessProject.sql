CREATE OR ALTER PROCEDURE sp_AccessControl_CanUserAccessProject
    @UserId    UNIQUEIDENTIFIER,
    @ProjectId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    -- Creator always has access
    IF EXISTS (SELECT 1 FROM Projects WHERE Id = @ProjectId AND CreatedBy = @UserId)
    BEGIN
        SELECT CAST(1 AS BIT);
        RETURN;
    END

    -- ProjectManager of any ProjectTeam linked to this project, or any team member
    IF EXISTS (
        SELECT 1
        FROM   ProjectTeams pt
        LEFT JOIN TeamMembers tm ON pt.TeamId = tm.TeamId AND tm.UserId = @UserId
        WHERE  pt.ProjectId = @ProjectId
          AND  (pt.ProjectManagerId = @UserId OR tm.Id IS NOT NULL)
    )
    BEGIN
        SELECT CAST(1 AS BIT);
        RETURN;
    END

    SELECT CAST(0 AS BIT);
END

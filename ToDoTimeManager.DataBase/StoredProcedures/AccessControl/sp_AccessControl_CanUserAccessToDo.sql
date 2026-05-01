CREATE OR ALTER PROCEDURE sp_AccessControl_CanUserAccessToDo
    @UserId UNIQUEIDENTIFIER,
    @ToDoId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TeamId    UNIQUEIDENTIFIER;
    DECLARE @ProjectId UNIQUEIDENTIFIER;

    SELECT @TeamId = TeamId, @ProjectId = ProjectId
    FROM   ToDos
    WHERE  Id = @ToDoId;

    -- Directly assigned
    IF EXISTS (SELECT 1 FROM ToDos WHERE Id = @ToDoId AND AssignedTo = @UserId)
    BEGIN
        SELECT CAST(1 AS BIT);
        RETURN;
    END

    -- Member of the todo's team
    IF @TeamId IS NOT NULL AND EXISTS (
        SELECT 1 FROM TeamMembers WHERE TeamId = @TeamId AND UserId = @UserId
    )
    BEGIN
        SELECT CAST(1 AS BIT);
        RETURN;
    END

    -- Can access the project the todo belongs to
    IF @ProjectId IS NOT NULL
    BEGIN
        DECLARE @CanAccessProject BIT = 0;

        IF EXISTS (SELECT 1 FROM Projects WHERE Id = @ProjectId AND CreatedBy = @UserId)
            SET @CanAccessProject = 1;

        IF @CanAccessProject = 0 AND EXISTS (
            SELECT 1
            FROM   ProjectTeams pt
            LEFT JOIN TeamMembers tm ON pt.TeamId = tm.TeamId AND tm.UserId = @UserId
            WHERE  pt.ProjectId = @ProjectId
              AND  (pt.ProjectManagerId = @UserId OR tm.Id IS NOT NULL)
        )
            SET @CanAccessProject = 1;

        SELECT @CanAccessProject;
        RETURN;
    END

    SELECT CAST(0 AS BIT);
END

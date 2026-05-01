CREATE OR ALTER PROCEDURE sp_AccessControl_CanUserAccessTimeLog
    @UserId    UNIQUEIDENTIFIER,
    @TimeLogId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ToDoId UNIQUEIDENTIFIER;

    SELECT @ToDoId = ToDoId FROM TimeLogs WHERE Id = @TimeLogId;

    -- Owner of the timelog
    IF EXISTS (SELECT 1 FROM TimeLogs WHERE Id = @TimeLogId AND UserId = @UserId)
    BEGIN
        SELECT CAST(1 AS BIT);
        RETURN;
    END

    -- Can access the associated todo
    IF @ToDoId IS NOT NULL
    BEGIN
        DECLARE @TeamId    UNIQUEIDENTIFIER;
        DECLARE @ProjectId UNIQUEIDENTIFIER;

        SELECT @TeamId = TeamId, @ProjectId = ProjectId FROM ToDos WHERE Id = @ToDoId;

        IF EXISTS (SELECT 1 FROM ToDos WHERE Id = @ToDoId AND AssignedTo = @UserId)
        BEGIN
            SELECT CAST(1 AS BIT);
            RETURN;
        END

        IF @TeamId IS NOT NULL AND EXISTS (
            SELECT 1 FROM TeamMembers WHERE TeamId = @TeamId AND UserId = @UserId
        )
        BEGIN
            SELECT CAST(1 AS BIT);
            RETURN;
        END

        IF @ProjectId IS NOT NULL
        BEGIN
            IF EXISTS (SELECT 1 FROM Projects WHERE Id = @ProjectId AND CreatedBy = @UserId)
            BEGIN
                SELECT CAST(1 AS BIT);
                RETURN;
            END

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
        END
    END

    SELECT CAST(0 AS BIT);
END

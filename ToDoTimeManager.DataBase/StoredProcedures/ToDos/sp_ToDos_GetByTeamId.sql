CREATE PROCEDURE [dbo].[sp_ToDos_GetByTeamId]
    @TeamId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        Id,
        Title,
        Description,
        CreatedAt,
        DueDate,
        Status,
        AssignedTo,
        NumberedId,
        TeamId
    FROM [dbo].[ToDos]
    WHERE TeamId = @TeamId;
END

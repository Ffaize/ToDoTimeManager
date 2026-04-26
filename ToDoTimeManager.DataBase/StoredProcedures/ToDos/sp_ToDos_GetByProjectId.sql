CREATE PROCEDURE [dbo].[sp_ToDos_GetByProjectId]
    @ProjectId UNIQUEIDENTIFIER
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
        TeamId,
        ProjectId
    FROM [dbo].[ToDos]
    WHERE ProjectId = @ProjectId;
END

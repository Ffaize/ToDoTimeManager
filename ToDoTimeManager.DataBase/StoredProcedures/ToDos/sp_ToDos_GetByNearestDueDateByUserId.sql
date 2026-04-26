CREATE PROCEDURE [dbo].[sp_ToDos_GetByNearestDueDateByUserId]
@UserId UNIQUEIDENTIFIER
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
    WHERE AssignedTo = @UserId
      AND DueDate IS NOT NULL
      AND CAST(DueDate AS DATE) >= CAST(GETUTCDATE() AS DATE)
      AND CAST(DueDate AS DATE) <= DATEADD(DAY, 7, CAST(GETUTCDATE() AS DATE))
    ORDER BY DueDate ASC;
END
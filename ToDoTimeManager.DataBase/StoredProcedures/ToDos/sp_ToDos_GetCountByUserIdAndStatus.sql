CREATE PROCEDURE [dbo].[sp_ToDos_GetCountByUserIdAndStatus] @UserId     UNIQUEIDENTIFIER,
                                                            @ToDoStatus INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id,
           Title,
           Description,
           CreatedAt,
           DueDate,
           Status,
           AssignedTo,
           NumberedId,
           TeamId,
           ProjectId,
           Type
    FROM [dbo].[ToDos]
    WHERE [AssignedTo] = @UserId
      AND [Status]     = @ToDoStatus;
END

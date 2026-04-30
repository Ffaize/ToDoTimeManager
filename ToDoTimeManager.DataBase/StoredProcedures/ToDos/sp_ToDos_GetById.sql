CREATE PROCEDURE [dbo].[sp_ToDos_GetById] @Id UNIQUEIDENTIFIER
AS
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
WHERE Id = @Id

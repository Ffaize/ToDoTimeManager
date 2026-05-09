CREATE PROCEDURE [dbo].[sp_ActivityLogs_GetAll]
AS
SELECT al.[Id],
       al.[ToDoId],
       al.[UserId],
       al.[Type],
       al.[Description],
       al.[ActivityTime],
       u.[Username]   AS UserName,
       t.[Title]      AS ToDoTitle,
       t.[NumberedId] AS ToDoNumberedId
FROM [dbo].[ActivityLogs] al
         INNER JOIN [dbo].[Users] u ON al.[UserId] = u.[Id]
         LEFT JOIN [dbo].[ToDos] t ON al.[ToDoId] = t.[Id]
ORDER BY al.[ActivityTime] DESC

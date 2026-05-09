CREATE PROCEDURE [dbo].[sp_ActivityLogs_DeleteById] @Id UNIQUEIDENTIFIER
AS
DELETE
FROM [dbo].[ActivityLogs]
WHERE [Id] = @Id

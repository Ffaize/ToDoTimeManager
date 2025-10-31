CREATE PROCEDURE [dbo].[sp_TimeLogs_DeleteById]
	@Id UNIQUEIDENTIFIER
AS
	DELETE FROM [dbo].[TimeLogs]
	WHERE
		Id = @Id
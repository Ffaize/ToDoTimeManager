CREATE PROCEDURE [dbo].[sp_Projects_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ToDos]
    SET ProjectId = NULL
    WHERE ProjectId = @Id;

    DELETE FROM [dbo].[ProjectTeams]
    WHERE ProjectId = @Id;

    DELETE FROM [dbo].[Projects]
    WHERE Id = @Id;
END

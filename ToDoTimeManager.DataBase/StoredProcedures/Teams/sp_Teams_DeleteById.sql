CREATE PROCEDURE [dbo].[sp_Teams_DeleteById] @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ToDos]
    SET TeamId = NULL
    WHERE TeamId = @Id;

    DELETE
    FROM [dbo].[TeamMembers]
    WHERE TeamId = @Id;

    DELETE
    FROM [dbo].[Teams]
    WHERE Id = @Id;
END

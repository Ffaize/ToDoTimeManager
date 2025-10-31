CREATE PROCEDURE [dbo].[sp_ToDos_DeleteById]
	@Id UNIQUEIDENTIFIER
AS
    BEGIN
        Update dbo.TimeLogs
            SET ToDoId = '00000000-0000-0000-0000-000000000000'
        WHERE ToDoId = @Id;

        DELETE FROM [dbo].[ToDos]
        WHERE
            Id = @Id
    END



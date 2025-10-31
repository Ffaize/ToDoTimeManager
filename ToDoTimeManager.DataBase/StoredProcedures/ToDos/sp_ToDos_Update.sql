CREATE PROCEDURE [dbo].[sp_ToDos_Update]
    @Id UNIQUEIDENTIFIER,
    @Title NVARCHAR(100),
    @Description NVARCHAR(MAX),
    @CreatedAt DATETIME,
    @DueDate DATETIME,
    @Status INT,
    @AssignedTo UNIQUEIDENTIFIER
AS
    UPDATE [dbo].[ToDos]
    SET
        Title = @Title,
        Description = @Description,
        CreatedAt = @CreatedAt,
        DueDate = @DueDate,
        Status = @Status,
        AssignedTo = @AssignedTo
    WHERE
        Id = @Id

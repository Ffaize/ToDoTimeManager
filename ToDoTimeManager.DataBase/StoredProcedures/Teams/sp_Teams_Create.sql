CREATE PROCEDURE [dbo].[sp_Teams_Create] @Id UNIQUEIDENTIFIER,
                                         @Name NVARCHAR(100),
                                         @Description NVARCHAR(500),
                                         @CreatedAt DATETIME,
                                         @CreatedBy UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[Teams] (Id, Name, Description, CreatedAt, CreatedBy)
    VALUES (@Id, @Name, @Description, @CreatedAt, @CreatedBy);
END

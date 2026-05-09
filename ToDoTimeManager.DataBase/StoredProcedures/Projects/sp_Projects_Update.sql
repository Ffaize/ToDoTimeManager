CREATE PROCEDURE [dbo].[sp_Projects_Update]
    @Id          UNIQUEIDENTIFIER,
    @Name        NVARCHAR(100),
    @Description NVARCHAR(500),
    @Type        INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Projects]
    SET Name        = @Name,
        Description = @Description,
        Type        = @Type
    WHERE Id = @Id;
END

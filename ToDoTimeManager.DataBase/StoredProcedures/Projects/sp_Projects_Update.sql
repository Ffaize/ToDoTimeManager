CREATE PROCEDURE [dbo].[sp_Projects_Update]
    @Id          UNIQUEIDENTIFIER,
    @Name        NVARCHAR(100),
    @Description NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Projects]
    SET Name        = @Name,
        Description = @Description
    WHERE Id = @Id;
END

CREATE PROCEDURE [dbo].[sp_Teams_Update]
    @Id          UNIQUEIDENTIFIER,
    @Name        NVARCHAR(100),
    @Description NVARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[Teams]
    SET Name        = @Name,
        Description = @Description
    WHERE Id = @Id;
END

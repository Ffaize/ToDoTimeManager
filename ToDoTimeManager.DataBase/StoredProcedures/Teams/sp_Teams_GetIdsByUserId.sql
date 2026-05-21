CREATE PROCEDURE [dbo].[sp_Teams_GetIdsByUserId] @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT tm.TeamId
    FROM [dbo].[TeamMembers] tm
    WHERE tm.UserId = @UserId;
END

CREATE PROCEDURE [dbo].[sp_TwoFactorCodes_GetByUserId] @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, UserId, Code, ExpiresAt
    FROM [dbo].[TwoFactorCodes]
    WHERE UserId = @UserId;
END

CREATE PROCEDURE [dbo].[sp_TimeLogs_GetByUserIdAndTime]
	@UserId UNIQUEIDENTIFIER,
	@DaysAgo INT
	AS
	BEGIN
	SET NOCOUNT ON;
	SELECT *
	FROM TimeLogs
	WHERE [UserId] = @UserId
	  AND LogDate >= DATEADD(DAY, -@DaysAgo, GETUTCDATE())
	ORDER BY LogDate DESC
	END
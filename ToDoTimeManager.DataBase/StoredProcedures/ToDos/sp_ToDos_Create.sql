CREATE PROCEDURE [dbo].[sp_ToDos_Create]
	@Id UNIQUEIDENTIFIER,
	@NumberedId INT,
	@Title NVARCHAR(100),
	@Description NVARCHAR(MAX),
	@CreatedAt DATETIME,
	@DueDate DATETIME,
	@Status INT,
	@AssignedTo UNIQUEIDENTIFIER
AS
	INSERT INTO [dbo].[ToDos] (
		Id,
		Title,
		Description,
		CreatedAt,
		DueDate,
		Status,
		AssignedTo,
		NumberedId
	)
	VALUES (
		@Id,
		@Title,
		@Description,
		@CreatedAt,
		@DueDate,
		@Status,
		@AssignedTo,
		(SELECT ISNULL(MAX(NumberedId), 0) + 1 FROM [dbo].[ToDos])
	)

CREATE PROCEDURE [dbo].[sp_ToDos_Create]
	@Id UNIQUEIDENTIFIER,
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
		AssignedTo
	)
	VALUES (
		@Id,
		@Title,
		@Description,
		@CreatedAt,
		@DueDate,
		@Status,
		@AssignedTo
	)

CREATE TABLE [dbo].[ImportProgresses]
(
	[LineNumber] INT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [Type] TINYINT NOT NULL, 
    [Message] VARCHAR(MAX) NOT NULL
)

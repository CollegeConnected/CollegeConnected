CREATE TABLE [dbo].[ImportProgresses]
(
	[LineNumber] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Type] TINYINT NOT NULL, 
    [Message] VARCHAR(MAX) NOT NULL
)

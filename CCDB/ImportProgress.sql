CREATE TABLE [dbo].[ImportProgresses]
(
	[LineNumber] INT NOT NULL PRIMARY KEY, 
    [Type] TINYINT NOT NULL, 
    [Message] VARCHAR(MAX) NOT NULL
)

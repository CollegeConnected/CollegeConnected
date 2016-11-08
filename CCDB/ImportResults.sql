CREATE TABLE [dbo].[ImportResults]
(
	[Id] SMALLINT NOT NULL PRIMARY KEY, 
    [Type] VARCHAR(50) NOT NULL, 
    [ImportFile] VARBINARY(MAX) NOT NULL, 
    [RejectFile] VARBINARY(MAX) NOT NULL, 
    [ImportCount] SMALLINT NOT NULL, 
    [TimeStamp] DATETIME NOT NULL, 
    [Errors] NVARCHAR(MAX) NOT NULL, 
    [ConvertCount] SMALLINT NOT NULL
)

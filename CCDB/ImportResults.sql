CREATE TABLE [dbo].[ImportResults]
(
	[Id] SMALLINT NOT NULL PRIMARY KEY IDENTITY (1,1), 
    [Type] VARCHAR(50) NOT NULL, 
    [ImportFile] VARBINARY(MAX) NOT NULL, 
    [RejectFile] VARBINARY(MAX) NULL, 
    [ImportCount] SMALLINT NOT NULL, 
    [TimeStamp] DATETIME NOT NULL, 
    [Errors] NVARCHAR(MAX) NULL, 
    [ConvertCount] SMALLINT NOT NULL
)

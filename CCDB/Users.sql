GO
CREATE TABLE [dbo].[Users]
(
	[UserID] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Username] VARCHAR(100) NOT NULL, 
    [Password] VARBINARY(50) NULL
);


INSERT INTO [dbo].[Users] (Username, Password) VALUES ('unf-alumni@unf.edu','password');

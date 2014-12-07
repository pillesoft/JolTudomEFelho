CREATE TABLE [users].[Person] (
    [PersonID]   INT              IDENTITY (1, 1) NOT NULL,
    [UserName]   VARCHAR (8)      NOT NULL,
    [Prefix]     VARCHAR (5)      NULL,
    [LastName]   VARCHAR (25)     NOT NULL,
    [MiddleName] VARCHAR (25)     NULL,
    [FirstName]  VARCHAR (25)     NOT NULL,
    [Created]    DATETIME         DEFAULT (getdate()) NOT NULL,
    [Password]   VARCHAR (21)     NOT NULL,
    [UserGuid]   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Student_PersonID] PRIMARY KEY CLUSTERED ([PersonID] ASC)
);


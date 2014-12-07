CREATE TABLE [users].[Sessions] (
    [Token]      VARCHAR (100) NOT NULL,
    [PersonID]   INT           NOT NULL,
    [LastAction] DATETIME      NOT NULL,
    [RoleID]     INT           NOT NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED ([Token] ASC),
    CONSTRAINT [FK_Sessions_Person] FOREIGN KEY ([PersonID]) REFERENCES [users].[Person] ([PersonID])
);


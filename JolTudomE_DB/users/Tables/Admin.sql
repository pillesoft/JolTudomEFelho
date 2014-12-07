CREATE TABLE [users].[Admin] (
    [AdminID] INT NOT NULL,
    CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED ([AdminID] ASC),
    CONSTRAINT [FK_Admin_Person] FOREIGN KEY ([AdminID]) REFERENCES [users].[Person] ([PersonID])
);


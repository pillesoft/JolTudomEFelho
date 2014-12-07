CREATE TABLE [users].[Student] (
    [StudentID] INT NOT NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY CLUSTERED ([StudentID] ASC),
    CONSTRAINT [FK_Student_Person] FOREIGN KEY ([StudentID]) REFERENCES [users].[Person] ([PersonID])
);


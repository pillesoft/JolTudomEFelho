CREATE TABLE [users].[Teacher] (
    [TeacherID] INT NOT NULL,
    CONSTRAINT [PK_Teacher] PRIMARY KEY CLUSTERED ([TeacherID] ASC),
    CONSTRAINT [FK_Teacher_Person] FOREIGN KEY ([TeacherID]) REFERENCES [users].[Person] ([PersonID])
);


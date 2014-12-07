CREATE TABLE [test].[Results] (
    [RowID]         INT IDENTITY (1, 1) NOT NULL,
    [TestID]        INT NOT NULL,
    [QuestionID]    INT NOT NULL,
    [AnswerID]      INT NULL,
    [PersonID]      INT NOT NULL,
    [CheckedAnswer] BIT DEFAULT ((0)) NULL,
    CONSTRAINT [PK_Results] PRIMARY KEY CLUSTERED ([RowID] ASC),
    CONSTRAINT [FK_Results_Answers] FOREIGN KEY ([AnswerID]) REFERENCES [test].[Answers] ([AnswerID]),
    CONSTRAINT [FK_Results_Person] FOREIGN KEY ([PersonID]) REFERENCES [users].[Person] ([PersonID]),
    CONSTRAINT [FK_Results_Questions] FOREIGN KEY ([QuestionID]) REFERENCES [test].[Questions] ([QuestionID]),
    CONSTRAINT [FK_Results_Times] FOREIGN KEY ([TestID]) REFERENCES [test].[Times] ([TestID])
);


CREATE TABLE [test].[Answers] (
    [AnswerID]   INT          IDENTITY (1, 1) NOT NULL,
    [QuestionID] INT          NOT NULL,
    [AnswerText] VARCHAR (50) NOT NULL,
    [IsCorrect]  BIT          NOT NULL,
    CONSTRAINT [PK_Answers] PRIMARY KEY CLUSTERED ([AnswerID] ASC),
    CONSTRAINT [FK_Answers_Questions] FOREIGN KEY ([QuestionID]) REFERENCES [test].[Questions] ([QuestionID])
);


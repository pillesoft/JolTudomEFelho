CREATE TABLE [test].[Questions] (
    [QuestionID]   INT           IDENTITY (1, 1) NOT NULL,
    [QuestionText] VARCHAR (255) NOT NULL,
    [TopicID]      INT           NOT NULL,
    CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED ([QuestionID] ASC),
    CONSTRAINT [FK_Questions_Topics] FOREIGN KEY ([TopicID]) REFERENCES [course].[Topics] ([TopicID])
);


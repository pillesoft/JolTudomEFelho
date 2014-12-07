CREATE TABLE [course].[Topics] (
    [TopicID]          INT           IDENTITY (1, 1) NOT NULL,
    [CourseID]         INT           NOT NULL,
    [TopicName]        VARCHAR (50)  NOT NULL,
    [TopicDescription] VARCHAR (255) NULL,
    CONSTRAINT [PK_course.Topics] PRIMARY KEY CLUSTERED ([TopicID] ASC),
    CONSTRAINT [FK_course.Topics_Courses] FOREIGN KEY ([CourseID]) REFERENCES [course].[Courses] ([CourseID])
);


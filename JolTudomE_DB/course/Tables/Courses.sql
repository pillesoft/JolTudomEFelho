CREATE TABLE [course].[Courses] (
    [CourseID]          INT           IDENTITY (1, 1) NOT NULL,
    [CourseName]        VARCHAR (30)  NOT NULL,
    [CourseDescription] VARCHAR (255) NULL,
    CONSTRAINT [PK_course.Courses] PRIMARY KEY CLUSTERED ([CourseID] ASC)
);


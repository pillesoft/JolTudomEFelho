CREATE TABLE [test].[Times] (
    [TestID]    INT      NOT NULL,
    [Generated] DATETIME CONSTRAINT [DF_Times_Generated] DEFAULT (getdate()) NOT NULL,
    [Completed] DATETIME NULL,
    CONSTRAINT [PK_Times] PRIMARY KEY CLUSTERED ([TestID] ASC)
);


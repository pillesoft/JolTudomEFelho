CREATE TABLE [mobile].[Events] (
    [RowID]      INT      IDENTITY (1, 1) NOT NULL,
    [TestID]     INT      NOT NULL,
    [SequenceID] INT      NOT NULL,
    [EventID]    BIT      NOT NULL,
    [TimeStamp]  DATETIME NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([RowID] ASC),
    CONSTRAINT [FK_Events_Times] FOREIGN KEY ([TestID]) REFERENCES [test].[Times] ([TestID])
);


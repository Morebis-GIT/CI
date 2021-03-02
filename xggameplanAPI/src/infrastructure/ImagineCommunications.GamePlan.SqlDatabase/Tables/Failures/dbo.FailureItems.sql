CREATE TABLE [dbo].[FailureItems]
(
	[Id] INT NOT NULL IDENTITY(1,1),
    [FailureId] INT NOT NULL,
	[Campaign] BIGINT NULL,
    [CampaignName] NVARCHAR(256) NULL,
    [ExternalId] NVARCHAR(256) NULL,
    [Type] INT NULL,
    [Failures] BIGINT NULL,
    [SalesAreaName] NVARCHAR(256) NULL,
    CONSTRAINT PK_FailureItems PRIMARY KEY ([Id] ASC),
    CONSTRAINT [FK_FailureItems_FailureId] FOREIGN KEY (FailureId)
		REFERENCES [dbo].[Failures] (Id)
		ON DELETE CASCADE
		ON UPDATE CASCADE
)
GO

CREATE INDEX IX_FailureItems_FailureId ON [dbo].[FailureItems] (FailureId)
GO
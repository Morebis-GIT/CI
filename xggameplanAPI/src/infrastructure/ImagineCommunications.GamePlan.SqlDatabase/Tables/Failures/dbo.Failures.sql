CREATE TABLE [dbo].[Failures]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[ScenarioId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_Failures] PRIMARY KEY CLUSTERED (Id ASC)
)
GO

CREATE UNIQUE INDEX IX_Failures_ScenarioId ON [dbo].[Failures]([ScenarioId])
GO

-- =============================================
-- Author		: Adnane REZZOUK
-- Create date	: 30/09/2018
-- Description	: Ajouter column Statut dans la table Entreprise

-- =============================================

GO
	 PRINT N' Ajouter column Statut dans la table Entreprise';
GO

IF NOT EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = N'dbo' AND TABLE_NAME = N'Entreprise' AND COLUMN_NAME = N'Statut')
BEGIN
	ALTER TABLE [dbo].[Entreprise] ADD [Statut] nvarchar(10);
END
GO

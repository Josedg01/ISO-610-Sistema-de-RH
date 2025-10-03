CREATE TABLE [dbo].[Puestos]
(
	[Id] INT  NOT NULL PRIMARY KEY, 
    [Nombre] VARCHAR(64) NOT NULL, 
    [NivelDeRiesgo] SMALLINT NOT NULL, 
    [MinimoSalario] MONEY NOT NULL DEFAULT 14161.00, 
    [MaximoSalario] INT NOT NULL
)

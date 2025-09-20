CREATE TABLE [dbo].[Departamentos]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Nombre] NVARCHAR(128) NOT NULL, 
    [UbicacionFisica] NVARCHAR(128) NOT NULL, 
    [idResponsableArea] INT NOT NULL
)

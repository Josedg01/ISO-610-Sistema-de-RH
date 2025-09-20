CREATE TABLE [dbo].[Empleados]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Cedula] NCHAR(11) NOT NULL, 
    [Nombre] VARCHAR(64) NOT NULL, 
    [SalarioMensual] NUMERIC NOT NULL, 
    [idDepartamento] INT NOT NULL, 
    [idPuesto] INT NOT NULL, 
    [idNomina] INT NOT NULL 
)

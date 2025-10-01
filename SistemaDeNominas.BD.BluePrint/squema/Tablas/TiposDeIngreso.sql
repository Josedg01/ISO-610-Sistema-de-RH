CREATE TABLE [dbo].[TiposDeIngreso]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Nombre] NVARCHAR(64) NOT NULL, 
    [idEmpleado] INT NOT NULL, 
    [Estado] NVARCHAR(64) NOT NULL
)
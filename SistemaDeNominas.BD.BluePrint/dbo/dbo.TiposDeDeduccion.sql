CREATE TABLE [dbo].[TiposDeDeduccion]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Nombre] NVARCHAR(64) NOT NULL, 
    [Descripcion] NVARCHAR(256) NULL,
    [MontoFijo] DECIMAL(18, 2) NULL,
    [Porcentaje] DECIMAL(5, 2) NULL, -- Porcentaje del salario
    [Estado] NVARCHAR(64) NOT NULL DEFAULT 'Activo'
)
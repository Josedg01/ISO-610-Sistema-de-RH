CREATE TABLE [dbo].[Nominas]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [FechaInicio] DATE NOT NULL, 
    [FechaFin] DATE NOT NULL, 
    [FechaCreacion] DATETIME NOT NULL DEFAULT GETDATE(),
    [Estado] NVARCHAR(50) NOT NULL DEFAULT 'Calculada', -- 'Calculada', 'Pagada'
    [TotalCalculado] DECIMAL(18, 2) NOT NULL DEFAULT 0.00
)
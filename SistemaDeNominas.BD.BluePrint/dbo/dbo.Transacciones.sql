CREATE TABLE [dbo].[Transacciones]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [idEmpleado] INT NOT NULL,
    [Tipo] NVARCHAR(50) NOT NULL, -- 'Ingreso' o 'Deduccion'
    [ConceptoId] INT NOT NULL, -- El ID de TiposDeIngreso o TiposDeDeduccion
    [Descripcion] NVARCHAR(256) NULL, -- Ej: "Bono por ventas mes de Octubre"
    [Monto] DECIMAL(18, 2) NOT NULL,
    [Fecha] DATE NOT NULL,
    [Estado] NVARCHAR(50) NOT NULL DEFAULT 'Pendiente', -- 'Pendiente' o 'Procesada'
    CONSTRAINT [FK_Transacciones_Empleados] FOREIGN KEY ([idEmpleado]) REFERENCES [dbo].[Empleados]([Id])
)
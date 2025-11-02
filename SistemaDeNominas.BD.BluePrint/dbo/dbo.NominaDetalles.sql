CREATE TABLE [dbo].[NominaDetalles]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [idNomina] INT NOT NULL,
    [idEmpleado] INT NOT NULL,
    [SalarioBase] DECIMAL(18, 2) NOT NULL,
    [TotalIngresos] DECIMAL(18, 2) NOT NULL,
    [TotalDeducciones] DECIMAL(18, 2) NOT NULL,
    [NetoAPagar] DECIMAL(18, 2) NOT NULL,
    CONSTRAINT [FK_NominaDetalles_Nominas] FOREIGN KEY ([idNomina]) REFERENCES [dbo].[Nominas]([Id]),
    CONSTRAINT [FK_NominaDetalles_Empleados] FOREIGN KEY ([idEmpleado]) REFERENCES [dbo].[Empleados]([Id])
)
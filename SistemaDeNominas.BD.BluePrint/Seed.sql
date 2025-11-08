/*********************************************************************************
 * Archivo  : Seed.sql
 * Abstract : Inserta datos de prueba (dummy data) para todas las tablas.
 * Este script es IDEMPOTENTE: se puede ejecutar múltiples veces.
 * Limpia los datos existentes antes de insertar los nuevos.
 ********************************************************************************/

-- Usar la base de datos correcta
USE Nomina;
GO

-- Iniciar una transacción para asegurar que todo se ejecute correctamente
BEGIN TRANSACTION;
GO

PRINT 'Iniciando inserción de datos de prueba...';

-- 1. Deshabilitar todos los constraints para permitir la limpieza
EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
PRINT 'Constraints deshabilitados.';
GO

-- 2. Limpiar tablas en orden inverso de dependencia
PRINT 'Limpiando tablas existentes...';
DELETE FROM [dbo].[NominaDetalles];
DELETE FROM [dbo].[Nominas];
DELETE FROM [dbo].[Transacciones];
DELETE FROM [dbo].[TiposDeIngreso];
DELETE FROM [dbo].[TiposDeDeduccion];
DELETE FROM [dbo].[Empleados];
DELETE FROM [dbo].[Puestos];
DELETE FROM [dbo].[Departamentos];
GO

-- 3. Resetear los contadores de identidad (IDENTITY)
PRINT 'Reseteando contadores de identidad...';
DBCC CHECKIDENT ('[dbo].[NominaDetalles]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Nominas]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Transacciones]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[TiposDeDeduccion]', RESEED, 0);
DBCC CHECKIDENT ('[dbo].[Empleados]', RESEED, 0);
GO


-- 4. Insertar Departamentos
-- (Tu esquema no usa IDENTITY, así que insertamos IDs manualmente)
INSERT INTO [dbo].[Departamentos] ([Id], [Nombre], [UbicacionFisica], [idResponsableArea])
VALUES
(1, 'Tecnología', 'Piso 3, Ala Norte', 1), -- Se asume que el Empleado 1 será el responsable
(2, 'Recursos Humanos', 'Piso 2, Ala Sur', 3), -- Se asume que el Empleado 3 será el responsable
(3, 'Contabilidad', 'Piso 2, Ala Norte', 4);
PRINT 'Departamentos creados.';
GO

-- 5. Insertar Puestos
-- (Tu esquema no usa IDENTITY)
INSERT INTO [dbo].[Puestos] ([Nombre], [NivelDeRiesgo], [MinimoSalario], [MaximoSalario])
VALUES
('Gerente de TI', 3, 150000.00, 250000.00),
('Desarrollador Senior', 2, 90000.00, 160000.00),
('Analista de RRHH', 1, 45000.00, 75000.00),
('Contador Senior', 2, 55000.00, 95000.00);
PRINT 'Puestos creados.';
GO

-- 6. Insertar Empleados
-- (Tu esquema SÍ usa IDENTITY)
SET IDENTITY_INSERT [dbo].[Empleados] ON;
INSERT INTO [dbo].[Empleados] ([id], [Cedula], [Nombre], [idDepartamento], [idPuesto], [SalarioMensual], [idNomina])
VALUES
(1, '00112345671', 'Ana Rodríguez', 1, 1, 175000.00, 1),
(2, '00112345672', 'Juan Pérez', 1, 2, 120000.00, 1),
(3, '00112345673', 'María López', 2, 3, 55000.00, 1),
(4, '00112345674', 'Carlos Gómez', 3, 4, 60000.00, 1);
SET IDENTITY_INSERT [dbo].[Empleados] OFF;
PRINT 'Empleados creados.';
GO

-- 7. Insertar Tipos de Deducción (Globales)
-- (El esquema que creamos SÍ usa IDENTITY)
SET IDENTITY_INSERT [dbo].[TiposDeDeduccion] ON;
INSERT INTO [dbo].[TiposDeDeduccion] ([id], [Nombre], [Descripcion], [MontoFijo], [Porcentaje], [Estado])
VALUES
(1, 'AFP (Aseg. Fondo de Pensiones)', 'Descuento de ley TSS 2.87%', NULL, 2.87, 'Activo'),
(2, 'SFS (Seguro Familiar de Salud)', 'Descuento de ley TSS 3.04%', NULL, 3.04, 'Activo'),
(3, 'Préstamo Interno', 'Deducción por préstamo de empleado', NULL, NULL, 'Activo'),
(4, 'Adelanto de Salario', 'Deducción por adelanto', NULL, NULL, 'Activo'),
(5, 'ISR (Impuesto Sobre la Renta)', 'Retención de ISR según escala salarial', NULL, 0.00, 'Activo'); -- Se deja en 0, el cálculo de nómina debería hacerlo
SET IDENTITY_INSERT [dbo].[TiposDeDeduccion] OFF;
PRINT 'Tipos de Deducción creados.';
GO

-- 8. Insertar Tipos de Ingreso (Por Empleado, según tu esquema)
-- (Tu esquema no usa IDENTITY)
INSERT INTO [dbo].[TiposDeIngreso] ([Id], [Nombre], [idEmpleado], [Estado])
VALUES
(1, 'Comisión por Ventas', 1, 'Activo'), 
(2, 'Bono por Desempeño', 1, 'Activo'), 
(3, 'Comisión por Ventas', 2, 'Activo'), 
(4, 'Bono por Desempeño', 2, 'Activo'), 
(5, 'Bono por Desempeño', 3, 'Activo'), 
(6, 'Bono por Horas Extra', 4, 'Activo'); 
PRINT 'Tipos de Ingreso creados.';
GO

-- 9. Insertar Transacciones (Pendientes para la nómina)
DECLARE @Today DATE = GETDATE();
SET IDENTITY_INSERT [dbo].[Transacciones] ON;
INSERT INTO [dbo].[Transacciones] ([id], [idEmpleado], [Tipo], [ConceptoId], [Descripcion], [Monto], [Fecha], [Estado])
VALUES
-- Ingresos para la nómina actual
(1, 1, 'Ingreso', 2, 'Bono proyecto X finalizado', 25000.00, @Today, 'Pendiente'),
(2, 2, 'Ingreso', 3, 'Comisiones Octubre', 15000.00, @Today, 'Pendiente'),
(3, 4, 'Ingreso', 6, 'Horas extra cierre de mes', 7500.00, @Today, 'Pendiente'),

-- Deducciones para la nómina actual
(4, 2, 'Deduccion', 3, 'Cuota 1/3 Préstamo PC', 5000.00, @Today, 'Pendiente'),
(5, 3, 'Deduccion', 4, 'Adelanto del día 10', 2000.00, @Today, 'Pendiente');
SET IDENTITY_INSERT [dbo].[Transacciones] OFF;
PRINT 'Transacciones pendientes creadas.';
GO

-- 10. Habilitar todos los constraints
EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'
PRINT 'Constraints habilitados.';
GO

-- Confirmar la transacción
COMMIT TRANSACTION;
PRINT '¡Datos dummy insertados correctamente!';
GO
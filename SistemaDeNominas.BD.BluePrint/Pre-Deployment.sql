/*********************************************************************************
 *  Fecha    : 2025-10-03
 *  Archivo  : Pre-Deployment.sql
 *  Autor    : MBCX
 *                                                                                        
 *  Abstract :                                                                            
 *  Script que initializa el ambiente y crea todas las tablas (y BD) necesarios
 ********************************************************************************/


-- Crea la base de datos si no existe.
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Nomina')
BEGIN
	CREATE DATABASE Nomina;
	PRINT 'Base de datos Nomina creada';
END

USE Nomina;

-- Crea cada una de las tablas.
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'Departamentos'
)
BEGIN
    CREATE TABLE [dbo].[Departamentos]
    (
	    [Id] INT NOT NULL PRIMARY KEY, 
        [Nombre] NVARCHAR(128) NOT NULL, 
        [UbicacionFisica] NVARCHAR(128) NOT NULL, 
        [idResponsableArea] INT NOT NULL
    )
    PRINT 'Tabla Departamentos creada'
END

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'Empleados'
)
BEGIN
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
    PRINT 'Tabla Empleados creada'
END

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'Puestos'
)
BEGIN
    CREATE TABLE [dbo].[Puestos]
    (
	    [Id] INT NOT NULL PRIMARY KEY, 
        [Nombre] VARCHAR(64) NOT NULL, 
        [NivelDeRiesgo] SMALLINT NOT NULL, 
        [MinimoSalario] MONEY NOT NULL DEFAULT 14161.00, 
        [MaximoSalario] INT NOT NULL
    )
    PRINT 'Tabla Puestos creada'
END

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'TiposDeIngreso'
)
BEGIN
    CREATE TABLE [dbo].[TiposDeIngreso]
    (
	    [Id] INT NOT NULL PRIMARY KEY, 
        [Nombre] NVARCHAR(64) NOT NULL, 
        [idEmpleado] INT NOT NULL, 
        [Estado] NVARCHAR(64) NOT NULL
    )
    PRINT 'Tabla TiposDeIngreso creada'
END

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'TiposDeDeduccion'
)
BEGIN
    CREATE TABLE [dbo].[TiposDeDeduccion]
    (
	    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
        [Nombre] NVARCHAR(64) NOT NULL, 
        [Descripcion] NVARCHAR(256) NULL,
        [MontoFijo] DECIMAL(18, 2) NULL,
        [Porcentaje] DECIMAL(5, 2) NULL,
        [Estado] NVARCHAR(64) NOT NULL DEFAULT 'Activo'
    )
    PRINT 'Tabla TiposDeDeduccion creada'
END

IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_SCHEMA = 'dbo'
      AND TABLE_NAME = 'Transacciones'
)
BEGIN
    CREATE TABLE [dbo].[Transacciones]
    (
	    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
        [idEmpleado] INT NOT NULL,
        [Tipo] NVARCHAR(50) NOT NULL, -- 'Ingreso' o 'Deduccion'
        [ConceptoId] INT NOT NULL, 
        [Descripcion] NVARCHAR(256) NULL,
        [Monto] DECIMAL(18, 2) NOT NULL,
        [Fecha] DATE NOT NULL,
        [Estado] NVARCHAR(50) NOT NULL DEFAULT 'Pendiente',
        CONSTRAINT [FK_Transacciones_Empleados] FOREIGN KEY ([idEmpleado]) REFERENCES [dbo].[Empleados]([Id])
    )
    PRINT 'Tabla Transacciones creada'
END
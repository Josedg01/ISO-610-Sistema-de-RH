/*********************************************************************************
 *  Fecha    : 2025-10-03
 *  Archivo  : Pre-Deployment.sql
 *  Autor    : MBCX
 *                                                                                        
 *  Abstract :                                                                            
 *  Script que initializa el ambiente y crea todas las tablas (y BD) necesarios
 ********************************************************************************/


-- Crea la base de datos si no existe.
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RecursosHumanos')
BEGIN
	CREATE DATABASE RecursosHumanos;
	PRINT 'Base de datos RecursosHumanos creada';
END

USE RecursosHumanos;

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
SET NOCOUNT ON;
GO

-- =============================================
-- 1. BASE DE DATOS: Auth
-- =============================================
USE [auth_csf_db];
GO

IF OBJECT_ID('dbo.AppUsers', 'U') IS NOT NULL DROP TABLE dbo.AppUsers;
GO

CREATE TABLE dbo.AppUsers (
    Id               INT IDENTITY(1,1) NOT NULL,
    Username         NVARCHAR(100)     NOT NULL,
    FullName         NVARCHAR(200)     NOT NULL,
    [Role]           NVARCHAR(50)      NOT NULL,
    PasswordHash     NVARCHAR(MAX)     NOT NULL,
    CreatedAtUtc     DATETIME2         NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_AppUsers PRIMARY KEY (Id),
    CONSTRAINT UQ_AppUsers_Username UNIQUE (Username)
);
GO


-- =============================================
-- 1. BASE DE DATOS: Compras
-- =============================================
USE [purchase_csf_db];
GO

IF OBJECT_ID('dbo.CompraDet', 'U') IS NOT NULL DROP TABLE dbo.CompraDet;
IF OBJECT_ID('dbo.CompraCab', 'U') IS NOT NULL DROP TABLE dbo.CompraCab;
GO

CREATE TABLE dbo.CompraCab (
    Id_CompraCab INT IDENTITY(1,1) NOT NULL,
    FecRegistro  DATETIME2         NOT NULL DEFAULT SYSUTCDATETIME(),
    SubTotal     DECIMAL(18,2)     NOT NULL,
    Igv          DECIMAL(18,2)     NOT NULL,
    Total        DECIMAL(18,2)     NOT NULL,
    Status       INT               NOT NULL DEFAULT 1,
    CONSTRAINT PK_CompraCab PRIMARY KEY (Id_CompraCab)
);
GO

CREATE TABLE dbo.CompraDet (
    Id_CompraDet INT IDENTITY(1,1) NOT NULL,
    Id_CompraCab INT               NOT NULL,
    Id_Producto  INT               NOT NULL,
    Cantidad     DECIMAL(18,2)     NOT NULL,
    Precio       DECIMAL(18,2)     NOT NULL,
    SubTotal     DECIMAL(18,2)     NOT NULL,
    Igv          DECIMAL(18,2)     NOT NULL,
    Total        DECIMAL(18,2)     NOT NULL,
    CONSTRAINT PK_CompraDet PRIMARY KEY (Id_CompraDet),
    CONSTRAINT FK_CompraDet_CompraCab FOREIGN KEY (Id_CompraCab) REFERENCES dbo.CompraCab(Id_CompraCab)
);
GO

-- =============================================
-- 2. BASE DE DATOS: Productos
-- =============================================
USE [product_csf_db]; -- <--- Cambia esto por el nombre real
GO

IF OBJECT_ID('dbo.ProductPriceUpdateLog', 'U') IS NOT NULL DROP TABLE dbo.ProductPriceUpdateLog;
IF OBJECT_ID('dbo.OutboxMessages', 'U')         IS NOT NULL DROP TABLE dbo.OutboxMessages;
IF OBJECT_ID('dbo.Productos', 'U')            IS NOT NULL DROP TABLE dbo.Productos;
GO

CREATE TABLE dbo.Productos (
    Id_producto     INT IDENTITY(1,1)   NOT NULL,
    Nombre_producto NVARCHAR(200)       NOT NULL,
    NroLote         NVARCHAR(100)       NOT NULL,
    Fec_registro    DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
    Costo           DECIMAL(18, 2)      NOT NULL,
    PrecioVenta     DECIMAL(18, 2)      NOT NULL,

    CONSTRAINT PK_Productos PRIMARY KEY (Id_producto)
);
GO

CREATE TABLE dbo.ProductPriceUpdateLog (
    Id              INT IDENTITY(1,1)   NOT NULL,
    PurchaseId      INT                 NOT NULL,
    ProductId       INT                 NOT NULL,
    OldCosto        DECIMAL(18, 2)      NOT NULL,
    OldPrecioVenta  DECIMAL(18, 2)      NOT NULL,
    NewCosto        DECIMAL(18, 2)      NOT NULL,
    NewPrecioVenta  DECIMAL(18, 2)      NOT NULL,
    RolledBack      BIT                 NOT NULL DEFAULT 0,
    CreatedAtUtc    DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
    RolledBackAtUtc DATETIME2           NULL,

    CONSTRAINT PK_ProductPriceUpdateLog PRIMARY KEY (Id),
    CONSTRAINT UQ_ProductPriceUpdateLog UNIQUE (PurchaseId, ProductId),
);
GO

CREATE TABLE dbo.OutboxMessages (
    Id              UNIQUEIDENTIFIER    NOT NULL,
    EventType       NVARCHAR(200)       NOT NULL,
    Payload         NVARCHAR(MAX)       NOT NULL,
    OccurredAtUtc   DATETIME2           NOT NULL DEFAULT SYSUTCDATETIME(),
    ProcessedAtUtc  DATETIME2           NULL,
    ErrorMessage    NVARCHAR(1000)      NULL,

    CONSTRAINT PK_OutboxMessages PRIMARY KEY (Id)
);
GO

-- =============================================
-- 3. BASE DE DATOS: Movimientos
-- =============================================
USE [movement_csf_db];
GO

IF OBJECT_ID('dbo.MovementCompensationLog', 'U') IS NOT NULL DROP TABLE dbo.MovementCompensationLog;
IF OBJECT_ID('dbo.MovimientoDet', 'U') IS NOT NULL DROP TABLE dbo.MovimientoDet;
IF OBJECT_ID('dbo.MovimientoCab', 'U') IS NOT NULL DROP TABLE dbo.MovimientoCab;
GO

CREATE TABLE dbo.MovimientoCab
(
    Id_MovimientoCab INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Fec_registro DATETIME2 NOT NULL,
    Id_TipoMovimiento INT NOT NULL,
    Id_DocumentoOrigen INT NOT NULL,
    IsCompensation BIT NOT NULL
);
GO

CREATE TABLE dbo.MovimientoDet
(
    Id_MovimientoDet INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_movimientocab INT NOT NULL,
    Id_Producto INT NOT NULL,
    Cantidad DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_MovimientoDet_MovimientoCab
        FOREIGN KEY (Id_movimientocab) REFERENCES dbo.MovimientoCab(Id_MovimientoCab)
);
GO

CREATE TABLE dbo.MovementCompensationLog
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    IdDocumentoOrigen INT NOT NULL,
    OriginalMovementType INT NOT NULL,
    CompensationMovementType INT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CONSTRAINT UQ_MovementCompensationLog
        UNIQUE (IdDocumentoOrigen, OriginalMovementType, CompensationMovementType)
);
GO

-- =============================================
-- 3. BASE DE DATOS: Orquestador
-- =============================================
USE [orchestrator_csf_db];
GO

IF OBJECT_ID('dbo.SagaStep', 'U') IS NOT NULL DROP TABLE dbo.SagaStep;
IF OBJECT_ID('dbo.SagaInstance', 'U') IS NOT NULL DROP TABLE dbo.SagaInstance;
GO

CREATE TABLE dbo.SagaInstance
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    CorrelationId UNIQUEIDENTIFIER NOT NULL,
    SagaType INT NOT NULL,
    Status INT NOT NULL,
    CurrentStep NVARCHAR(200) NOT NULL,
    ErrorMessage NVARCHAR(2000) NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    UpdatedAtUtc DATETIME2 NOT NULL
);
GO

CREATE TABLE dbo.SagaStep
(
    Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SagaInstanceId INT NOT NULL,
    StepName NVARCHAR(200) NOT NULL,
    Status INT NOT NULL,
    RequestPayload NVARCHAR(MAX) NULL,
    ResponsePayload NVARCHAR(MAX) NULL,
    ErrorMessage NVARCHAR(2000) NULL,
    IsCompensation BIT NOT NULL,
    CreatedAtUtc DATETIME2 NOT NULL,
    CONSTRAINT FK_SagaStep_SagaInstance FOREIGN KEY (SagaInstanceId)
        REFERENCES dbo.SagaInstance(Id)
);
GO

-- =============================================
-- 3. BASE DE DATOS: Ventas
-- =============================================
USE [orchestrator_csf_db];
GO

IF OBJECT_ID('dbo.VentaDet', 'U') IS NOT NULL DROP TABLE dbo.VentaDet;
IF OBJECT_ID('dbo.VentaCab', 'U') IS NOT NULL DROP TABLE dbo.VentaCab;
IF OBJECT_ID('dbo.ProcessedIntegrationEvents', 'U') IS NOT NULL DROP TABLE dbo.ProcessedIntegrationEvents;
IF OBJECT_ID('dbo.ProductPricingSnapshot', 'U') IS NOT NULL DROP TABLE dbo.ProductPricingSnapshot;
GO


CREATE TABLE dbo.VentaCab
(
    Id_VentaCab INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    FecRegistro DATETIME2 NOT NULL,
    SubTotal DECIMAL(18,2) NOT NULL,
    Igv DECIMAL(18,2) NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    Status INT NOT NULL
);
GO

CREATE TABLE dbo.VentaDet
(
    Id_VentaDet INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Id_VentaCab INT NOT NULL,
    Id_producto INT NOT NULL,
    Cantidad DECIMAL(18,2) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    Sub_Total DECIMAL(18,2) NOT NULL,
    Igv DECIMAL(18,2) NOT NULL,
    Total DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_VentaDet_VentaCab
        FOREIGN KEY (Id_VentaCab) REFERENCES dbo.VentaCab(Id_VentaCab)
);
GO

CREATE TABLE ProductPricingSnapshot
(
    IdProducto INT NOT NULL PRIMARY KEY,
    NombreProducto NVARCHAR(200) NOT NULL,
    PrecioVenta DECIMAL(18,2) NOT NULL,
    LastUpdatedUtc DATETIME2 NOT NULL
);
GO

CREATE TABLE ProcessedIntegrationEvents
(
    EventId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    EventType NVARCHAR(200) NOT NULL,
    ProcessedAtUtc DATETIME2 NOT NULL
);
GO
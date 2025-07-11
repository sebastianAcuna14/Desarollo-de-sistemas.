USE master;

-- Crear la base de datos
CREATE DATABASE FerreteriaBD;
GO

USE FerreteriaBD;
GO

-- Tabla: Cliente
CREATE TABLE Cliente (
    Cedula VARCHAR(15) PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    Telefonos VARCHAR(50),
    Correo VARCHAR(100)
);
GO

-- Tabla: Rol
CREATE TABLE Rol (
    IdRol INT IDENTITY(1,1) PRIMARY KEY,
    Nombre_rol VARCHAR(50),
    Permisos VARCHAR(255)
);
GO

-- Tabla: Empleado
CREATE TABLE Empleado (
    IdEmpleado INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    Telefono VARCHAR(20),
    Correo VARCHAR(100),
    IdRol INT,
    FOREIGN KEY (IdRol) REFERENCES Rol(IdRol)
);
GO

-- Tabla: Proveedor
CREATE TABLE Proveedor (
    IdProveedor INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100),
    Telefono VARCHAR(20),
    Correo VARCHAR(100),
    Direccion VARCHAR(200)
);
GO

-- Tabla: Inventario
CREATE TABLE Inventario (
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100),
    Descripcion VARCHAR(255),
    Cantidad INT,
	Categoria VARCHAR(100),
    Precio DECIMAL(10,2),
    IdProveedor INT,
    FOREIGN KEY (IdProveedor) REFERENCES Proveedor(IdProveedor)
);
GO

-- Tabla: Reparacion
CREATE TABLE Reparacion (
    IdReparacion INT IDENTITY(1,1) PRIMARY KEY,
    Fecha_Salida DATE,
    Fecha_Ingreso DATE,
    Descripcion VARCHAR(255),
    Estado VARCHAR(50),
    IdCliente VARCHAR(15),
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula)
);
GO

-- Tabla: Reporte
CREATE TABLE Reporte (
    IdReporte INT IDENTITY(1,1) PRIMARY KEY,
    Fecha_Salida DATE,
    Fecha_Ingreso DATE,
    Mensaje VARCHAR(255),
    Estado VARCHAR(50),
    IdCliente VARCHAR(15),
    IdProducto INT,
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula),
    FOREIGN KEY (IdProducto) REFERENCES Inventario(IdProducto)
);
GO

-- Tabla: Notificacion
CREATE TABLE Notificacion (
    IdNotificacion INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente VARCHAR(15),
    IdRepara INT,
    Fecha_Envio DATE,
    Mensaje VARCHAR(255),
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula),
    FOREIGN KEY (IdRepara) REFERENCES Reparacion(IdReparacion)
);
GO
-- Tabla: Pedido
CREATE TABLE Pedido (
    Id int PRIMARY KEY,
    Nombre_Producto VARCHAR(50),
    Numero_Pedido VARCHAR(50),
    Cantidad int,
    FechaPedido Date,
	Precio decimal,
	Estado VARCHAR(100)
);
GO
--Procedimientos almacenados 

--Reparacion
/*Create*/
CREATE PROCEDURE CrearReparacion
    @Fecha_Ingreso DATE,
    @Fecha_Salida DATE,
    @Descripcion VARCHAR(255),
    @Estado VARCHAR(50),
    @IdCliente VARCHAR(15)
AS
BEGIN
    INSERT INTO Reparacion (Fecha_Ingreso, Fecha_Salida, Descripcion, Estado, IdCliente)
    VALUES (@Fecha_Ingreso, @Fecha_Salida, @Descripcion, @Estado, @IdCliente)
END



/*Tabla de index mostrar las reparaciones*/
CREATE PROCEDURE ObtenerReparaciones
AS
BEGIN
    SELECT * FROM Reparacion
END

DROP TABLE Reparacion;

CREATE PROCEDURE ConsultarReparacionID
    @IdReparacion INT
AS
BEGIN
    SELECT * FROM Reparacion WHERE IdReparacion = @IdReparacion
END



/*Update*/
CREATE PROCEDURE ActualizarReparacion
    @IdReparacion INT,
    @Fecha_Ingreso DATE,
    @Fecha_Salida DATE,
    @Descripcion VARCHAR(255),
    @Estado VARCHAR(50),
    @IdCliente VARCHAR(15)
AS
BEGIN
    UPDATE Reparacion
    SET Fecha_Ingreso = @Fecha_Ingreso,
        Fecha_Salida = @Fecha_Salida,
        Descripcion = @Descripcion,
        Estado = @Estado,
        IdCliente = @IdCliente
    WHERE IdReparacion = @IdReparacion
END



/*Delete*/
CREATE PROCEDURE EliminarReparacion
    @IdReparacion INT
AS
BEGIN
    DELETE FROM Reparacion 
	WHERE IdReparacion = @IdReparacion
END;


-- Consultas de prueba
SELECT * FROM Cliente;

SELECT * FROM Inventario;

SELECT * FROM Reparacion;
///////////////////////////

--Precedimientos almacenados PEDIDO
CREATE PROCEDURE Crear_Pedido 
	-- Add the parameters for the stored procedure here
    @Id int, @Nombre_Producto VARCHAR, @Numero_Pedido VARCHAR, @Cantidad int, @FechaPedido Date, @Precio decimal, @Estado VARCHAR
AS
BEGIN
	INSERT INTO dbo.Pedido(Id ,Nombre_Producto , Numero_Pedido , Cantidad , FechaPedido , Precio , Estado)
	values( @Id , @Nombre_Producto , @Numero_Pedido , @Cantidad , @FechaPedido , @Precio , @Estado )
END
GO
/////
-- Procedimiento almacenado PEDIDO
CREATE PROCEDURE Crear_Pedido 
    @Id INT, 
    @Nombre_Producto VARCHAR(100), 
    @Numero_Pedido VARCHAR(50), 
    @Cantidad INT, 
    @FechaPedido DATE, 
    @Precio DECIMAL(10,2), 
    @Estado VARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.Pedido(Id, Nombre_Producto, Numero_Pedido, Cantidad, FechaPedido, Precio, Estado)
    VALUES (@Id, @Nombre_Producto, @Numero_Pedido, @Cantidad, @FechaPedido, @Precio, @Estado)
END
GO

-- Procedimiento: Crear Pedido
-- ===============================
CREATE OR ALTER PROCEDURE Crear_Pedido 
    @Id INT, 
    @Nombre_Producto VARCHAR(50), 
    @Numero_Pedido VARCHAR(50), 
    @Cantidad INT, 
    @FechaPedido DATE, 
    @Precio DECIMAL(10,2), 
    @Estado VARCHAR(100)
AS
BEGIN
    INSERT INTO Pedido (Id, Nombre_Producto, Numero_Pedido, Cantidad, FechaPedido, Precio, Estado)
    VALUES (@Id, @Nombre_Producto, @Numero_Pedido, @Cantidad, @FechaPedido, @Precio, @Estado);
END;
GO

-- ===============================
-- Procedimiento: Obtener todos los pedidos
-- ===============================
CREATE OR ALTER PROCEDURE Obtener_Pedidos
AS
BEGIN
    SELECT * FROM Pedido;
END;
GO

-- ===============================
-- Procedimiento: Consultar pedido por ID
-- ===============================
CREATE OR ALTER PROCEDURE Consultar_Pedido_Por_Id
    @IdPedido INT
AS
BEGIN
    SELECT * FROM Pedido WHERE Id = @IdPedido;
END;
GO

-- ===============================
-- Procedimiento: Actualizar Pedido
-- ===============================
CREATE OR ALTER PROCEDURE Actualizar_Pedido
    @Id INT,
    @Nombre_Producto VARCHAR(50),
    @Numero_Pedido VARCHAR(50),
    @Cantidad INT,
    @FechaPedido DATE,
    @Precio DECIMAL(10,2),
    @Estado VARCHAR(100)
AS
BEGIN
    UPDATE Pedido
    SET Nombre_Producto = @Nombre_Producto,
        Numero_Pedido = @Numero_Pedido,
        Cantidad = @Cantidad,
        FechaPedido = @FechaPedido,
        Precio = @Precio,
        Estado = @Estado
    WHERE Id = @Id;
END;
GO

-- ===============================
-- Procedimiento: Eliminar Pedido
-- ===============================
CREATE OR ALTER PROCEDURE Eliminar_Pedido
    @IdPedido INT
AS
BEGIN
    DELETE FROM Pedido WHERE Id = @IdPedido;
END;
GO



//////////////////////////////////////JOSHUA
DROP TABLE IF EXISTS ItemsDevueltos, Devoluciones, MetodosPago, ItemsVendidos, Venta, NotaCredito;
GO

-- Tabla de Notas de Crédito
CREATE TABLE NotaCredito (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Monto DECIMAL(18,2) NOT NULL,
    Comentario NVARCHAR(MAX) NULL
);
GO

-- Tabla principal de ventas
CREATE TABLE Venta (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL,
    NotaCreditoId INT NULL,
    FOREIGN KEY (NotaCreditoId) REFERENCES NotaCredito(Id)
);
GO

-- Tabla de productos vendidos
CREATE TABLE ItemsVendidos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VentaId INT NOT NULL,
    Producto NVARCHAR(100) NOT NULL,
    Cantidad INT NOT NULL,
    PrecioUnitario DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (VentaId) REFERENCES Venta(Id) ON DELETE CASCADE
);
GO

-- Tabla de métodos de pago
CREATE TABLE MetodosPago (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VentaId INT NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Tipo NVARCHAR(20) NOT NULL,
    FOREIGN KEY (VentaId) REFERENCES Venta(Id) ON DELETE CASCADE
);
GO

-- Tabla de devoluciones
CREATE TABLE Devoluciones (
    Id INT PRIMARY KEY IDENTITY(1,1),
    VentaId INT NOT NULL,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Motivo NVARCHAR(255),
    FOREIGN KEY (VentaId) REFERENCES Venta(Id) ON DELETE CASCADE
);
GO

-- Productos devueltos por devolución
CREATE TABLE ItemsDevueltos (
    Id INT PRIMARY KEY IDENTITY(1,1),
    DevolucionId INT NOT NULL,
    Producto NVARCHAR(100) NOT NULL,
    Cantidad INT NOT NULL,
    Observaciones NVARCHAR(255),
    FOREIGN KEY (DevolucionId) REFERENCES Devoluciones(Id) ON DELETE CASCADE
);
GO

--Precedimientos almacenados
-- Crear una venta
CREATE OR ALTER PROCEDURE CrearVenta
    @Fecha DATETIME,
    @NotaCreditoId INT = NULL
AS
BEGIN
    INSERT INTO Venta (Fecha, NotaCreditoId)
    VALUES (@Fecha, @NotaCreditoId);

    SELECT SCOPE_IDENTITY() AS NuevaVentaId;
END
GO

CREATE OR ALTER PROCEDURE ObtenerVentas
AS
BEGIN
    SELECT * FROM Venta;
END
GO


-- Consultar venta por ID
CREATE OR ALTER PROCEDURE ConsultarVentaPorId
    @Id INT
AS
BEGIN
    SELECT * FROM Venta WHERE Id = @Id;
END
GO


-- Actualizar venta
CREATE OR ALTER PROCEDURE ActualizarVenta
    @Id INT,
    @Fecha DATETIME,
    @NotaCreditoId INT = NULL
AS
BEGIN
    UPDATE Venta
    SET Fecha = @Fecha,
        NotaCreditoId = @NotaCreditoId
    WHERE Id = @Id;
END
GO


-- Eliminar venta
CREATE OR ALTER PROCEDURE EliminarVenta
    @Id INT
AS
BEGIN
    DELETE FROM Venta WHERE Id = @Id;
END
GO


-- Tabla de movimientos financieros
CREATE TABLE Finanza (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    Descripcion NVARCHAR(500) NOT NULL,
    Monto DECIMAL(10,2) NOT NULL,
    Tipo NVARCHAR(30) NOT NULL,
    FechaVencimiento DATE NULL,
    Pagada BIT NOT NULL DEFAULT 0,
    Anulada BIT NOT NULL DEFAULT 0
);
GO


-- Crear movimiento financiero
CREATE OR ALTER PROCEDURE CrearMovimientoFinanciero
    @Fecha DATETIME,
    @Descripcion NVARCHAR(500),
    @Monto DECIMAL(10,2),
    @Tipo NVARCHAR(30),
    @FechaVencimiento DATE = NULL,
    @Pagada BIT = 0,
    @Anulada BIT = 0
AS
BEGIN
    INSERT INTO Finanza (Fecha, Descripcion, Monto, Tipo, FechaVencimiento, Pagada, Anulada)
    VALUES (@Fecha, @Descripcion, @Monto, @Tipo, @FechaVencimiento, @Pagada, @Anulada);

    SELECT SCOPE_IDENTITY() AS NuevoMovimientoId;
END
GO


-- Obtener movimientos financieros no anulados
CREATE OR ALTER PROCEDURE ObtenerMovimientosFinancieros
AS
BEGIN
    SELECT * FROM Finanza
    WHERE Anulada = 0;
END
GO


-- Actualizar movimiento financiero existente
CREATE OR ALTER PROCEDURE ActualizarMovimientoFinanciero
    @Id INT,
    @Fecha DATETIME,
    @Descripcion NVARCHAR(500),
    @Monto DECIMAL(10,2),
    @Tipo NVARCHAR(30),
    @FechaVencimiento DATE = NULL,
    @Pagada BIT,
    @Anulada BIT
AS
BEGIN
    UPDATE Finanza
    SET Fecha = @Fecha,
        Descripcion = @Descripcion,
        Monto = @Monto,
        Tipo = @Tipo,
        FechaVencimiento = @FechaVencimiento,
        Pagada = @Pagada,
        Anulada = @Anulada
    WHERE Id = @Id;
END
GO


-- Marcar cuenta por cobrar como pagada (y cambiar tipo a ingreso)
CREATE OR ALTER PROCEDURE MarcarCuentaPorCobrarComoPagada
    @Id INT
AS
BEGIN
    UPDATE Finanza
    SET Pagada = 1,
        Tipo = 'INGRESO'
    WHERE Id = @Id AND Tipo = 'CUENTA_POR_COBRAR';
END
GO


-- Eliminar movimiento financiero (eliminación real)
CREATE OR ALTER PROCEDURE EliminarMovimientoFinanciero
    @Id INT
AS
BEGIN
    DELETE FROM Finanza WHERE Id = @Id;
END
GO


USE master;
GO

-- Forzar modo SINGLE_USER y cerrar conexiones abiertas
ALTER DATABASE FerreteriaBD SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

DROP DATABASE FerreteriaBD;
GO
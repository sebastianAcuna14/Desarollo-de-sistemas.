-- Crear la base de datos
CREATE DATABASE FerreteriaBD;
USE FerreteriaBD;

-- Tabla: Cliente
CREATE TABLE Cliente (
    Cedula VARCHAR(15) PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    Telefonos VARCHAR(50),
    Correo VARCHAR(100)
);

-- Tabla: Rol
CREATE TABLE Rol (
    IdRol INT AUTO_INCREMENT PRIMARY KEY,
    Nombre_rol VARCHAR(50),
    Permisos VARCHAR(255)
);

-- Tabla: Empleado
CREATE TABLE Empleado (
    IdEmpleado INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    Telefono VARCHAR(20),
    Correo VARCHAR(100),
    IdRol INT,
    FOREIGN KEY (IdRol) REFERENCES Rol(IdRol)
);

-- Tabla: Proveedor
CREATE TABLE Proveedor (
    IdProveedor INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100),
    Telefono VARCHAR(20),
    Correo VARCHAR(100),
    Direccion VARCHAR(200)
);

-- Tabla: Inventario
CREATE TABLE Inventario (
    IdProducto INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(100),
    Descripcion VARCHAR(255),
    Cantidad INT,
    Precio DECIMAL(10,2),
    IdProveedor INT,
    FOREIGN KEY (IdProveedor) REFERENCES Proveedor(IdProveedor)
);

-- Tabla: Reparacion
CREATE TABLE Reparacion (
    IdReparacion INT AUTO_INCREMENT PRIMARY KEY,
    Fecha_Salida DATE,
    Fecha_Ingreso DATE,
    Mensaje VARCHAR(255),
    Estado VARCHAR(50),
    IdCliente VARCHAR(15),
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula)
);

-- Tabla: Reporte
CREATE TABLE Reporte (
    IdReporte INT AUTO_INCREMENT PRIMARY KEY,
    Fecha_Salida DATE,
    Fecha_Ingreso DATE,
    Mensaje VARCHAR(255),
    Estado VARCHAR(50),
    IdCliente VARCHAR(15),
    IdProducto INT,
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula),
    FOREIGN KEY (IdProducto) REFERENCES Inventario(IdProducto)
);

-- Tabla: Notificacion
CREATE TABLE Notificacion (
    IdNotificacion INT AUTO_INCREMENT PRIMARY KEY,
    IdCliente VARCHAR(15),
    IdRepara INT,
    Fecha_Envio DATE,
    Mensaje VARCHAR(255),
    FOREIGN KEY (IdCliente) REFERENCES Cliente(Cedula),
    FOREIGN KEY (IdRepara) REFERENCES Reparacion(IdReparacion)
);

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

CREATE TABLE Producto (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre_Producto NVARCHAR(100) NOT NULL,
    Categoria NVARCHAR(100) NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad >= 0),
    Proveedor NVARCHAR(100) NOT NULL,
    Precio DECIMAL(10, 2) NOT NULL CHECK (Precio > 0)
);


-- Tabla Carrito
CREATE TABLE Carrito (
    Id INT IDENTITY PRIMARY KEY,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    Nombre_Producto NVARCHAR(100) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL
);

-- Procedimiento para agregar al carrito
CREATE OR ALTER PROCEDURE AgregarAlCarrito
    @ProductoId INT,
    @Cantidad INT,
    @Nombre_Producto NVARCHAR(100),  
    @Precio DECIMAL(18, 2)
AS
BEGIN
    INSERT INTO Carrito (ProductoId, Cantidad, Nombre_Producto, Precio)
    VALUES (@ProductoId, @Cantidad, @Nombre_Producto, @Precio)
END

-- Procedimiento para obtener carrito
CREATE OR ALTER PROCEDURE ObtenerCarrito
AS
BEGIN
    SELECT 
        Id,
        ProductoId,
        Nombre_Producto,
        Precio,
        Cantidad,
        (Precio * Cantidad) AS Subtotal
    FROM Carrito
END

-- Procedimiento para eliminar un producto del carrito
CREATE OR ALTER PROCEDURE EliminarDelCarrito
    @Id INT
AS
BEGIN
    DELETE FROM Carrito WHERE Id = @Id
END

-- Procedimiento para vaciar el carrito
CREATE OR ALTER PROCEDURE LimpiarCarrito
AS
BEGIN
    DELETE FROM Carrito
END

-- Procedimiento para actualizar la cantidad
CREATE OR ALTER PROCEDURE ActualizarCantidadCarrito
    @Id INT,
    @Cantidad INT
AS
BEGIN
    UPDATE Carrito
    SET Cantidad = @Cantidad
    WHERE Id = @Id
END

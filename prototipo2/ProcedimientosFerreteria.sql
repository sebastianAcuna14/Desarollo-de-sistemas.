/***********************************      Procedimientos almacenados del sistema       *******************/
USE FerreteriaBD;
GO

-- Login

CREATE PROCEDURE RegistrarCliente
    @Nombre NVARCHAR(100),
    @Apellido NVARCHAR(100),
    @Cedula NVARCHAR(15),
    @Correo NVARCHAR(100),
    @Telefonos NVARCHAR(50),
    @Contrasena NVARCHAR(50)
AS
BEGIN
    INSERT INTO dbo.Cliente (Cedula, Nombre, Apellido, Telefonos, Correo, Contrasena)
    VALUES (@Cedula, @Nombre, @Apellido, @Telefonos, @Correo, @Contrasena);
END;
GO



Create PROCEDURE ValidarInicioSesionCliente
	@Correo varchar(100),
	@Contrasena varchar(50)
AS
BEGIN

	SELECT	Cedula,
			idCliente,
			Nombre,
			Apellido,
			Correo,
			Telefonos,
			Cedula




	  FROM	dbo.Cliente
	WHERE	Correo = @Correo
		AND Contrasena = @Contrasena
		
END
GO

-----EMPLEADO
Create PROCEDURE ValidarInicioSesionEmpleado
	@Correo varchar(100),
	@Contrasena varchar(50)
AS
BEGIN

	SELECT	
			IdEmpleado,
			Nombre,
			Apellido,
			Correo,
			Telefono
			




	  FROM	EMPLEADO
	WHERE	Correo = @Correo
		AND contrasena = @Contrasena
		
END
GO

alter PROCEDURE RegistrarEmpleado
    @Nombre VARCHAR(100),
    @Apellido VARCHAR(100),
    @Correo VARCHAR(100),
    @Telefono VARCHAR(20),
    @Contrasena VARCHAR(50)
	
AS
BEGIN
    INSERT INTO Empleado( Nombre, Apellido, Correo, Telefono, Contrasena)
    VALUES (@Nombre,@Apellido, @Telefono, @Correo, @Contrasena);
END;
GO

CREATE PROCEDURE ObtenerEmpleado
AS
BEGIN
    SELECT * FROM Empleado
END
------------   


CREATE PROCEDURE ValidarCorreo
	@Correo varchar(100)
AS
BEGIN

	SELECT	idCliente,
		    Cedula,
			Nombre,
			Correo
	  FROM	CLIENTE
	WHERE	Correo = @Correo
		
	
END
GO


CREATE PROCEDURE ActualizarUsuario
	@Cedula varchar(20),
	@Nombre varchar(15),
	@Correo varchar(100),
	@idCliente int
AS
BEGIN
	
	IF NOT EXISTS(SELECT 1 FROM CLIENTE
				  WHERE Cedula = @Cedula
					AND Correo = @Correo
					AND idCliente != @idCliente)
	BEGIN

		UPDATE	CLIENTE
		SET		Cedula = @Cedula,
				Nombre = @Nombre,
				Correo =  @Correo
		WHERE	idCliente = @idCliente

	END

END
GO


CREATE PROCEDURE ActualizarContrasenna
	@idCliente int,
	@contrasena varchar(255)
AS
BEGIN
	
	UPDATE	CLIENTE
	SET		contrasena = @contrasena
	WHERE	idCliente = @idCliente

END
GO

--  Pedido

CREATE PROCEDURE Crear_Pedido
    @Nombre_Producto VARCHAR(50), 
    @Numero_Pedido VARCHAR(50), 
    @Cantidad INT, 
    @FechaPedido DATE, 
    @Precio DECIMAL(10,2), 
    @Estado VARCHAR(100),
	@CorreoCliente VARCHAR(100)
AS
BEGIN
    INSERT INTO Pedido (Nombre_Producto, Numero_Pedido, Cantidad, FechaPedido, Precio, Estado, CorreoCliente)
    VALUES (@Nombre_Producto, @Numero_Pedido, @Cantidad, @FechaPedido, @Precio, @Estado, @CorreoCliente)
END


CREATE PROCEDURE ActualizarPedido
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
    SET
        Nombre_Producto = @Nombre_Producto,
        Numero_Pedido = @Numero_Pedido,
        Cantidad = @Cantidad,
        FechaPedido = @FechaPedido,
        Precio = @Precio,
        Estado = @Estado
    WHERE Id = @Id;
END


/*Delete*/
CREATE PROCEDURE EliminarPedido
    @Id INT
AS
BEGIN
    DELETE FROM Pedido 
	WHERE Id = @Id
END;


/*Procedimiento: MarcarPedidoComoEnCamino*/
CREATE PROCEDURE MarcarPedidoComoEnCamino
    @Id INT
AS
BEGIN
    UPDATE Pedido
    SET Estado = 'En camino'
    WHERE Id = @Id AND Estado = 'Preparando';
END


/*Procedimiento: MarcarPedidoComoEnviado*/
CREATE PROCEDURE MarcarPedidoComoEnviado
    @Id INT
AS
BEGIN
    UPDATE Pedido
    SET Estado = 'Enviado'
    WHERE Id = @Id AND Estado = 'En camino';
END


--    Reparaciones

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

 
 
--  Ventas

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
 

 --  Finanzas

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
 
-- Carrito

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

--  Inventario

--Crear un producto 
CREATE OR ALTER PROCEDURE CrearProducto
    @Nombre       VARCHAR(100),
    @Descripcion  VARCHAR(255),
    @Cantidad     INT,
    @Precio       DECIMAL(10, 2),
    @IdProveedor  INT,
    @IdCategoria  INT
AS
BEGIN
    INSERT INTO INVENTARIO (Nombre, Descripcion, Cantidad, Precio, IdProveedor, IdCategoria)
    VALUES (@Nombre, @Descripcion, @Cantidad, @Precio, @IdProveedor, @IdCategoria);
END

SELECT * FROM 

--Listar productos
CREATE OR ALTER PROCEDURE ObtenerProductos
AS
BEGIN
    SELECT 
        I.IdProducto,
        I.Nombre,
        I.Descripcion,
        I.Cantidad,
        I.Precio,
        I.IdProveedor,
        P.NombreEmpresa   AS NombreProveedor,
        I.IdCategoria,
        C.Nombre          AS NombreCategoria
    FROM 
        INVENTARIO I
    INNER JOIN 
        PROVEEDOR P   ON I.IdProveedor  = P.IDProveedor
    INNER JOIN 
        Categoria C   ON I.IdCategoria  = C.IdCategoria;
END;
GO


--Actualizar/editar prdcto
CREATE OR ALTER PROCEDURE ActualizarProducto
    @IdProducto   INT,
    @Nombre       VARCHAR(100),
    @Descripcion  VARCHAR(255),
    @Cantidad     INT,
    @Precio       DECIMAL(10,2),
    @IdProveedor  INT,
    @IdCategoria  INT
AS
BEGIN
    UPDATE INVENTARIO
    SET 
        Nombre      = @Nombre,
        Descripcion = @Descripcion,
        Cantidad    = @Cantidad,
        Precio      = @Precio,
        IdProveedor = @IdProveedor,
        IdCategoria = @IdCategoria
    WHERE IdProducto = @IdProducto;
END;
GO



--Eliminar el producto del inventario
CREATE PROCEDURE EliminarProducto
    @IdProducto INT
AS
BEGIN
    DELETE FROM INVENTARIO
    WHERE IdProducto = @IdProducto
END;


--  Categoria

CREATE PROCEDURE CrearCategoria
    @Nombre VARCHAR(100),
    @Descripcion VARCHAR(255)
AS
BEGIN
    INSERT INTO Categoria (Nombre, Descripcion)
    VALUES (@Nombre, @Descripcion);
END;


CREATE PROCEDURE ObtenerCategorias
AS
BEGIN
    SELECT * FROM Categoria;
END;

CREATE PROCEDURE ConsultarCategoriaID
    @IdCategoria INT
AS
BEGIN
    SELECT * FROM Categoria WHERE IdCategoria = @IdCategoria;
END;

CREATE PROCEDURE ActualizarCategoria
    @IdCategoria INT,
    @Nombre VARCHAR(100),
    @Descripcion VARCHAR(255)
AS
BEGIN
    UPDATE Categoria
    SET Nombre = @Nombre,
        Descripcion = @Descripcion
    WHERE IdCategoria = @IdCategoria;
END;

CREATE PROCEDURE EliminarCategoria
    @IdCategoria INT
AS
BEGIN
    DELETE FROM Categoria WHERE IdCategoria = @IdCategoria;
END;

--  PROVEEDOR

--CREAR
CREATE PROCEDURE CrearProveedor
    @NombreEmpresa NVARCHAR(100),
    @Correo NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @Estado NVARCHAR(20)
AS
BEGIN
    INSERT INTO Proveedor (NombreEmpresa, Correo, Telefono, Estado)
    VALUES (@NombreEmpresa, @Correo, @Telefono, @Estado)
END;


--MOSTRAR LA LISTA DE PROVEEDORES EN LA VISTA
CREATE PROCEDURE ObtenerProveedores
AS
BEGIN
    SELECT * FROM Proveedor
END;

CREATE OR ALTER PROCEDURE ObtenerProveedorPorId
    @IDProveedor INT
AS
BEGIN
    SELECT *
    FROM Proveedor
    WHERE IDProveedor = @IDProveedor;
END;

CREATE OR ALTER PROCEDURE ActualizarProveedor
    @IDProveedor INT,
    @NombreEmpresa NVARCHAR(100),
    @Correo NVARCHAR(100),
    @Telefono NVARCHAR(20),
    @Estado NVARCHAR(20)
AS
BEGIN
    UPDATE Proveedor
    SET 
        NombreEmpresa = @NombreEmpresa,
        Correo = @Correo,
        Telefono = @Telefono,
        Estado = @Estado
    WHERE IDProveedor = @IDProveedor;
END;

CREATE OR ALTER PROCEDURE EliminarProveedor
    @IDProveedor INT
AS
BEGIN
    DELETE FROM Proveedor
    WHERE IDProveedor = @IDProveedor;
END;

select * from cliente
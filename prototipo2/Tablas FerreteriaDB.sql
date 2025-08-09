USE master;

-- Crear la base de datos
CREATE DATABASE FerreteriaBD;
GO

USE FerreteriaBD;
GO

/*******************              TABLAS DEL SISTEMA                        ***********************/

-- Tabla: Cliente
CREATE TABLE Cliente (
	idCliente int IDENTITY(1,1),
    Cedula VARCHAR(15) PRIMARY KEY,
    Nombre VARCHAR(50),
    Apellido VARCHAR(50),
    Telefonos VARCHAR(50),
    Correo VARCHAR(100),
	contrasena varchar(50)
);
GO


-- ROL
CREATE TABLE ROL (
    IDRol INT IDENTITY(1,1) PRIMARY KEY,
    NombreRol NVARCHAR(50)
);
GO

----- INSERTS DE ROLES -----
INSERT INTO ROL (NombreRol) VALUES ('Administrador');
INSERT INTO ROL (NombreRol) VALUES ('Cliente');
INSERT INTO ROL (NombreRol) VALUES ('Empleado');



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


--Tabla PROVEEDOR
CREATE TABLE Proveedor (
    IdProveedor INT PRIMARY KEY IDENTITY(1,1),
    NombreEmpresa NVARCHAR(100) NOT NULL,
    Correo NVARCHAR(100),
    Telefono NVARCHAR(20),
    Estado NVARCHAR(20) DEFAULT 'Activo'
);


--Tabla de Categoria
CREATE TABLE Categoria (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(255)
);


-- Tabla: Inventario
CREATE TABLE INVENTARIO (
  IdProducto INT IDENTITY(1,1) PRIMARY KEY,
  Nombre VARCHAR(100),
  Descripcion VARCHAR(255),
  Cantidad INT,
  Precio DECIMAL(10,2),
  IdCategoria INT,
  IdProveedor INT,
  FOREIGN KEY (IdProveedor) REFERENCES Proveedor(IDProveedor),
  FOREIGN KEY (IdCategoria) REFERENCES Categoria(IdCategoria)
);
GO
-- Tabla de Reparacion
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

-- Tabla Carrito
CREATE TABLE Carrito (
    Id INT IDENTITY PRIMARY KEY,
    ProductoId INT NOT NULL,
    Cantidad INT NOT NULL,
    Nombre_Producto NVARCHAR(100) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL
);

INSERT INTO Carrito (ProductoId, Cantidad, Nombre_Producto, Precio)
VALUES 
(1, 2, 'Martillo de acero', 8.50),
(2, 1, 'Taladro eléctrico', 59.99);


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

CREATE PROCEDURE InsertarProducto
    @Nombre NVARCHAR(100),
    @Descripcion NVARCHAR(255),
    @Stock INT,
    @Precio DECIMAL(10,2)
AS
BEGIN
    INSERT INTO Productos (Nombre, Descripcion, Stock, Precio, Estado)
    VALUES (@Nombre, @Descripcion, @Stock, @Precio, 1);
END

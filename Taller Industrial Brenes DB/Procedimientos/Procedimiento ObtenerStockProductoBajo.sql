CREATE PROCEDURE ObtenerProductosConStockBajo
    @Limite INT = 10
AS
BEGIN
    SELECT ProductoID, Nombre, Descripcion, Stock, Precio
    FROM Productos
    WHERE Estado = 1 AND Stock < @Limite;
END


INSERT INTO Productos (Nombre, Descripcion, Stock, Precio)
VALUES 
('Martillo', 'Martillo con madera', 9, 18000);
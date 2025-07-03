CREATE TABLE Productos (
    ProductoID BIGINT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Stock INT NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Estado BIT NOT NULL DEFAULT 1
);

CREATE PROCEDURE ObtenerProductos
AS
BEGIN
    SELECT ProductoID, Nombre, Descripcion, Stock, Precio
    FROM Productos
    WHERE Estado = 1;
END

INSERT INTO Productos (Nombre, Descripcion, Stock, Precio)
VALUES 
('Aceite de Motor', 'Aceite sintético 5W-30', 50, 18000),
('Filtro de Aire', 'Filtro para motor Toyota', 25, 8500),
('Batería 12V', 'Batería de auto 12 voltios 60Ah', 15, 39000),
('Lámpara LED', 'Luz LED para faroles', 40, 7500),
('Refrigerante', 'Refrigerante para radiador', 30, 6700);

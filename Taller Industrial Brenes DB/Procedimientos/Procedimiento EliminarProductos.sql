CREATE PROCEDURE EliminarProducto
    @ProductoID BIGINT
AS
BEGIN
    UPDATE Productos
    SET Estado = 0
    WHERE ProductoID = @ProductoID;
END

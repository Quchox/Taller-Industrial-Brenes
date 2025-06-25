
USE TallerBrenes;
GO

IF OBJECT_ID('dbo.Productos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Productos
    (
        ProductoId   INT IDENTITY(1,1) PRIMARY KEY,
        Nombre       NVARCHAR(200)  NOT NULL,
        Descripcion  NVARCHAR(MAX)  NULL,
        CategoriaId  INT            NOT NULL,
        Precio       DECIMAL(18,2)  NOT NULL,
        Cantidad     INT            NOT NULL,      -- ← NUEVO
        ImagenUrl    NVARCHAR(500)  NULL,
        EstaActivo   BIT            NOT NULL DEFAULT 1
    );
    -- FK
    ALTER TABLE dbo.Productos
        ADD CONSTRAINT FK_Productos_Categorias
        FOREIGN KEY (CategoriaId) REFERENCES dbo.Categorias(CategoriaId);
END
ELSE
BEGIN
    IF COL_LENGTH('dbo.Productos', 'Cantidad') IS NULL
        ALTER TABLE dbo.Productos
        ADD Cantidad INT NOT NULL DEFAULT 0;
END
GO


IF OBJECT_ID('dbo.spCrearProducto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spCrearProducto;
GO
CREATE PROCEDURE dbo.spCrearProducto
    @Nombre       NVARCHAR(200),
    @Descripcion  NVARCHAR(MAX) = NULL,
    @CategoriaId  INT,
    @Precio       DECIMAL(18,2),
    @Cantidad     INT,
    @ImagenUrl    NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Productos
          (Nombre, Descripcion, CategoriaId, Precio, Cantidad, ImagenUrl, EstaActivo)
    VALUES(@Nombre, @Descripcion, @CategoriaId, @Precio, @Cantidad, @ImagenUrl, 1);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS NuevoProductoId;
END;
GO



IF OBJECT_ID('dbo.spActualizarProducto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spActualizarProducto;
GO
CREATE PROCEDURE dbo.spActualizarProducto
    @ProductoId   INT,
    @Nombre       NVARCHAR(200),
    @Descripcion  NVARCHAR(MAX) = NULL,
    @CategoriaId  INT,
    @Precio       DECIMAL(18,2),
    @Cantidad     INT,
    @ImagenUrl    NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Productos
       SET Nombre       = @Nombre,
           Descripcion  = @Descripcion,
           CategoriaId  = @CategoriaId,
           Precio       = @Precio,
           Cantidad     = @Cantidad,
           ImagenUrl    = @ImagenUrl
     WHERE ProductoId   = @ProductoId;

    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO



IF OBJECT_ID('dbo.spEliminarProducto', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spEliminarProducto;
GO
CREATE PROCEDURE dbo.spEliminarProducto
    @ProductoId INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Productos
       SET EstaActivo = 0
     WHERE ProductoId = @ProductoId;
    SELECT @@ROWCOUNT AS FilasAfectadas;
END;
GO



IF OBJECT_ID('dbo.spObtenerProductoPorId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spObtenerProductoPorId;
GO
CREATE PROCEDURE dbo.spObtenerProductoPorId
    @ProductoId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT P.ProductoId, P.Nombre, P.Descripcion,
           P.CategoriaId, P.Precio, P.Cantidad,
           P.ImagenUrl, P.EstaActivo,
           C.Nombre AS NombreCategoria
      FROM dbo.Productos P
      JOIN dbo.Categorias C ON C.CategoriaId = P.CategoriaId
     WHERE P.ProductoId = @ProductoId
       AND P.EstaActivo = 1;
END;
GO



IF OBJECT_ID('dbo.spListarProductos', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spListarProductos;
GO
CREATE PROCEDURE dbo.spListarProductos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT P.ProductoId, P.Nombre, P.Descripcion,
           P.CategoriaId, C.Nombre AS NombreCategoria,
           P.Precio, P.Cantidad,
           P.ImagenUrl
      FROM dbo.Productos P
      JOIN dbo.Categorias C ON C.CategoriaId = P.CategoriaId
     WHERE P.EstaActivo = 1;
END;
GO



IF OBJECT_ID('dbo.spListarProductosPorCategoria', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spListarProductosPorCategoria;
GO
CREATE PROCEDURE dbo.spListarProductosPorCategoria
    @CategoriaId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ProductoId, Nombre, Descripcion,
           Precio, Cantidad, ImagenUrl
      FROM dbo.Productos
     WHERE CategoriaId = @CategoriaId
       AND EstaActivo  = 1;
END;
GO

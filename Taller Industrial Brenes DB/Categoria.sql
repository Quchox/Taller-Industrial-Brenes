USE TallerBrenes;
GO
/*==============================================================
  1) Crear tabla (por si aún no existe) 
  ==============================================================*/
IF OBJECT_ID('dbo.Categorias', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Categorias
    (
        CategoriaID INT IDENTITY(1,1) PRIMARY KEY,
        Nombre      NVARCHAR(100) NOT NULL
        -- Si en el futuro quieres borrado lógico:
        -- ,EstaActivo BIT NOT NULL DEFAULT 1
    );
END
GO

/*==============================================================
  2) spCrearCategoria
  ==============================================================*/
IF OBJECT_ID('dbo.spCrearCategoria', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spCrearCategoria;
GO
CREATE PROCEDURE dbo.spCrearCategoria
    @Nombre NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.Categorias (Nombre)
    VALUES (@Nombre);

    SELECT SCOPE_IDENTITY() AS NuevaCategoriaID;   -- Devuelve el ID creado
END;
GO

/*==============================================================
  3) spActualizarCategoria
  ==============================================================*/
IF OBJECT_ID('dbo.spActualizarCategoria', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spActualizarCategoria;
GO
CREATE PROCEDURE dbo.spActualizarCategoria
    @CategoriaID INT,
    @Nombre      NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.Categorias
    SET Nombre = @Nombre
    WHERE CategoriaID = @CategoriaID;

    SELECT @@ROWCOUNT AS FilasAfectadas;           -- 0 = no existe, 1 = ok
END;
GO

/*==============================================================
  4) spEliminarCategoria
     (borrado físico; si prefieres lógico, cambia a UPDATE)
  ==============================================================*/
IF OBJECT_ID('dbo.spEliminarCategoria', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spEliminarCategoria;
GO
CREATE PROCEDURE dbo.spEliminarCategoria
    @CategoriaID INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.Categorias
    WHERE CategoriaID = @CategoriaID;

    SELECT @@ROWCOUNT AS FilasAfectadas;           -- 0 = no existe, 1 = ok
END;
GO

/*==============================================================
  5) spObtenerCategoriaPorId
  ==============================================================*/
IF OBJECT_ID('dbo.spObtenerCategoriaPorId', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spObtenerCategoriaPorId;
GO
CREATE PROCEDURE dbo.spObtenerCategoriaPorId
    @CategoriaID INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CategoriaID,
        Nombre
    FROM dbo.Categorias
    WHERE CategoriaID = @CategoriaID;
END;
GO

/*==============================================================
  6) spListarCategorias
  ==============================================================*/
IF OBJECT_ID('dbo.spListarCategorias', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spListarCategorias;
GO
CREATE PROCEDURE dbo.spListarCategorias
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        CategoriaID,
        Nombre
    FROM dbo.Categorias
    ORDER BY Nombre;
END;
GO

ALTER PROCEDURE dbo.spListarProductosPorCategoria
    @CategoriaId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        P.ProductoId,
        P.Nombre,
        P.Descripcion,
        P.CategoriaId,          -- ← añade esta línea
        P.Precio,
        P.ImagenUrl
    FROM dbo.Productos P
    WHERE P.CategoriaId = @CategoriaId
      AND P.EstaActivo  = 1;
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

    SELECT
        P.ProductoId,
        P.Nombre,
        P.Descripcion,
        P.CategoriaId,             -- ← columna necesaria
        P.Precio,
        P.Cantidad,
        P.ImagenUrl
    FROM dbo.Productos P
    WHERE P.CategoriaId = @CategoriaId
      AND P.EstaActivo  = 1;
END;
GO


USE [TallerBrenes]
GO

/****** Object:  StoredProcedure [dbo].[CambiarRolUsuario]    Script Date: 10/07/2025 18:12:03 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CambiarRolUsuario]
    @UsuarioID INT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT 1 FROM Usuarios WHERE UsuarioID = @UsuarioID)
    BEGIN
        DECLARE @RolActual INT;

        SELECT @RolActual = RolID FROM Usuarios WHERE UsuarioID = @UsuarioID;

        IF @RolActual = 0 OR @RolActual = 1
        BEGIN
            UPDATE Usuarios SET RolID = 2 WHERE UsuarioID = @UsuarioID;
            SELECT 1 AS Resultado; -- Cambiado a 2
        END
        ELSE IF @RolActual = 2
        BEGIN
            UPDATE Usuarios SET RolID = 1 WHERE UsuarioID = @UsuarioID;
            SELECT 1 AS Resultado; -- Cambiado a 1
        END
        ELSE
        BEGIN
            -- Si el rol es otro valor distinto a 0,1,2 no hace nada
            SELECT 0 AS Resultado; -- No cambio
        END
    END
    ELSE
    BEGIN
        SELECT -1 AS Resultado; -- Usuario no existe
    END
END;
GO




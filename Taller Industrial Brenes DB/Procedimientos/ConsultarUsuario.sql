USE [TallerBrenes]
GO

/****** Object:  StoredProcedure [dbo].[ConsultarUsuario]    Script Date: 10/07/2025 18:16:29 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[ConsultarUsuario]
    @UsuarioID BIGINT
AS
BEGIN
    IF (@UsuarioID = 0)
        SET @UsuarioID = NULL;

    SELECT 
        UsuarioID, 
        Identificacion, 
        Nombre, 
        ApellidoPaterno,
        ApellidoMaterno,
        Correo, 
        Estado,
        RolID
    FROM dbo.Usuarios
    WHERE (UsuarioID = @UsuarioID OR @UsuarioID IS NULL);
END
GO


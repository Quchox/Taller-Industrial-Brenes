
USE [TallerBrenes]
GO

/****** Object:  StoredProcedure [dbo].[ObtenerUsuarioPorCorreo]    Script Date: 24/06/2025 13:25:45 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerUsuarioPorCorreo]
    @Correo VARCHAR(100)
AS
BEGIN
    SELECT UsuarioID, Identificacion, Nombre, ApellidoPaterno, ApellidoMaterno,
           Correo, Estado, RolID
    FROM Usuarios
    WHERE LTRIM(RTRIM(Correo)) = LTRIM(RTRIM(@Correo)) AND Estado = 1;
END
GO

USE [TallerBrenes]
GO
/****** Object:  StoredProcedure [dbo].[ObtenerUsuarioCompleto]    Script Date: 24/06/2025 13:25:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ObtenerUsuarioCompleto]
    @correo VARCHAR(100)
AS
BEGIN
    SELECT  
        U.UsuarioID,
        U.Identificacion,
        U.Nombre,
        U.ApellidoPaterno,
        U.ApellidoMaterno,
        U.Correo,
        U.Estado,
        U.RolID,
        R.NombreRol AS NombreRol
    FROM dbo.Usuarios U
    INNER JOIN dbo.Roles R ON U.RolID = R.RolID
    WHERE U.Correo = @Correo
END
GO

USE [TallerBrenes]
GO
/****** Object:  StoredProcedure [dbo].[DesactivarUsuario]    Script Date: 24/06/2025 13:24:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DesactivarUsuario]
    @UsuarioID bigint
AS
BEGIN
    UPDATE Usuarios
    SET Estado = 0
    WHERE UsuarioID = @UsuarioID
END
GO

USE [TallerBrenes]
GO
/****** Object:  StoredProcedure [dbo].[ConsultarClientes]    Script Date: 24/06/2025 13:24:15 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ConsultarClientes]
	@UsuarioID BIGINT
AS
BEGIN
	IF (@UsuarioID = 0)
		SET @UsuarioID = NULL

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
	WHERE UsuarioID = ISNULL(@UsuarioID, UsuarioID)
	AND RolID = 2;
END
GO


USE [TallerBrenes]
GO
/****** Object:  StoredProcedure [dbo].[ActivarUsuario]    Script Date: 24/06/2025 13:22:39 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ActivarUsuario]
    @UsuarioID bigint
AS
BEGIN
    UPDATE Usuarios
    SET Estado = 1
    WHERE UsuarioID = @UsuarioID
END
GO
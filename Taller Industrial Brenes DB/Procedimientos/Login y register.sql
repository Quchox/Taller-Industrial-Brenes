-- Procedimiento para login
CREATE PROCEDURE LoginUsuario
    @Correo VARCHAR(100),
    @Contrasenna VARCHAR(255)
AS
BEGIN
    SELECT * FROM Usuarios
    WHERE Correo = @Correo AND Contrasenna = @Contrasenna AND Estado = 1;
END
GO

-- Procedimiento para registrar
CREATE PROCEDURE RegistrarUsuario
    @Identificacion VARCHAR(15),
    @Nombre VARCHAR(50),
    @ApellidoPaterno VARCHAR(50),
    @ApellidoMaterno VARCHAR(50),
    @Correo VARCHAR(100),
    @Contrasenna VARCHAR(255),
    @RolID BIGINT
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Usuarios WHERE Correo = @Correo)
    BEGIN
        RAISERROR('El correo ya está registrado.', 16, 1)
        RETURN
    END

    INSERT INTO Usuarios (Identificacion, Nombre, ApellidoPaterno, ApellidoMaterno, Correo, Contrasenna, Estado, RolID)
    VALUES (@Identificacion, @Nombre, @ApellidoPaterno, @ApellidoMaterno, @Correo, @Contrasenna, 1, @RolID);
END
GO

use TallerBrenes;
Go

-- Tabla de Asistencia
CREATE TABLE Asistencia (
    AsistenciaID BIGINT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID BIGINT NOT NULL,
    Fecha DATE NOT NULL,
    HoraEntrada TIME NULL,
    HoraSalida TIME NULL,
    Estado VARCHAR(50),
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
);
GO

-- Procedimiento para marcar entrada
CREATE PROCEDURE MarcarEntrada
    @UsuarioID BIGINT,
    @HoraEntrada TIME
AS
BEGIN
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    IF EXISTS (SELECT 1 FROM Asistencia WHERE UsuarioID = @UsuarioID AND Fecha = @Hoy)
    BEGIN
        RAISERROR('Ya se registró la entrada para hoy.', 16, 1);
        RETURN;
    END

    INSERT INTO Asistencia (UsuarioID, Fecha, HoraEntrada)
    VALUES (@UsuarioID, @Hoy, @HoraEntrada);
END
GO

-- Procedimiento para marcar salida
CREATE PROCEDURE MarcarSalida
    @UsuarioID BIGINT,
    @HoraSalida TIME
AS
BEGIN
    DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);

    IF NOT EXISTS (SELECT 1 FROM Asistencia WHERE UsuarioID = @UsuarioID AND Fecha = @Hoy)
    BEGIN
        RAISERROR('No hay entrada registrada para hoy.', 16, 1);
        RETURN;
    END

    UPDATE Asistencia
    SET HoraSalida = @HoraSalida
    WHERE UsuarioID = @UsuarioID AND Fecha = @Hoy;
END
GO

-- Procedimiento para listar asistencias por usuario
CREATE PROCEDURE ListarAsistenciaPorUsuario
    @UsuarioID BIGINT
AS
BEGIN
    SELECT * FROM Asistencia
    WHERE UsuarioID = @UsuarioID
    ORDER BY Fecha DESC;
END
GO

﻿USE [master]
GO
/****** Object:  Database [TallerBrenes]    Script Date: 5/6/2025 11:05:57 ******/
CREATE DATABASE [TallerBrenes]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TallerBrenes', FILENAME = N'D:\Progra avanzada\MSSQL16.SQLEXPRESS\MSSQL\DATA\TallerBrenes.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TallerBrenes_log', FILENAME = N'D:\Progra avanzada\MSSQL16.SQLEXPRESS\MSSQL\DATA\TallerBrenes_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [TallerBrenes] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TallerBrenes].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [TallerBrenes] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [TallerBrenes] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [TallerBrenes] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [TallerBrenes] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [TallerBrenes] SET ARITHABORT OFF 
GO
ALTER DATABASE [TallerBrenes] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [TallerBrenes] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [TallerBrenes] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [TallerBrenes] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [TallerBrenes] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [TallerBrenes] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [TallerBrenes] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [TallerBrenes] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [TallerBrenes] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [TallerBrenes] SET  ENABLE_BROKER 
GO
ALTER DATABASE [TallerBrenes] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [TallerBrenes] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [TallerBrenes] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [TallerBrenes] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [TallerBrenes] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [TallerBrenes] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [TallerBrenes] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [TallerBrenes] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [TallerBrenes] SET  MULTI_USER 
GO
ALTER DATABASE [TallerBrenes] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [TallerBrenes] SET DB_CHAINING OFF 
GO
ALTER DATABASE [TallerBrenes] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [TallerBrenes] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [TallerBrenes] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [TallerBrenes] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [TallerBrenes] SET QUERY_STORE = ON
GO
ALTER DATABASE [TallerBrenes] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [TallerBrenes]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 5/6/2025 11:05:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RolID] [bigint] IDENTITY(1,1) NOT NULL,
	[NombreRol] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RolID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Usuarios]    Script Date: 5/6/2025 11:05:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Usuarios](
	[UsuarioID] [bigint] IDENTITY(1,1) NOT NULL,
	[Identificacion] [varchar](15) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[ApellidoPaterno] [varchar](50) NOT NULL,
	[ApellidoMaterno] [varchar](50) NOT NULL,
	[Correo] [varchar](100) NOT NULL,
	[Contrasenna] [varchar](255) NOT NULL,
	[TieneContrasennaTemp] [bit] NULL,
	[FechaVencimientoTemp] [datetime] NULL,
	[Estado] [bit] NOT NULL,
	[RolID] [bigint] NOT NULL,
 CONSTRAINT [PK_Usuarios] PRIMARY KEY CLUSTERED 
(
	[UsuarioID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

INSERT [dbo].[Roles] ([RolID], [NombreRol]) VALUES (1, N'Administrador')
INSERT [dbo].[Roles] ([RolID], [NombreRol]) VALUES (2, N'Usuario')
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[Usuarios] ON 

INSERT [dbo].[Usuarios] ([UsuarioID], [Identificacion], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [Correo], [Contrasenna], [TieneContrasennaTemp], [FechaVencimientoTemp], [Estado], [RolID]) VALUES (3, N'123456789', N'Juan', N'Pérez', N'Soto', N'juanperez@example.com', N'12345', NULL, NULL, 1, 1)
INSERT [dbo].[Usuarios] ([UsuarioID], [Identificacion], [Nombre], [ApellidoPaterno], [ApellidoMaterno], [Correo], [Contrasenna], [TieneContrasennaTemp], [FechaVencimientoTemp], [Estado], [RolID]) VALUES (4, N'11111111111', N'Esteban ', N'Quiros', N'Martinez', N'quchochucho@gmail.com', N'12345', NULL, NULL, 1, 1)
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO
ALTER TABLE [dbo].[Usuarios]  WITH NOCHECK ADD  CONSTRAINT [FK_Usuarios_Roles] FOREIGN KEY([RolID])
REFERENCES [dbo].[Roles] ([RolID])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Roles]
GO
/****** Object:  StoredProcedure [dbo].[LoginUsuario]    Script Date: 5/6/2025 11:05:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Procedimiento para login
CREATE PROCEDURE [dbo].[LoginUsuario]
    @Correo VARCHAR(100),
    @Contrasenna VARCHAR(255)
AS
BEGIN
    SELECT * FROM Usuarios
    WHERE Correo = @Correo AND Contrasenna = @Contrasenna AND Estado = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[RegistrarUsuario]    Script Date: 5/6/2025 11:05:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Procedimiento para registrar
CREATE PROCEDURE [dbo].[RegistrarUsuario]
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
USE [master]
GO
ALTER DATABASE [TallerBrenes] SET  READ_WRITE 
GO
CREATE TABLE Horarios (
    HorarioID INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioID BIGINT NOT NULL, -- Relacionado con Usuarios
    DiaSemana VARCHAR(10) NOT NULL, -- Ej: 'Lunes'
    HoraInicio TIME NOT NULL,
    HoraFin TIME NOT NULL,
    Observaciones VARCHAR(255),
    FOREIGN KEY (UsuarioID) REFERENCES Usuarios(UsuarioID)
);

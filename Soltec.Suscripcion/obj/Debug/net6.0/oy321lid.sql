IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Plan] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Plan] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Rol] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Rol] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TicketValidacion] (
    [Id] nvarchar(450) NOT NULL,
    [IdUsuario] int NOT NULL,
    [Tipo] nvarchar(max) NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [FechaVencimiento] datetime2 NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_TicketValidacion] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Usuario] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Password] nvarchar(max) NOT NULL,
    [Salt] nvarchar(max) NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Suscripcion] (
    [Id] int NOT NULL IDENTITY,
    [IdCuenta] nvarchar(max) NOT NULL,
    [IdPlan] int NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    [Importe] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_Suscripcion] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Suscripcion_Plan_IdPlan] FOREIGN KEY ([IdPlan]) REFERENCES [Plan] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UsuarioCuenta] (
    [IdUsuario] int NOT NULL,
    [IdCuenta] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_UsuarioCuenta] PRIMARY KEY ([IdUsuario], [IdCuenta]),
    CONSTRAINT [FK_UsuarioCuenta_Usuario_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuario] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UsuarioRol] (
    [IdUsuario] int NOT NULL,
    [IdRol] int NOT NULL,
    CONSTRAINT [PK_UsuarioRol] PRIMARY KEY ([IdUsuario], [IdRol]),
    CONSTRAINT [FK_UsuarioRol_Rol_IdRol] FOREIGN KEY ([IdRol]) REFERENCES [Rol] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UsuarioRol_Usuario_IdUsuario] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuario] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Suscripcion_IdPlan] ON [Suscripcion] ([IdPlan]);
GO

CREATE INDEX [IX_UsuarioRol_IdRol] ON [UsuarioRol] ([IdRol]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230612123229_InitialCreate', N'7.0.1');
GO

COMMIT;
GO


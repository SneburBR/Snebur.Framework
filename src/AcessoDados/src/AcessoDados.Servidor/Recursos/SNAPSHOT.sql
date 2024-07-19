-- Mudar para SINGLE_USER
ALTER DATABASE Usuarios
SET SINGLE_USER
WITH ROLLBACK IMMEDIATE;
GO

-- Habilitar ALLOW_SNAPSHOT_ISOLATION
ALTER DATABASE Usuarios
SET ALLOW_SNAPSHOT_ISOLATION ON;
GO

-- Habilitar READ_COMMITTED_SNAPSHOT
ALTER DATABASE Usuarios
SET READ_COMMITTED_SNAPSHOT ON;
GO

-- Mudar para MULTI_USER
ALTER DATABASE Usuarios
SET MULTI_USER;
GO

-- Verificar configurações (opcional)
SELECT name, is_read_committed_snapshot_on, snapshot_isolation_state_desc
FROM sys.databases
WHERE name = 'Usuarios';
GO

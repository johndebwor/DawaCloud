-- Database Rename Script: DawaBes to DawaCloud
-- Execute this script as SA or database administrator
-- Server: 192.168.100.79

USE master;
GO

-- Kill all active connections to DawaBes database
DECLARE @kill varchar(8000) = '';
SELECT @kill = @kill + 'KILL ' + CONVERT(varchar(5), session_id) + ';'
FROM sys.dm_exec_sessions
WHERE database_id = DB_ID('DawaBes');

EXEC(@kill);
GO

-- Set database to single user mode
ALTER DATABASE [DawaBes] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

-- Rename the database
ALTER DATABASE [DawaBes] MODIFY NAME = [DawaCloud];
GO

-- Set back to multi user mode
ALTER DATABASE [DawaCloud] SET MULTI_USER;
GO

-- Verify rename
SELECT name, state_desc FROM sys.databases WHERE name = 'DawaCloud';
GO

PRINT 'Database successfully renamed from DawaBes to DawaCloud';
GO

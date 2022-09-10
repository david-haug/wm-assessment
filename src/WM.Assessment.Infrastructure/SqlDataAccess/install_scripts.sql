--CREATE DB NAMED wm_assessment
IF
NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'wm_assessment')
BEGIN
    CREATE
DATABASE [wm_assessment]
END
GO

--INSTALL DB OBJECTS
USE [wm_assessment]
GO

--TABLES
IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE 'dbo' AND name like 'event')
DROP TABLE [dbo].[event];
GO
CREATE TABLE [dbo].[event]
(
    row_id
    int
    identity,
    id
    varchar
(
    50
) not null
    constraint PK_event
    primary key,
    data varchar
(
    max
),
    date_created datetimeoffset
    constraint DF_event_date_created default getdate
(
) not null
    )
    GO
    IF EXISTS
(
    SELECT * FROM sys.tables WHERE SCHEMA_NAME
(
    schema_id
) LIKE 'dbo' AND name like 'guids')
DROP TABLE [dbo].[guids];
GO
CREATE TABLE [dbo].[guids]
(
    Guid varchar
(
    50
) not null
    constraint guids_pk
    primary key,
    [User] varchar
(
    500
),
    Expire datetimeoffset
    )
    go

--PROCS
BEGIN
DROP PROCEDURE IF EXISTS [dbo].[get_json_all]
END
GO

CREATE PROCEDURE [dbo].[get_json_all]
    @startAfter varchar(50) = null
as

declare
@results table
                 (
                     row_id int,
                     id varchar(50),
                     [data] varchar(MAX),
                     date_created datetimeoffset(7)
                 )

insert into @results
select row_id, id, [data], date_created
from [dbo].[event]
declare
@startAfterRowId int = 0
    if(ISNULL(@startAfter,'') <> '')
select @startAfterRowId = row_id
from @results
where id = @startAfter

select row_id as rowId, id, [data], date_created
from @results
where row_id > @startAfterRowId
order by row_id
    GO

BEGIN
DROP PROCEDURE IF EXISTS [dbo].[guid_delete]
END
GO

CREATE PROCEDURE [dbo].[guid_delete]
    @Guid varchar(50)
as

delete
from guids
where Guid = @Guid
    GO

BEGIN
DROP PROCEDURE IF EXISTS [dbo].[guid_get_by_id]
END
GO

CREATE PROCEDURE [dbo].[guid_get_by_id]
    @Guid varchar(50)
as

select *
from guids
where Guid = @Guid
    GO

BEGIN
DROP PROCEDURE IF EXISTS [dbo].[guid_insert]
END
GO

CREATE PROCEDURE [dbo].[guid_insert]
    @Guid varchar(50),
    @User varchar(500),
    @Expire datetimeoffset
as 
INSERT INTO guids
(
 Guid, [User], Expire
)
VALUES 
(
@Guid,@User,@Expire
)
GO

BEGIN
DROP PROCEDURE IF EXISTS [dbo].[guid_save]
END
GO

CREATE PROCEDURE [dbo].[guid_save]
    @Guid varchar(50),
    @User varchar(500),
    @Expire datetimeoffset
as
    if (select count(Guid) from guids where Guid = @Guid) = 0
        INSERT INTO guids
        (
            Guid, [User], Expire
        )
        VALUES
        (
            @Guid,@User,@Expire
        )
    else
UPDATE guids
SET [User] = @User,
    Expire = @Expire
WHERE Guid = @Guid
    GO

BEGIN
DROP PROCEDURE IF EXISTS [dbo].[guid_update]
END
GO

CREATE PROCEDURE [dbo].[guid_update]
    @Guid varchar(50),
    @User varchar(500),
    @Expire datetimeoffset
as

UPDATE guids
SET [User] = @User,
    Expire = @Expire
WHERE Guid = @Guid
    GO

BEGIN
DROP PROCEDURE IF EXISTS [dbo].[save_json]
END
GO

CREATE PROCEDURE [dbo].[save_json]
    @id varchar(50),
    @data varchar(MAX)
as
    if (select count(id) from [dbo].[event] where id = @id) = 0
        insert into [dbo].[event] (id,[data]) values (@id,@data)
    else
update [dbo].[event]
set [data] = @data
where id = @id
    GO

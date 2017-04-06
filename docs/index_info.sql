use QualityAndEngineering
use CheckWeigh_Data_Dev
go

--select * from spares_contract_numbers

--select name from sysobjects where type='U'

--SELECT name,object_name(parent_object_id) FROM sys.objects
--WHERE type = 'PK' 
----AND  parent_object_id = OBJECT_ID ('tableName')
--order by 2
--go

--select * from sysindexes where id=object_id('colt_employee') and indid=1

declare 
	@index int,
	@colName varchar(100),
	@tableName varchar(100);
set @index=1
set @tableName='colt_employee';
while @index<=16 begin
	select @colName=index_col(@tableName,1,@index)
	set @index=@index+1
	if isnull(@colName,'NONE')='NONE'
		break
	--select 'COL-NAME='+@colName
	select @colName
end
--select index_col(@tableName,1,1)
--select index_col(@tableName,1,2)
--select index_col(@tableName,1,3)
select object_name(id),* from sysindexes   where indid=1 order by 1
go
--sp_help colt_employee
--go

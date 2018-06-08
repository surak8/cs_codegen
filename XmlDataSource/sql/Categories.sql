use CheckWeigh_Data_Dev;

IF OBJECT_ID(N'dbo.Categories', N'U') IS NOT NULL 
	drop table Categories;
GO

CREATE table Categories(
	CategoryID int identity(1,1) not null primary key,
	CategoryName varchar(20) not null
);

insert Categories(CategoryName) values('Cat1');
insert Categories(CategoryName) values('Cat2');
insert Categories(CategoryName) values('Cat3');
insert Categories(CategoryName) values('Cat4');
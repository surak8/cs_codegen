use CheckWeigh_Data_Dev;

IF OBJECT_ID(N'dbo.Customers', N'U') IS NOT NULL 
	drop table Customers;
GO

CREATE table Customers(
	CustomerID int identity(1,1) not null primary key,
	CompanyName varchar(20) not null
);

insert Customers(CompanyName) values('Cust1');
insert Customers(CompanyName) values('Cust2');
insert Customers(CompanyName) values('Cust3');
insert Customers(CompanyName) values('Cust4');

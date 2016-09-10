use checkweigh_data_dev
go
print 'installing in '+db_name()+' on server '+@@servername

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID(N'dbo.my_test_proc', N'P') IS NOT NULL 
	drop procedure my_test_proc
GO
create procedure my_test_proc 
            @badge_number varchar(10),
            @message varchar(255)
as begin
	declare 
		@emp varchar(20),
		@last varchar(20),
		@first varchar(20);

	print '*** args are: Badge='+isnull(@badge_number,'null')+', message='''+@message+'''.';

	if (@badge_number is null) begin
		print '*** null badge-number, so process values in @message';
		if (UPPER(@message)='ERR1') BEGIN
			print @message+' returns return-value 10';
			return 10;
		END
		else BEGIN
			if (UPPER(@MESSAGE)='ERR2') BEGIN
				print '*** '+@message+' returns return-value 20, and performs a SELECT';
				select 'hooray, found message:'+@message
				return 20;
			END
			else BEGIN
				if (UPPER(@MESSAGE)='ERR3') BEGIN
					print '*** '+@message+' performs a RAISERROR';
--					raiserror(N'intentionally raising an error for @message=''%s''.',2,2,@message);
					RAISERROR('Intentional raise-error',16,1);
					return 30;

				END
				else BEGIN
					print '*** unhandled message:'+@message
					select 'unhandled-message',@message
					return 40;
				END
			END
		end
	end
	else begin
		if (select count(*) from   colt_employee where badgenum=@badge_number)<1 BEGIN
			print 'badge '+@badge_number+' not found!'
			return -1;
		END
		select @emp=emp,@last=lastname,@first=firstname from colt_employee where badgenum=@badge_number
		print 'found '''+@last+','+@first+''' for badge='+@badge_number
		-- implied return-value is zero
	end
end
grant exec on my_test_proc to public
go

/*
sqlcmd -Uoperator -Poperator -Scolt-sql -d checkweigh_data_dev

declare @rc int;
exec @rc=my_test_proc @badge_number='11111',@message='riktest'
print 'RC='+convert(varchar(10),@rc)
exec @rc=my_test_proc @badge_number=null,@message='err1'
print 'RC='+convert(varchar(10),@rc)
exec @rc=my_test_proc @badge_number=null,@message='err2'
print 'RC='+convert(varchar(10),@rc)
exec @rc=my_test_proc @badge_number=null,@message='err3'
print 'RC='+convert(varchar(10),@rc)
exec @rc=my_test_proc @badge_number=null,@message='riktest'
print 'RC='+convert(varchar(10),@rc)
go
*/

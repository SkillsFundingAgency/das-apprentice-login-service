
  /*  CS-1163
  
  Update the SupportUrl eg. in AT
  "SupportUrl": "https://confirm.at-aas.apprenticeships.education.gov.uk/"  to
  "SupportUrl": "https://at-aas.apprenticeships.education.gov.uk/home"
  
  Prod:
  "SupportUrl": "https://confirm.my.apprenticeships.education.gov.uk/"  to  
  "SupportUrl": "https://my.apprenticeships.education.gov.uk/Home"

  */

  declare @environment nvarchar(10) =
	case
		when DB_NAME() = 'das-at-alogin-db' then 'at'
		when DB_NAME() = 'das-test-alogin-db' then 'test'
		when DB_NAME() = 'das-test2-alogin-db' then 'test2'
		when DB_NAME() = 'das-pp-alogin-db' then 'pp'
		when DB_NAME() = 'das-prd-alogin-db' then ''
	else null
  end;

  if @environment is null
	throw 60000, 'Database should be one of das-at-alogin-db, das-test-alogin-db, das-test2-alogin-db, das-pp-alogin-db, das-prd-alogin-db', 1;

  
  declare @existingLink nvarchar(100), @newLink nvarchar(100);
  
  if DB_NAME() = 'das-prd-alogin-db'
    begin
		select @existingLink = '"SupportUrl": "https://confirm.my.apprenticeships.education.gov.uk/",',
		       @newLink = '"SupportUrl": "https://my.apprenticeships.education.gov.uk/Home",';
    end
  else
    begin
		select @existingLink = '"SupportUrl": "https://confirm.' + @environment + '-aas.apprenticeships.education.gov.uk/",',
		       @newLink = '"SupportUrl": "https://' + @environment + '-aas.apprenticeships.education.gov.uk/Home",';
	end

  
  --select replace([ServiceDetails], @existingLink, @newLink)
  --  from [LoginService].[Clients]
  -- where [IdentityServerClientId] = 'apprentice'
   
  
  update c
     set c.[ServiceDetails] = replace([ServiceDetails], @existingLink, @newLink)
    from [LoginService].[Clients] c
   where [IdentityServerClientId] = 'apprentice'
     and c.[ServiceDetails] like '%' + @existingLink + '%'
	 

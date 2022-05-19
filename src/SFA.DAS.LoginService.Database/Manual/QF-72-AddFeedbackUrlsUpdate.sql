
  /*  QF-72
  
  Add the Feedback Application SignIn/SignOut Urls to the Login Application
  
  AT:
  https://feedback.at-aas.apprenticeships.education.gov.uk/signin-oidc
  https://feedback.at-aas.apprenticeships.education.gov.uk/signout-callback-oidc
  
  Prod:
  https://feedback.my.apprenticeships.education.gov.uk/signin-oidc  
  https://feedback.my.apprenticeships.education.gov.uk/signout-callback-oidc
  
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

  
  declare @newSignInLink nvarchar(120), @newSignOutLink nvarchar(120), @clientId int;
  
  if DB_NAME() = 'das-prd-alogin-db'
    begin
      select @newSignInLink = 'https://feedback.my.apprenticeships.education.gov.uk/signin-oidc';
      select @newSignOutLink = 'https://feedback.my.apprenticeships.education.gov.uk/signout-callback-oidc';
    end
  else
    begin
      select @newSignInLink = 'https://feedback.' + @environment + '-aas.apprenticeships.education.gov.uk/signin-oidc';
      select @newSignOutLink = 'https://feedback.' + @environment + '-aas.apprenticeships.education.gov.uk/signout-callback-oidc';
    end

  select @clientId = Id from [IdentityServer].[Clients] where ClientId = 'apprentice'

  if not exists (
    select 1 from [IdentityServer].[ClientRedirectUris] where RedirectUri = @newSignInLink
  )
  begin  
    insert into [IdentityServer].[ClientRedirectUris] (RedirectUri, ClientId) values(@newSignInLink, @clientId)
  end

  if not exists(
    select 1 from [IdentityServer].[ClientPostLogoutRedirectUris] where PostLogoutRedirectUri = @newSignoutLink
  )
  begin
    insert into [IdentityServer].[ClientPostLogoutRedirectUris] (PostLogoutRedirectUri, ClientId) values (@newSignOutLink, @clientId)
  end
  


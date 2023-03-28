
/*  CSP-558

Add the aan Application SignIn/SignOut Urls to the Login Application

AT:
https://aan.at-aas.apprenticeships.education.gov.uk/signin-oidc
https://aan.at-aas.apprenticeships.education.gov.uk/signout-callback-oidc

Prod:
https://aan.my.apprenticeships.education.gov.uk/signin-oidc  
https://aan.my.apprenticeships.education.gov.uk/signout-callback-oidc

*/

declare @dbName nvarchar(max) = DB_NAME();

declare @environment nvarchar(10) =
    case
        when @dbName = 'das-at-alogin-db' then 'at'
        when @dbName = 'das-test-alogin-db' then 'test'
        when @dbName = 'das-test2-alogin-db' then 'test2'
        when @dbName = 'das-pp-alogin-db' then 'pp'
        when @dbName = 'das-prd-alogin-db' then ''
    else null
    end;

if @environment is null
    throw 60000, 'Database should be one of das-at-alogin-db, das-test-alogin-db, das-test2-alogin-db, das-pp-alogin-db, das-prd-alogin-db', 1;

declare @newSignInLink nvarchar(120), @newSignOutLink nvarchar(120), @clientId int;
  
if @dbName = 'das-prd-alogin-db'
begin
    select @newSignInLink = 'https://aan.my.apprenticeships.education.gov.uk/signin-oidc';
    select @newSignOutLink = 'https://aan.my.apprenticeships.education.gov.uk/signout-callback-oidc';
end
else
begin
    select @newSignInLink = 'https://aan.' + @environment + '-aas.apprenticeships.education.gov.uk/signin-oidc';
    select @newSignOutLink = 'https://aan.' + @environment + '-aas.apprenticeships.education.gov.uk/signout-callback-oidc';
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
  


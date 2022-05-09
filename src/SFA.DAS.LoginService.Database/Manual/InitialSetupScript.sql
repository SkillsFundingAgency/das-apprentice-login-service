/*
	This script will create initial setup data for a new apprentice login service database and was copied from confluence
	https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/2595815438/Using+das-login-service+for+authentication+provider

	First two values need to be set up manually amd then the script ran. Only work at the moment on a new database
*/


declare @apprenticePortalHostname as nvarchar(100) = 'localhost:7070' -- WITHOUT trailing slash (or scheme) 
declare @apprenticeAccountWebHost as nvarchar(100); 
set @apprenticeAccountWebHost = 'https://localhost:7080'
declare @apprenticeFeedbackWebHost as nvarchar(100); 
set @apprenticeFeedbackWebHost = 'https://localhost:7090'

declare @apprenticePortalWebHost as nvarchar(100); set @apprenticePortalWebHost = 'https://' + @apprenticePortalHostname
declare @confirmWebHost as nvarchar(100); set @confirmWebHost = 'https://confirm.' + @apprenticePortalHostname


---- [IdentityServer].[IdentityResources]

set identity_insert [IdentityServer].[IdentityResources] on

insert [IdentityServer].[IdentityResources] ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [NonEditable])
select * from (values 
	(1, 1, N'openid', N'Your user identifier', NULL, 1, 0, 1, CAST(N'2019-03-18T11:32:45.4856135' AS DateTime2), NULL, 0),
	(2, 1, N'profile', N'User profile', N'Your user profile information (first name, last name, etc.)', 0, 1, 1, CAST(N'2019-03-18T11:32:45.5529611' AS DateTime2), NULL, 0),
	(3, 1, N'email', N'Your email address', NULL, 0, 1, 1, CAST(N'2019-03-18T11:32:45.5744577' AS DateTime2), NULL, 0))
as insrt ([Id], [Enabled], [Name], [DisplayName], [Description], [Required], [Emphasize], [ShowInDiscoveryDocument], [Created], [Updated], [NonEditable]) 
where not exists (select 1 from [IdentityServer].[IdentityResources] where Id = insrt.Id)

set identity_insert [IdentityServer].[IdentityResources] off

---- [IdentityServer].[IdentityClaims]

set identity_insert [IdentityServer].[IdentityClaims] on

insert [IdentityServer].[IdentityClaims] ([Id], [Type], [IdentityResourceId])
select * from (values 
	(1, N'sub', 1),	
	(2, N'updated_at', 2),	
	(2, N'updated_at', 2),	
	(8, N'email', 3),	
	(10, N'preferred_username', 2),	
	(13, N'given_name', 2),	
	(14, N'family_name', 2),	
	(15, N'name', 2),	
	(16, N'profile', 2),	
	(17, 'apprentice_id', 2),	
	(18, N'email_verified', 3))
as insrt ([Id], [Type], [IdentityResourceId])
where not exists (select 1 from [IdentityServer].[IdentityClaims] where Id = insrt.Id)

set identity_insert [IdentityServer].[IdentityClaims] off



---- [IdentityServer].[Clients]

set identity_insert [IdentityServer].[Clients] on

insert [IdentityServer].[Clients] ([Id], [Enabled], [ClientId], [ProtocolType], [RequireClientSecret], [ClientName], [Description], [ClientUri], [LogoUri], [RequireConsent], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [RequirePkce], [AllowPlainTextPkce], [AllowAccessTokensViaBrowser], [FrontChannelLogoutUri], [FrontChannelLogoutSessionRequired], [BackChannelLogoutUri], [BackChannelLogoutSessionRequired], [AllowOfflineAccess], [IdentityTokenLifetime], [AccessTokenLifetime], [AuthorizationCodeLifetime], [ConsentLifetime], [AbsoluteRefreshTokenLifetime], [SlidingRefreshTokenLifetime], [RefreshTokenUsage], [UpdateAccessTokenClaimsOnRefresh], [RefreshTokenExpiration], [AccessTokenType], [EnableLocalLogin], [IncludeJwtId], [AlwaysSendClientClaims], [ClientClaimsPrefix], [PairWiseSubjectSalt], [Created], [Updated], [LastAccessed], [UserSsoLifetime], [UserCodeType], [DeviceCodeLifetime], [NonEditable])
select * from (values 
	(2, 1, N'apprentice', N'oidc', 1, N'Apprentice Client', NULL, NULL, NULL, 0, 1, 0, 0, 0, 0, NULL, 1, NULL, 1, 0, 300, 3600, 300, NULL, 2592000, 1296000, 1, 0, 1, 0, 1, 0, 0, N'apprentice_', NULL, CAST(N'2019-03-18T11:32:44.0555601' AS DateTime2), NULL, NULL, NULL, NULL, 300, 0))
as insrt ([Id], [Enabled], [ClientId], [ProtocolType], [RequireClientSecret], [ClientName], [Description], [ClientUri], [LogoUri], [RequireConsent], [AllowRememberConsent], [AlwaysIncludeUserClaimsInIdToken], [RequirePkce], [AllowPlainTextPkce], [AllowAccessTokensViaBrowser], [FrontChannelLogoutUri], [FrontChannelLogoutSessionRequired], [BackChannelLogoutUri], [BackChannelLogoutSessionRequired], [AllowOfflineAccess], [IdentityTokenLifetime], [AccessTokenLifetime], [AuthorizationCodeLifetime], [ConsentLifetime], [AbsoluteRefreshTokenLifetime], [SlidingRefreshTokenLifetime], [RefreshTokenUsage], [UpdateAccessTokenClaimsOnRefresh], [RefreshTokenExpiration], [AccessTokenType], [EnableLocalLogin], [IncludeJwtId], [AlwaysSendClientClaims], [ClientClaimsPrefix], [PairWiseSubjectSalt], [Created], [Updated], [LastAccessed], [UserSsoLifetime], [UserCodeType], [DeviceCodeLifetime], [NonEditable])
where not exists (select 1 from [IdentityServer].[Clients] where Id = insrt.Id)

set identity_insert [IdentityServer].[Clients] off
  
 
 
---- [IdentityServer].[ClientGrantTypes]

set identity_insert [IdentityServer].[ClientGrantTypes] on

insert [IdentityServer].[ClientGrantTypes] ([Id], [GrantType], [ClientId])
select * from (values 
	(1, N'client_credentials', 1))
as insrt ([Id], [GrantType], [ClientId])
where not exists (select 1 from [IdentityServer].[ClientGrantTypes] where Id = insrt.Id)

set identity_insert [IdentityServer].[ClientGrantTypes] off

 
---- [IdentityServer].[ClientRedirectUris]

set identity_insert [IdentityServer].[ClientRedirectUris] on

insert [IdentityServer].[ClientRedirectUris] ([Id], [RedirectUri], [ClientId])
select * from (values 
	(1, @apprenticePortalWebHost + N'/signin-oidc', 2),
	(2, @confirmWebHost + N'/signin-oidc', 2), 
	(3, @apprenticeAccountWebHost + N'/signin-oidc', 2),
	(4, @apprenticeFeedbackWebHost + N'/signin-oidc', 2))
as insrt ([Id], [RedirectUri], [ClientId])
where not exists (select 1 from [IdentityServer].[ClientRedirectUris] where Id = insrt.Id)

set identity_insert [IdentityServer].[ClientRedirectUris] off


---- [IdentityServer].[ClientPostLogoutRedirectUris]
set identity_insert [IdentityServer].[ClientPostLogoutRedirectUris] on

insert [IdentityServer].[ClientPostLogoutRedirectUris] ([Id], [PostLogoutRedirectUri], [ClientId])
select * from (values 
	(1, @apprenticePortalWebHost + N'/signout-callback-oidc', 2),
	(2, @confirmWebHost + N'/signout-callback-oidc', 2),
	(3, @apprenticeAccountWebHost + N'/signout-callback-oidc', 2),
	(3, @apprenticeFeedbackWebHost + N'/signout-callback-oidc', 2))
as insrt ([Id], [PostLogoutRedirectUri], [ClientId])
where not exists (select 1 from [IdentityServer].[ClientPostLogoutRedirectUris] where Id = insrt.Id)

set identity_insert [IdentityServer].[ClientPostLogoutRedirectUris] off


---- [IdentityServer].[ClientGrantTypes]

set identity_insert [IdentityServer].[ClientGrantTypes] on

insert [IdentityServer].[ClientGrantTypes] ([Id], [GrantType], [ClientId])
select * from (values 
	(2, N'implicit', 2))
as insrt ([Id], [GrantType], [ClientId])
where not exists (select 1 from [IdentityServer].[ClientGrantTypes] where Id = insrt.Id)

set identity_insert [IdentityServer].[ClientGrantTypes] off


---- [IdentityServer].[ClientScopes]

set identity_insert [IdentityServer].[ClientScopes] on

insert [IdentityServer].[ClientScopes] ([Id], [Scope], [ClientId])
select * from (values 
	(2, N'openid', 2),
	(4, N'profile', 2))
as insrt ([Id], [Scope], [ClientId])
where not exists (select 1 from [IdentityServer].[ClientScopes] where Id = insrt.Id)

set identity_insert [IdentityServer].[ClientScopes] off


---- [LoginService].[Clients]

delete from LoginService.Clients where Clients.Id = '36BCFAAD-1FF7-49CB-8EEF-19877B7AD0C9'

insert [LoginService].[Clients] ([Id], [ServiceDetails], [IdentityServerClientId], [AllowInvitationSignUp], [AllowLocalSignUp])
select * from (values 
	(N'{36BCFAAD-1FF7-49CB-8EEF-19877B7AD0C9}',
    N'{
    "ServiceName": "My apprenticeship",
    "ServiceTeam": "My apprenticeship service team",
    "SupportUrl": "' + @confirmWebHost + '/",
    "CreateAccountUrl": null,
    "PostPasswordResetReturnUrl": "' + @confirmWebHost + '/apprenticeships",
    "EmailTemplates": [{
            "Name": "SignUpInvitation",
            "TemplateId": " 4f04cf81-b291-4577-9452-ecab875ed6f8 "
        }, {
            "Name": "PasswordReset",
            "TemplateId": "ecbff8b8-3ad4-48b8-a42c-7d3f602dbbd3"
        }, {
            "Name": "PasswordResetNoAccount",
            "TemplateId": "04326941-2067-4956-8dc2-4ccd60c84af5"
        }, {
            "Name": "LoginPasswordWasReset",
            "TemplateId": "fa156448-44d5-4d76-8407-685a609a14ca"
        }, {
            "Name": "LoginSignupError",
            "TemplateId": "2b49c5be-43fc-4998-b40d-6bb5b4c1fcee"
        }, {
            "Name": "LoginSignupInvite",
            "TemplateId": "4f04cf81-b291-4577-9452-ecab875ed6f8 "
        }, {
            "Name": "ChangeEmailAddress",
            "TemplateId": "e120ee71-b425-4ddd-950f-9d59d05bdf1d"
        }
    ]}',
	N'apprentice', 1, 0))
as insrt ([Id], [ServiceDetails], [IdentityServerClientId], [AllowInvitationSignUp], [AllowLocalSignUp])
where not exists (select 1 from [LoginService].[Clients] where Id = insrt.Id)

IF NOT EXISTS(SELECT Id FROM [IdentityServer].[ClientScopes] WHERE Scope = 'profile' AND ClientId = 2)
BEGIN
  INSERT INTO [IdentityServer].[ClientScopes] ([Scope], [ClientId]) VALUES (N'profile', 2)
END

IF NOT EXISTS(SELECT Id FROM [IdentityServer].[ClientScopes] WHERE Scope = 'profile' AND ClientId = 3)
BEGIN
  INSERT [IdentityServer].[ClientScopes] ([Scope], [ClientId]) VALUES (N'profile', 3)
END


INSERT INTO IdentityServer.AspNetUserClaims (ClaimType, ClaimValue, UserId)
SELECT 'family_name' AS ClaimType, users.FamilyName, users.Id 
FROM IdentityServer.AspNetUsers AS users
WHERE NOT EXISTS (SELECT UserId FROM IdentityServer.AspNetUserClaims claims WHERE claims.ClaimType = 'family_name' AND claims.UserId = users.Id)

INSERT INTO IdentityServer.AspNetUserClaims (ClaimType, ClaimValue, UserId)
SELECT 'given_name' AS ClaimType, users.GivenName, users.Id 
FROM IdentityServer.AspNetUsers AS users
WHERE NOT EXISTS (SELECT UserId FROM IdentityServer.AspNetUserClaims claims WHERE claims.ClaimType = 'given_name' AND claims.UserId = users.Id)

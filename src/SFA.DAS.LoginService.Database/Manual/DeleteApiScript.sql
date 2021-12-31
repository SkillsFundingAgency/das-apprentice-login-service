/*
	This script will remove the API endpoints and associated table values. so invitation cannot be sent 
*/

-- Sanity Check
SELECT * FROM [IdentityServer].[ApiResources]       -- Should have 1 row
SELECT * FROM [IdentityServer].[ApiScopes]          -- Should have 1 row
SELECT * FROM [IdentityServer].[ApiSecrets]         -- should have 0 rows

SELECT * FROM [IdentityServer].[Clients]            -- Should have 2 rows
SELECT * FROM [IdentityServer].[ClientScopes]       -- Should have 3 rows
SELECT * FROM [IdentityServer].[ClientSecrets]      -- Should have 1 row (but no worries if none)



BEGIN TRANSACTION


---- [IdentityServer].[ApiResources]

IF (SELECT COUNT(*) FROM [IdentityServer].[ApiResources]) = 1 
BEGIN
    DELETE [IdentityServer].[ApiResources] 
END ELSE BEGIN
    PRINT 'Error Deleting [IdentityServer].[ApiResources]'
END

---- [IdentityServer].[Clients]

IF (SELECT COUNT(*) FROM [IdentityServer].[Clients] WHERE Id = 1) = 1 
BEGIN
    DELETE [IdentityServer].[Clients] WHERE Id = 1
END ELSE BEGIN
    PRINT 'Error Deleting [IdentityServer].[Clients]'
END



ROLLBACK TRANSACTION

-- COMMIT -- If no printed errors


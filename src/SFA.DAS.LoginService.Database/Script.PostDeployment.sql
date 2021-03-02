-- Apply to be setup for RoATP
UPDATE LoginService.Clients
SET ServiceDetails =  JSON_MODIFY(JSON_MODIFY(ServiceDetails,'$.ServiceName','Register of apprenticeship training providers service'),'$.ServiceTeam','The Apprenticeship Service')
WHERE  IdentityServerClientId = 'apply'

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'LoginService' 
                 AND  TABLE_NAME = 'InvalidPasswords'))
BEGIN
    DROP TABLE LoginService.InvalidPasswords
END




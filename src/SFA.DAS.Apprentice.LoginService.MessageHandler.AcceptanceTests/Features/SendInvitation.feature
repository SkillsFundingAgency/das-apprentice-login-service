Feature: SendInvitation
	In order to allow apprentices to create accounts
	An invitaion is sent to the apprentices email address

Scenario: When 'send invitation command' is received for a new apprentice
	Given we have created a valid SendInvitationCommand 
	And the login service api is ready to respond
	When the SendInvitationCommand is received
	Then the invitation is forwarded to the api correctly

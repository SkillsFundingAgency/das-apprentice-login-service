Feature: SendInvitation
	In order to allow apprentices to create accounts
	An invitaion is sent to the apprentices email address

Scenario: When 'send invitation command' is received for a new apprentice
	When the SendInvitationCommand is received
	Then the invitation is forwarded to the api correctly

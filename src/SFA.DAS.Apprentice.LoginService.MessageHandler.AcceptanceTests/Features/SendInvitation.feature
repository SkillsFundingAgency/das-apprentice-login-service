Feature: SendInvitation
	In order to allow apprentices to create accounts
	An invitaion is sent to the apprentices email address

Scenario: When account invitation is required for a new apprentice
	Given the login service has a client for this request
	Given the apprentice's email does not already exist
	When the SendInvitationCommand is received
	Then the invitation is recorded
	And the invitation is logged
	And the invitation email is sent 
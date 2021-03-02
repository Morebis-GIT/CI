@Channels
Feature: Channels API
	In order to check the Channels functionality
	As an API user
	I want to test available Channels API endpoints

Scenario: As an API user I want to request all channels
	Given I know how many channels there are
	When I add 5 channels
	Then 5 additional channels are returned

Scenario: As an API user I want to remove channel
	Given I know how many channels there are
	When I add 5 channels
	And I delete 2 channels
	Then 3 additional channels are returned

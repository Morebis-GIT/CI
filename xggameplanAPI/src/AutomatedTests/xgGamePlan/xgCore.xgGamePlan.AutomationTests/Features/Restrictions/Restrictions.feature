@Restrictions
Feature: Restrictions API
	In order to check the Restrictions functionality
	As an API user
	I want to test available Restrictions API endpoints

Background:
	Given I have a valid Sales Area
	And I have a valid Clearance Code

Scenario: As an API User I want to request Restrictions
	Given I know how many Restrictions there are
	When I add 2 Restrictions
	Then 2 additional Restrictions are returned

Scenario:  As an API User I want to remove all Restrictions
	Given I have added 3 Restrictions
	When I delete all Restrictions
	Then no Restrictions are returned

Scenario: As an API User I want to request Restriction by ID
	Given I have added 1 Restriction
	When I request my Restriction by ID
	Then requested Restriction with ID is returned

Scenario: As an API User I want to update Restriction by ExternalIdentifier
	Given I have added 1 Restriction
	When I update Restriction by ExternalIdentifier
	Then updated Restriction is returned

Scenario: As an API User I want to update Restriction by ID
	Given I have added 1 Restriction
	When I update Restriction by ID
	Then updated Restriction is returned

Scenario: As an API User I want to remove Restriction by ID
	Given I have added 1 Restriction
	When I remove my Restriction by ID
	Then no Restriction with ID is returned

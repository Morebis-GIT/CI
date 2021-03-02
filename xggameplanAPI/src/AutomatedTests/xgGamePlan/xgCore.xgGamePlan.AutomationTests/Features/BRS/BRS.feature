@BRS
Feature: BRS API
	In order to check the BRS functionality
	As an API user
	I want to test available BRS API endpoints

Scenario: As an API User I want to create BRS configuration template
	Given I know how many BRS configuration templates there are
	When I add 1 BRS configuration template
	Then 1 additional BRS configuration template are returned

Scenario: As an API User I want to update BRS configuration template by Id
	Given I have a valid BRS configuration template
	When I update BRS configuration template by Id
	Then BRS configuration template is updated

Scenario: As an API User I want to delete BRS configuration template
	Given I have a valid BRS configuration template
	When I delete BRS configuration template
	Then BRS configuration template is deleted

Scenario: As an API User I want to get BRS configuration template by Id
	Given I have a valid BRS configuration template
	When I get BRS configuration template by Id
	Then BRS configuration template is returned

Scenario: As an API User I want to set as default BRS configuration template by Id
	Given I have a valid BRS configuration template
	When I set as default any BRS configuration template by id
	Then BRS configuration template is default

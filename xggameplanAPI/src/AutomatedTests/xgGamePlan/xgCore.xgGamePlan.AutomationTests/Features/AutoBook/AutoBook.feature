@AutoBook
Feature: AutoBook API
	In order to check the AutoBook functionality
	As an API user
	I want to test available AutoBook API endpoints

Scenario: As an API User I want to request AutoBook instance configurations
	When I request AutoBook instance configurations
	Then not-empty configuration collection is returned

Scenario: As an API user I want to update AutoBook settings
	When I request AutoBook settings
	And I update AutoBook settings with new values
	Then updated AutoBook settings are returned

Scenario: As an API user I want to create new AutoBooks
	Given I know how many AutoBooks there are
	When I add 3 AutoBooks
	Then 3 additional AutoBooks are returned

Scenario: As an API user I want to request AutoBook by id
	Given I have added new AutoBook
	When I get AutoBook by id
	Then created AutoBook is returned

Scenario: As an API user I want to delete AutoBook
	Given I have added new AutoBook
	When I delete AutoBook
	And I get AutoBook by id
	Then no AutoBook returned

Scenario: As an API user I want to request Audit trial
	Given I have 1 valid Run
	And I have added new AutoBook
	When I get Audit trial for run's scenario
	Then not empty string returned
	
Scenario: As an API user I want to request Scenario logs
	Given I have 1 valid Run
	And I have added new AutoBook
	When I get logs for run's scenario
	Then not empty string returned

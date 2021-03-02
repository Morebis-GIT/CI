@LibrarySalesAreaPassPriorities
Feature: LibrarySalesAreaPassPriorities
	In order to check the LibrarySalesAreaPassPriorities functionality
	As an API user
	I want to test available LibrarySalesAreaPassPriorities API endpoints

Background:
	Given I have a valid Sales Area

Scenario: As an API user I want to request SalesAreaPassPriorities
	Given I know how many SalesAreaPassPriorities there are
	When I add 3 SalesAreaPassPriorities
	Then 3 additional SalesAreaPassPriorities are returned

Scenario: As an API user I want to request SalesAreaPassPriority by Id
	Given I have created new SalesAreaPassPriority
	When I request created SalesAreaPassPriority by id
	Then created SalesAreaPassPriority is returned

Scenario: As an API user I want to update SalesAreaPassPriority
	Given I have created new SalesAreaPassPriority
	When I update SalesAreaPassPriority
	Then updated SalesAreaPassPriority is returned

Scenario: As an API user I want to delete SalesAreaPassPriority by Id
	Given I have created new SalesAreaPassPriority
	When I delete created SalesAreaPassPriority by id
	And I request created SalesAreaPassPriority by id
	Then no SalesAreaPassPriority is returned

Scenario: As an API user I want to set default SalesAreaPassPriority
	Given I know initial default SalesAreaPassPriority
	And I have created new SalesAreaPassPriority
	When I set as default created SalesAreaPassPriority
	And I get default SalesAreaPassPriority
	Then updated default SalesAreaPassPriority is returned

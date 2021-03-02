@DeliveryCappingGroup
Feature: DeliveryCappingGroup API
	In order to check the DeliveryCappingGroup functionality
	As an API user
	I want to test available DeliveryCappingGroup API endpoints

Scenario: As an API User I want to create delivery capping group
	Given I know how many delivery capping groups there are
	When I add 1 delivery capping group
	Then 1 additional delivery capping groups are returned

Scenario: As an API User I want to update delivery capping group by Id
	Given I have a valid delivery capping group
	When I update delivery capping group by Id
	Then delivery capping group is updated

Scenario: As an API User I want to delete delivery capping group
	Given I have a valid delivery capping group
	When I delete delivery capping group
	Then delivery capping group is deleted

Scenario: As an API User I want to get delivery capping group by Id
	Given I have a valid delivery capping group
	When I get delivery capping group by Id
	Then delivery capping group is returned

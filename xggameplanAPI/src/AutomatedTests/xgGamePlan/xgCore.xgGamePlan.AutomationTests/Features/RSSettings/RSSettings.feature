@RSSettings
Feature: RSSettings API
	In order to check RSSettings functionality
	As an API user
	I want to test available RSSettings API endpoints

Background:
	Given I have a valid Sales Area

Scenario: As an API user I want to request RSSettings by salesArea
	Given I have created 1 RSSettings with valid salesArea
	When I request RSSettings by salesArea
	Then 1 RSSettings are returned

Scenario: As an API user I want to create RSSettings
	Given I know how many RSSettings with valid salesArea there are
	When I add 1 RSSettings
	Then 1 additional RSSettings are returned

Scenario Outline: As an API user I want to update RSSettings
	Given I have created 1 RSSettings with valid salesArea
	When I update RSSetting with mode '<Mode>'
	Then updated RSSetting returned

Examples:
	| Mode |
	| 0    |
	| 1    |
	| 2    |
	| 3    |

Scenario: As an API user I want to delete all RSSettings by salesArea
	Given I have created 1 RSSettings with valid salesArea
	When I delete all RSSettings by salesArea
	Then 0 RSSettings are returned

Scenario Outline: As an API user I want to compare RSSettings with different modes
	When I compare RSSettings with mode '<Mode>'
	Then the method succeeded

Examples:
	| Mode |
	| 0    |
	| 1    |
	| 2    |

@Users
Feature: Users
	In order to check the Users functionality
	As an API user
	I want to test available Users API endpoints

Scenario: As an API user I want to request Users
	Given I know how many Users there are
	When I add 2 Users
	Then 2 additional Users are returned

Scenario: As an API user I want to request User by ID
	Given I have added a User
	When I request my User by ID
	Then requested User with ID is returned

Scenario: As an API user I want to update User by ID
	Given I have added a User
	When I update User by ID
	Then updated User is returned

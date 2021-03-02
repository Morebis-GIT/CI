@Languages
Feature: Languages API
	In order to check the Languages functionality
	As an API user
	I want to test available Languages API endpoints

Scenario: As an API user I want to request all languages
	When I request all languages
	Then the method succeeded

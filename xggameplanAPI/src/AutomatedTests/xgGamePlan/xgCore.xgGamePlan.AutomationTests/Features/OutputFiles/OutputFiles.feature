@OutputFiles
Feature: OutputFiles API
	In order to check the OutputFiles functionality
	As an API user
	I want to test available OutputFiles API endpoints

Scenario: As an API user I want to get all OutputFiles
	When I get all OutputFiles
	Then not empty collection is returned

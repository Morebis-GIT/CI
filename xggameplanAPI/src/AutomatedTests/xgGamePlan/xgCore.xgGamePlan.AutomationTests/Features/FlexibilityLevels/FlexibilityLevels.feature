@Languages
Feature: FlexibilityLevels API
	In order to check the FlexibilityLevels functionality
	As an API user
	I want to test available FlexibilityLevels API endpoints

Scenario: As an API user I want to request all flexibility levels
	When I request all FlexibilityLevels
	Then the method succeeded

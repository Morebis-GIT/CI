@Smooth
Feature: Smooth API
	In order to check Smooth functionality
	As an API user
	I want to test available Smooth API endpoints

Background:
	Given I have a valid SmoothConfiguration

Scenario: As an API user I want to validate SmoothConfiguration
	When I validate SmoothConfiguration by id
	Then no error messages are returned

Scenario: As an API user I want to export SmoothConfiguration for best break factor groups
	When I export SmoothConfiguration for best break factor groups
	Then the method succeeded

Scenario: As an API user I want to export SmoothConfiguration for passes
	When I export SmoothConfiguration for passes
	Then the method succeeded

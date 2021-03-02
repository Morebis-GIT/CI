@Languages
Feature: FeatureFlags API
	In order to check the FeatureFlags functionality
	As an API user
	I want to test available FeatureFlags API endpoints

Scenario: As an API user I want to request all feature flags
	When I request all FeatureFlags
	Then the method succeeded

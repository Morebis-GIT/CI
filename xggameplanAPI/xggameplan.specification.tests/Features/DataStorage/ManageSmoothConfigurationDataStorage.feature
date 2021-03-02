@ManageDataStorage

Feature: Manage smooth configuration data storage
	In order to configure smooth process
	As a user
	I want to store smooth configuration in a data store

Background:
	Given there is a SmoothConfiguration repository

Scenario: Add new SmoothConfiguration
	Given the following documents created:
		| Id | Version | RestrictionCheckEnabled | ClashExceptionCheckEnabled | RecommendationsForExcludedCampaigns | SmoothFailuresForExcludedCampaigns |
		| 1  | 1.0.0.1 | True                    | True                       | False                               | False                              |
	When I get document with id 1
	Then there should be 1 documents returned

Scenario: Get a SmoothConfiguration by id
	Given the following documents created:
		| Id | Version | RestrictionCheckEnabled | ClashExceptionCheckEnabled | RecommendationsForExcludedCampaigns | SmoothFailuresForExcludedCampaigns |
		| 1  | 1.0.0.1 | True                    | True                       | False                               | False                              |
		| 2  | 1.0.0.2 | True                    | True                       | False                               | False                              |
		| 3  | 1.0.0.3 | True                    | True                       | False                               | False                              |
	When I get document with id 2
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property                            | Value |
		| Id                                  | 2     |
		| RestrictionCheckEnabled             | True  |
		| ClashExceptionCheckEnabled          | True  |
		| RecommendationsForExcludedCampaigns | False |
		| SmoothFailuresForExcludedCampaigns  | False |

Scenario: Get a non-existing SmoothConfiguration by id
	Given the following documents created:
		| Id | Version | RestrictionCheckEnabled | ClashExceptionCheckEnabled | RecommendationsForExcludedCampaigns | SmoothFailuresForExcludedCampaigns |
		| 1  | 1.0.0.1 | True                    | True                       | False                               | False                              |
		| 2  | 1.0.0.2 | True                    | True                       | False                               | False                              |
		| 3  | 1.0.0.3 | True                    | True                       | False                               | False                              |
	When I get document with id 22
	Then no documents should be returned

Scenario: Update a SmoothConfiguration
	Given the following documents created:
		| Id | Version | RestrictionCheckEnabled | ClashExceptionCheckEnabled | RecommendationsForExcludedCampaigns | SmoothFailuresForExcludedCampaigns |
		| 1  | 1.0.0.1 | True                    | True                       | False                               | False                              |
		| 2  | 1.0.0.2 | True                    | True                       | False                               | False                              |
		| 3  | 1.0.0.3 | True                    | True                       | False                               | False                              |
	When I get document with id 2
	And I update received document by values:
		| Property                            | Value |
		| RestrictionCheckEnabled             | False |
		| ClashExceptionCheckEnabled          | False |
		| RecommendationsForExcludedCampaigns | True  |
		| SmoothFailuresForExcludedCampaigns  | True  |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property                            | Value |
		| RestrictionCheckEnabled             | False |
		| ClashExceptionCheckEnabled          | False |
		| RecommendationsForExcludedCampaigns | True  |
		| SmoothFailuresForExcludedCampaigns  | True  |

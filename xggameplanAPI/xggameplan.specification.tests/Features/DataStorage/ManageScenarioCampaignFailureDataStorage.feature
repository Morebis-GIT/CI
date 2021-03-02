@ManageDataStorage

Feature: ManageScenarioCampaignFailureDataStorage
	In order to manage scenariocampaignfailures
	As a user
	I want to store scenariocampaignfailures via ScenarioCampaignFailure repository

Background:
	Given there is a ScenarioCampaignFailures repository

Scenario: Add new ScenarioCampaignFailure
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new ScenarioCampaignFailures
	When I create the following documents:
		| Id | ScenarioId                           | CampaignExternalId |
		| 1  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 15G1555937         |
		| 2  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 15G1552996         |
		| 3  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 30G1554492         |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Remove ScenarioCampaignFailure
	Given the following documents created:
		| Id | ScenarioId                           | CampaignExternalId |
		| 1  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 15G1555937         |
		| 2  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 15G1552996         |
		| 3  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 30G1554492         |
	When I delete document with id '1'
	And I get document with id '1'
	Then no documents should be returned

Scenario: Get a non-existing ScenarioCampaignFailure by id
	Given the following documents created:
		| Id | ScenarioId                           | CampaignExternalId |
		| 1  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 15G1555937         |
		| 2  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 15G1552996         |
		| 3  | bf7bb3d7-059b-46d1-be5b-c7e2a1195fc7 | 30G1554492         |
	When I get document with id '4'
	Then no documents should be returned
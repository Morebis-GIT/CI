@ManageDataStorage

Feature: ManageScenarioCampaignMetricDataStorage
	In order to manage scenariocampaignmetric
	As a user
	I want to store scenariocampaignmetric via ScenarioCampaignMetric repository

Background:
	Given there is a ScenarioCampaignMetrics repository

Scenario: Add new ScenarioCampaignMetric
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get all ScenarioCampaignMetric
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Remove ScenarioCampaignMetric
	Given the following documents created:
		| Id                                   |
		| 2d661bfa-93e4-4123-9cc8-a2541c53f763 |
		| fafd3821-1684-445b-863f-fe4fcefa7c35 |
		| e51a653d-aab9-4aa7-97a2-24594cb49dc9 |
	When I delete document with id '2d661bfa-93e4-4123-9cc8-a2541c53f763'
	And I get document with id '2d661bfa-93e4-4123-9cc8-a2541c53f763'
	Then no documents should be returned

Scenario: Get a non-existing ScenarioCampaignMetric by id
	Given the following documents created:
		| Id                                   |
		| 2d661bfa-93e4-4123-9cc8-a2541c53f763 |
		| fafd3821-1684-445b-863f-fe4fcefa7c35 |
		| e51a653d-aab9-4aa7-97a2-24594cb49dc9 |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned
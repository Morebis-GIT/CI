@ManageDataStorage

Feature: ManageScenarioResultsStorage
	In order to manage ScenarioResults
	As a user
	I want to store scenario results via ScenarioResults repository

Background:
	Given there is a ScenarioResults repository

Scenario: Add new ScenarioResult
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get a non-existing ScenarioResult by id
	Given the following documents created:
		| Id                                   | TimeCompleted |
		| d2406c94-721a-4d3c-9d47-e632080f581a | 2010-02-03    |
		| 771be036-87f7-4013-b061-a44f6057325a | 2012-02-02    |
		| d037229b-1b70-459d-bda9-876b91e6888d | 2014-10-01    |
	When I get document with id 'd3617be3-3505-4aa6-a9b5-7b41d33612cf'
	Then no documents should be returned

Scenario: Get an existing ScenarioResult by id
	Given the following documents created:
		| Id                                   | TimeCompleted |
		| d2406c94-721a-4d3c-9d47-e632080f581a | 2010-02-03    |
		| 771be036-87f7-4013-b061-a44f6057325a | 2012-02-02    |
		| d037229b-1b70-459d-bda9-876b91e6888d | 2014-10-01    |
	When I get document with id '771be036-87f7-4013-b061-a44f6057325a'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property      | Value                                |
         | Id            | 771be036-87f7-4013-b061-a44f6057325a |
         | TimeCompleted | 2012-02-02                           |

Scenario: Get all ScenarioResults
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Remove an existing ScenarioResult
	Given the following documents created:
		| Id                                   | TimeCompleted |
		| d2406c94-721a-4d3c-9d47-e632080f581a | 2010-02-03    |
		| 771be036-87f7-4013-b061-a44f6057325a | 2012-02-02    |
		| d037229b-1b70-459d-bda9-876b91e6888d | 2014-10-01    |
	When I delete document with id '771be036-87f7-4013-b061-a44f6057325a'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing a non-existing ScenarioResult
	Given the following documents created:
		| Id                                   | TimeCompleted |
		| d2406c94-721a-4d3c-9d47-e632080f581a | 2010-02-03    |
		| 771be036-87f7-4013-b061-a44f6057325a | 2012-02-02    |
		| d037229b-1b70-459d-bda9-876b91e6888d | 2014-10-01    |	
	When I delete document with id '0107f291-ed15-48cc-a118-6db3caa4327a'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Update ScenarioResult
	Given the following documents created:
		| Id                                   | TimeCompleted |
		| d2406c94-721a-4d3c-9d47-e632080f581a | 2010-02-03    |
		| 771be036-87f7-4013-b061-a44f6057325a | 2012-02-02    |
		| d037229b-1b70-459d-bda9-876b91e6888d | 2014-10-01    |	
	When I get document with id '771be036-87f7-4013-b061-a44f6057325a'
	And I update received document by values:
         | Property      | Value      |
         | TimeCompleted | 2019-01-01 |
	And I get document with id '771be036-87f7-4013-b061-a44f6057325a'
	Then the received document should contain the following values:
         | Property      | Value                                |
         | Id            | 771be036-87f7-4013-b061-a44f6057325a |
         | TimeCompleted | 2019-01-01                           |     

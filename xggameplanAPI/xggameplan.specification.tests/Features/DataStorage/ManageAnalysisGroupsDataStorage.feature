@ManageDataStorage

Feature: Manage AnalysisGroup data storage
	In order to manage AnalysisGroups
	As an Airtime manager
	I want to store AnalysisGroups in a data store
	
Background: 
	Given there is a AnalysisGroups repository
	
Scenario: Get all AnalysisGroups
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I get all documents
	Then there should be 3 documents returned
	
Scenario: Get an existing AnalysisGroup by id
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I get document with id 1
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property  | Value                |
		| Id        | 1                    |
		| Name      | First analysis group |
	
Scenario: Get a nonexistent AnalysisGroup by id
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I get document with id 5
	Then no documents should be returned
	
Scenario: Add new AnalysisGroup
	When I create a document
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Update exiting AnalysisGroup
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I get document with id 2
	And I update received document by values:
		| Property | Value        |
		| Name     | Updated name |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property | Value        |
		| Name     | Updated name |

Scenario: Deactivate AnalysisGroup by id
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I get document with id 3
	And I update received document by values:
		| Property  | Value |
		| IsDeleted | true  |
	And I get document with id 3
	Then no documents should be returned
	
Scenario: Delete AnalysisGroup by id
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I delete document with id 3
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a nonexistent AnalysisGroup by id
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I delete document with id 5
	And I get all documents
	Then there should be 3 documents returned

Scenario Outline: Get AnalysisGroup by name
	Given the following documents created:
		| Id | Name   |
		| 1  | First  |
		| 2  | Second |
		| 3  | Third  |
	When I call GetByName method with parameters:
		| Parameter | Value |
		| name      | <Name> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Name   | ExpectedReturnCount |
	| First  | 1                   |
	| Second | 1                   |
	| Third  | 1                   |
	| Test   | 0                   |

Scenario Outline: Get AnalysisGroups by ids
	Given the following documents created:
		| Id | Name                  |
		| 1  | First analysis group  |
		| 2  | Second analysis group |
		| 3  | Third analysis group  |
	When I call GetByIds method with parameters:
         | Parameter  | Value        |
         | ids        | <Ids>        |
         | onlyActive | <OnlyActive> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids      | OnlyActive | ExpectedReturnCount |
	| 1        | false      | 1                   |
	| 1, 3     | false      | 2                   |
	| 1, 3, 2  | false      | 3                   |
	| 1, 3, 7  | false      | 2                   |
	| 9, 6, 13 | false      | 0                   |
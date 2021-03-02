@ManageDataStorage

Feature: ManageFunctionalAreaDataStorage
	In order to manage FunctionalAreas
	As an Airtime manager
	I want to store FunctionalAreas via FunctionalArea repository

Background:
	Given there is a FunctionalAreas repository

Scenario: Add new FunctionalArea
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get all FunctionalAreas
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get an existing functional area by id
	Given the following documents created:
		| Id                                   |
		| 720906C7-EB98-4057-9175-962AF459EA55 |
		| 2259294C-24E6-45B7-B1EF-2BAA53CF4278 |
		| 6D5304D7-5848-4577-8F31-65305E8DA0BD |
	When I get document with id '6D5304D7-5848-4577-8F31-65305E8DA0BD'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value                                |
		| Id       | 6D5304D7-5848-4577-8F31-65305E8DA0BD |

Scenario: Get a non-existing functional area by id
	Given the following documents created:
		| Id                                   |
		| 720906C7-EB98-4057-9175-962AF459EA55 |
		| 2259294C-24E6-45B7-B1EF-2BAA53CF4278 |
		| 6D5304D7-5848-4577-8F31-65305E8DA0BD |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

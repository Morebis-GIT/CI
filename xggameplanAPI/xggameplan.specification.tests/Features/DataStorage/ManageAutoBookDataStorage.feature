@ManageDataStorage

Feature: Manage AutoBook data storage
	In order to manage AutoBook
	As an administrator
	I want to store AutoBooks in a data store

Background: 
	Given there is a AutoBooks repository

Scenario: Add new AutoBook
	Given 1 documents created
	When I get all documents
	Then there should be 1 documents returned

Scenario: Counting all AutoBooks
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Get existing AutoBook by id
	Given the following documents created:
        | Id                                             |
        | AutoBooks/c33b0836-6c1f-4c95-aea6-8d0fa5364585 |
        | AutoBooks/3f398657-4136-4eeb-b3e0-1e52c2e9e24c |
        | AutoBooks/97b55faa-0cf2-4fb6-9dca-cd822b883d8b |
	When I get document with id 'c33b0836-6c1f-4c95-aea6-8d0fa5364585'
	Then there should be 1 documents returned

Scenario: Get non-existing AutoBook by id
	Given the following documents created:
        | Id                                             |
        | AutoBooks/c33b0836-6c1f-4c95-aea6-8d0fa5364585 |
        | AutoBooks/3f398657-4136-4eeb-b3e0-1e52c2e9e24c |
        | AutoBooks/97b55faa-0cf2-4fb6-9dca-cd822b883d8b |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

Scenario: Update AutoBook
	Given the following documents created:
        | Id                                             | Api     | Locked |
        | AutoBooks/c33b0836-6c1f-4c95-aea6-8d0fa5364585 | Initial | True   |
        | AutoBooks/3f398657-4136-4eeb-b3e0-1e52c2e9e24c | Api     | False  |
        | AutoBooks/97b55faa-0cf2-4fb6-9dca-cd822b883d8b | NoApi   | False  |
	When I get document with id 'c33b0836-6c1f-4c95-aea6-8d0fa5364585'
	Then there should be 1 documents returned
	When I update received document by values:
        | Parameter | Value   |
        | Api       | Updated |
        | Locked    | False   |
	And I get document with id 'c33b0836-6c1f-4c95-aea6-8d0fa5364585'
	Then the received document should contain the following values:
        | Parameter | Value   |
        | Api       | Updated |
        | Locked    | False   |

Scenario: Delete existing AutoBook
	Given the following documents created:
        | Id                                             |
        | AutoBooks/c33b0836-6c1f-4c95-aea6-8d0fa5364585 |
        | AutoBooks/3f398657-4136-4eeb-b3e0-1e52c2e9e24c |
        | AutoBooks/97b55faa-0cf2-4fb6-9dca-cd822b883d8b |
	When I delete document with id 'AutoBooks/c33b0836-6c1f-4c95-aea6-8d0fa5364585'
	And I get document with id 'c33b0836-6c1f-4c95-aea6-8d0fa5364585'
	Then no documents should be returned

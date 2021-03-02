@ManageDataStorage

Feature: ManageTenantsDataStorage
	In order to manage Tenants
	As an api user
	I want to store Tenant via Tenants repository

Background:
	Given there is a Tenants repository

Scenario: Add new Tenant
	When I create a document with values:
		| Id | Name    | DefaultTheme |
		| 1  | Tenant1 | Black        |
	And I get document with id 1
	Then there should be 1 documents returned

Scenario: Get all Tenants
	Given the following documents created:
		| Id | Name    | DefaultTheme |
		| 1  | Tenant1 | Black        |
		| 2  | Tenant2 | White        |
		| 3  | Tenant3 | Grey         |
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing Tenant by id
	Given the following documents created:
		| Id | Name    | DefaultTheme |
		| 1  | Tenant1 | Black        |
		| 2  | Tenant2 | White        |
		| 3  | Tenant3 | Grey         |
	When I get document with id 45
	Then no documents should be returned

Scenario: Get an existing Tenant by id
	Given the following documents created:
		| Id | Name    | DefaultTheme |
		| 1  | Tenant1 | Black        |
		| 2  | Tenant2 | White        |
		| 3  | Tenant3 | Grey         |
	When I get document with id 2
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property     | Value   |
         | Id           | 2       |
         | Name         | Tenant2 |
         | DefaultTheme | White   |

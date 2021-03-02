@ManageDataStorage

Feature: ManageBusinessTypeDataStorage
In order to manage BusinessTypes
As a user
I want to store businessTypes via BusinessTypes repository

Background:
	Given there is a BusinessTypes repository
	And predefined BusinessTypes data

Scenario: Get BusinessTypes
	Given predefined data imported
	When  I call GetAll method
	Then there should be 3 documents returned

Scenario: Get a non-existing BusinessType by code
	Given predefined data imported
	When I call GetByCode method with parameters:
		| Parameter | Value |
		| code      | ABC   |
	Then no documents should be returned

Scenario: Get an existing BusinessType by code
	Given predefined data imported
	When I call GetByCode method with parameters:
		| Parameter | Value |
		| code      | STD   |
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value |
		| Id       | 1     |
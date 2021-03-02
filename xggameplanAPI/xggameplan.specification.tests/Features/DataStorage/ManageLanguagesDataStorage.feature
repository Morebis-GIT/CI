@ManageDataStorage

Feature: ManageLanguagesDataStorage
	In order to manage languages
	As a user
	I want to store languages via Languages repository

Background:
	Given there is a Languages repository

Scenario: Add new language
	Given 1 documents created
	When I create 2 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all languages
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

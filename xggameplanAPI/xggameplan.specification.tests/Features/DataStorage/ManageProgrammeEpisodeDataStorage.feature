@ManageDataStorage


Feature: Manage ProgrammeEpisode data storage
	In order to manage ProgrammeEpisodes
	As an Airtime manager
	I want to store ProgrammeEpisodes in a data store

Background:
	Given there is a ProgrammeEpisodes repository

Scenario: Get all ProgrammeEpisodes
	Given the following documents created:
	| Id | ProgrammeExternalReference | Name           | Number |
	| 1  | HCN24                      | HCN24-Episode1 | 1      |
	| 2  | HCN24                      | HCN24-Episode2 | 2      |
	When I get all documents
	Then there should be 2 documents returned

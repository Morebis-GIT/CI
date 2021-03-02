@ManageDataStorage

Feature: ManageKPIComparisonConfigDataStorage
	In order to manage KPIComparisonConfig
	As a user
	I want to store KPIComparisonConfig via KPIComparisonConfig repository

Background: 
	Given there is a KPIComparisonConfig repository
	And predefined KPIComparisonConfig data

Scenario: Get all Metadata
	Given predefined data imported
	When I get all documents
	Then there should be 4 documents returned

@Campaigns
Feature: Campaigns API
	In order to check the Campaigns functionality
	As an API user
	I want to test available Campaigns API endpoints

Background:
	Given I have a valid Break Types "BASE,PREMIUM"
	And I have a valid Demographic
	And I have a valid Product
	And I have a valid Sales Area

Scenario: As an API User I want to create Campaigns
	Given I know how many Campaigns there are
	When I add 3 Campaigns
	Then 3 additional Campaigns are returned

Scenario: As an API User I want to search Campaigns
	Given I know how many active Campaigns there are
	When I add 4 active Campaigns
	And I search active Campaigns
	Then 4 additional active Campaigns are returned

Scenario: As an API User I want to get Campaigns by group
	Given I know how many Campaigns in group 'General'
	When I add 4 Campaigns in group 'General'
	And I get Campaigns in group 'General'
	Then 4 additional Campaigns in group are returned

Scenario: As an API User I want to get Campaign by externalRef
	Given I have added 5 Campaigns
	And I know Campaign externalRef
	When I get Campaign by externalRef
	Then Campaign is returned

Scenario: As an API User I want to get Campaign by id
	Given I have added 5 Campaigns
	And I know Campaign id
	When I get Campaign by id
	Then Campaign is returned

Scenario:  As an API User I want to remove all Campaigns
	Given I have added 5 Campaigns
	When I delete all Campaigns
	Then no Campaigns are returned

Scenario: As an API User I want to update Campaign by External Id
	Given I have added 1 Campaigns
	And I know Campaign external id
	When I update Campaign by external id
	Then updated Campaign is returned

Scenario: As an API User I want to create Campaign with invalid Lengths (invalid DesiredPercentageSplit) in Day Part
	Given I have an invalid DesiredPercentageSplit in Length of Campaign
	When I update Campaign with invalid percentage split value in day part length
	Then error response is returned

Scenario: As an API User I want to create Campaign with invalid Lengths (invalid CurrentPercentageSplit) in Day Part
	Given I have an invalid CurrentPercentageSplit in Length of Campaign
	When I update Campaign with invalid percentage split value in day part length
	Then error response is returned

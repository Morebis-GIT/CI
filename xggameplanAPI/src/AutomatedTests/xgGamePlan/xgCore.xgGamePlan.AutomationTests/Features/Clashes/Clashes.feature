@Clashes
Feature: Clashes API
	In order to check the Clashes functionality
	As an API user
	I want to test available Clashes API endpoints

Scenario: As an API User I want to add and request Clashes
	Given I know how many Clashes there are
	When I add 4 Clashes
	Then 4 additional Clashes are returned

Scenario: As an API User I want to search Clashes
	Given I know how many Clashes with description 'Test' there are
	When I add 4 Clashes with the 'Test' description
	And I search Clashes by 'Test' description
	Then 4 additional Clashes are found

Scenario: As an API User I want to update Clash which has Differences with new default exposure values globally
	Given I have added Clash with difference for sales area 'DIFFERENCE_SA' and external reference 'CWDG'
	When I update Clash default peek and off-peak exposure count using external reference 'CWDG' and Apply Globally set to 'True'
	Then There are no differences in returned Clash

Scenario: As an API User I want to update Clash which has Differences with new default exposure values not globally
	Given I have added Clash with difference for sales area 'DIFFERENCE_SA' and external reference 'CWDNG'
	When I update Clash default peek and off-peak exposure count using external reference 'CWDNG' and Apply Globally set to 'False'
	Then There is difference for Sales Area 'DIFFERENCE_SA' in returned Clash

Scenario: As an API User I want to remove Clash which has child Clash by external reference
	Given I have added Clashes with external references 'H_C_CL', 'C_CL'
	When I delete Clash by external reference 'H_C_CL'
	Then error response is received with message 'Clash cannot be deleted – in use'

Scenario: As an API User I want to remove Clash which is linked to active Product by external reference
	Given I have added Clash with external reference 'LD_TPC'
	And I have added Product with Clash Code 'LD_TPC'
	When I delete Clash by external reference 'LD_TPC'
	Then error response is received with message 'Clash cannot be deleted – in use'

Scenario: As an API User I want to remove a valid Clash by external reference
	Given I have added Clash with external reference 'VC_F_D'
	When I delete Clash by external reference 'VC_F_D'
	Then ok response is received

Scenario:  As an API User I want to remove all Clashes
	Given I have added 7 Clashes
	When I delete all Clashes
	Then no Clashes are returned

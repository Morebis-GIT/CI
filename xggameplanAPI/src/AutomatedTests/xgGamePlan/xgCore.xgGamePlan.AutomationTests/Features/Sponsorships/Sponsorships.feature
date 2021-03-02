@Sponsorships
Feature: Sponsorships API
	In order to check the Sponsorships functionality
	As an API user
	I want to test available Sponsorships API endpoints

Background:
	Given I have some Products with same Advertiser Identifier
	Given I have a valid Sales Area
	Given I have a valid Clash

Scenario: As an API User I want to create Sponsorships
	Given I know how many Sponsorships there are
	When I add 3 Sponsorships
	Then 3 additional Sponsorships are returned

Scenario: As an API User I want to update Sponsorships
	Given I add an Sponsorship with 1 SponsoredItem
	When I try to update Sponsorship to add another SponsoredItem
	Then I get Sponsorship
	And Sponsorship has 2 SponsoredItems

Scenario: As an API User I want to delete a single Sponsorship
	Given I add an Sponsorship 
	When I delete that Sponsorship
	Then that Sponsorship is not returned

Scenario: As an API User I want to delete all Sponsorships
	Given I know there are some Sponsorships
	When I delete all Sponsorships
	Then No Sponsorships are returned

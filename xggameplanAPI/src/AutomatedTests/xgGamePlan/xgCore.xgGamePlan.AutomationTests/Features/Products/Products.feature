@Products
Feature: Products API
	In order to check the Products functionality
	As an API user
	I want to test available Products API endpoints

Scenario: As an API User I want to create Products
	Given I know how many Products there are
	When I add 3 Products
	Then 3 additional Products are returned

Scenario: As an API User I want to create Product by external Id
	Given I know how many Products there are
	And I have a Product with not existing external id
	When I upsert Product by external id
	Then 1 additional Products are returned

Scenario: As an API User I want to update Product by external Id
	Given I have a valid Product
	When I upsert Product by external id
	Then updated Product is returned

Scenario: As an API User I want to search Products
	Given I have added 5 Products
	And I know how many Products with Name there are
	When I add 4 Products with Name
	And I search for Products with Name
	Then 4 additional Products are found

Scenario: As an API User I want to search Products by advertiser name
	Given I have added 5 Products
	And I know how many Products with Advertiser Name there are
	When I add 3 Products with Advertiser Name
	And I search Products by Advertiser Name
	Then 3 additional Products are found

Scenario: As an API User I want to get Product by externalRef
	Given I have added 4 Products
	And I know Product externalRef
	When I get Product by externalRef
	Then Product is returned

Scenario:  As an API User I want to remove all Products
	Given I have added 5 Products
	When I delete all Products
	Then no Products are returned

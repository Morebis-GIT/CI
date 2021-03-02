@Tenants
Feature: Tenants
	In order to check the Tenants functionality
	As an API user
	I want to test available Tenants API endpoints

Scenario: As an API user I want to request Tenants
	Given I know how many Tenants there are
	When I add 2 Tenants
	Then 2 additional Tenants are returned

Scenario: As an API User I want to request Tenant by ID
	Given I have added a Tenant
	When I request my Tenant by ID
	Then requested Tenant with ID is returned

Scenario: As an API User I want to update Tenant by ID
	Given I have added a Tenant
	When I update Tenant by ID
	Then updated Tenant is returned

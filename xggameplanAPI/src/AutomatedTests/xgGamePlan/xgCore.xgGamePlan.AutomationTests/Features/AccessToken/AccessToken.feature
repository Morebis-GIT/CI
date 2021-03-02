@AccessToken
Feature: AccessTokens API
	In order to check the AccessTokens functionality
	As an API user
	I want to test available AccessTokens API endpoints

Scenario: As an API user I want to create AccessToken with valid user credentials
    Given I have valid user credentials
    When I add new AccessToken
    Then new AccessToken returned

Scenario: As an API user I want to create AccessToken with invalid user credentials
    Given I have invalid user credentials
    When I try to add new AccessToken
    Then Unauthorized is returned by server

Scenario: As an API user I want to remove accessToken
    Given I have created new AccessToken
    When I remove AccessToken
    Then the method succeeded

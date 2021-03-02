@ManageDataStorage

Feature: Manage AccessToken data storage
	In order to manage AccessTokens
	As an administrator
	I want to store tokens in a data store

Background: 
	Given there is a AccessTokens repository

Scenario: Add new AccessToken
	Given the following documents created:
		| Token                                | UserId | ValidUntil          | ValidUntilValue     |
		| 06c842cf-66c1-4794-a1a6-da69bc6ab458 | 1      | 2019-03-29 04:15:00 | 2019-04-29 04:15:00 |
	When I get document with id '06c842cf-66c1-4794-a1a6-da69bc6ab458'
	Then there should be 1 documents returned

Scenario: Remove AccessToken
	Given the following documents created:
		| Token                                | UserId | ValidUntil          | ValidUntilValue     |
		| 06c842cf-66c1-4794-a1a6-da69bc6ab458 | 1      | 2019-03-29 04:15:00 | 2019-04-29 04:15:00 |
		| 49b453f4-2732-4866-a996-afa628f620b5 | 1      | 2019-03-29 05:15:00 | 2019-04-29 05:15:00 |
		| f8709529-5c10-40df-adfd-36769480dafe | 1      | 2019-03-29 07:15:00 | 2019-04-29 07:15:00 |
    When I delete document with id '49b453f4-2732-4866-a996-afa628f620b5'
    And I get document with id '49b453f4-2732-4866-a996-afa628f620b5'
	Then no documents should be returned

Scenario: Get AccessToken
	Given the following documents created:
		| Token                                | UserId | ValidUntil          | ValidUntilValue     |
		| 06c842cf-66c1-4794-a1a6-da69bc6ab458 | 1      | 2019-03-29 04:15:00 | 2019-04-29 04:15:00 |
		| 49b453f4-2732-4866-a996-afa628f620b3 | 1      | 2019-03-29 05:15:00 | 2019-04-29 05:15:00 |
		| f8709529-5c10-40df-adfd-36769480dafe | 1      | 2019-03-29 07:15:00 | 2019-04-29 07:15:00 |
	When I get document with id '49b453f4-2732-4866-a996-afa628f620b3'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property        | Value                                |
		| Token           | 49b453f4-2732-4866-a996-afa628f620b3 |

Scenario: Remove all expired AccessTokens
	Given the following documents created:
		| Token                                | UserId | ValidUntil          | ValidUntilValue     |
		| 06c842cf-66c1-4794-a1a6-da69bc6ab458 | 1      | 2000-03-29 04:15:00 | 2000-04-29 04:15:00 |
		| 49b453f4-2732-4866-a996-afa628f620b3 | 1      | 2500-03-29 05:15:00 | 2100-04-29 05:15:00 |
		| f8709529-5c10-40df-adfd-36769480dafe | 1      | 2019-03-29 07:15:00 | 2019-04-29 07:15:00 |
	When I call RemoveAllExpired method
	And I get document with id '06c842cf-66c1-4794-a1a6-da69bc6ab458'
	Then no documents should be returned
	When I get document with id '49b453f4-2732-4866-a996-afa628f620b3'
	Then there should be 1 documents returned

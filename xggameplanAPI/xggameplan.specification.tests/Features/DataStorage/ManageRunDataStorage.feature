@ManageDataStorage

Feature: ManageRunStorage
	In order to manage runs
	As a user
	I want to store runs via Runs repository

Background:
	Given there is a Runs repository
	And predefined Runs data

Scenario: Add new Run
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get a non-existing Run by id
	Given the following documents created:
		| Id                                   | Name  | ShortName |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

Scenario: Get an existing Run by id
	Given the following documents created:
		| Id                                   | Name  | ShortName |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       |
	When I get document with id '6DAD8F17-04E5-447D-9DEA-1DEA6950A40E'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value                                |
		| Id       | 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E |

Scenario: Get an existing manual Run by id
	Given the following documents created:
		| Id                                   | Name  | ShortName | Manual |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | false  |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | true   |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | false  |
	When I get document with id '6DAD8F17-04E5-447D-9DEA-1DEA6950A40E'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value |
		| Manual   | true  |

#update
Scenario: Update Run
	Given the following documents created:
		| Id                                   | Description   |
		| d2406c94-721a-4d3c-9d47-e632080f581a | Description_1 |
		| 771be036-87f7-4013-b061-a44f6057325a | Description_2 |
		| d037229b-1b70-459d-bda9-876b91e6888d | Description_3 |
	When I get document with id '771be036-87f7-4013-b061-a44f6057325a'
	And I update received document by values:
		| Property    | Value         |
		| Description | Description_4 |
	And I get document with id '771be036-87f7-4013-b061-a44f6057325a'
	Then the received document should contain the following values:
		| Property    | Value                                |
		| Id          | 771be036-87f7-4013-b061-a44f6057325a |
		| Description | Description_4                        |

Scenario: Get all Runs
	Given predefined data imported
	When I get all documents
	Then there should be 9 documents returned

Scenario: Get all active Runs
	Given predefined data imported
	When I call GetAllActive method
	Then there should be 3 documents returned

Scenario: Delete an existing Run by id
	Given the following documents created:
		| Id                                   | Name  | ShortName |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       |
	When I delete document with id '423D0C30-4661-4132-99ED-56ED9C64F205'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a non-existing Run by id
	Given the following documents created:
		| Id                                   | Name  | ShortName |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       |
	When I delete document with id '3f78817a-44da-49d6-a29b-06d3003a5256'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Find Run by scenario id
	Given predefined data imported
	When I call FindByScenarioId method with parameters:
		| Parameter  | Value                                |
		| scenarioId | e6ffb1a9-4337-4302-9595-bc3cbf8aafc7 |
	Then the received document should contain the following values:
		| Property | Value                                |
		| Id       | 84269516-e193-4a04-a038-361ae19677f5 |

Scenario: Get Runs with scenario id
	Given predefined data imported
	When I call GetRunsWithScenarioId method
	Then there should be 11 documents returned

Scenario Outline: Get Run id by scenario id
	Given predefined data imported
	When I call GetRunIdForScenario method with parameters:
		| Parameter  | Value        |
		| scenarioId | <ScenarioId> |
	Then result should be <RunId>

	Examples:
		| ScenarioId                           | RunId                                |
		| 0dfb8dcf-1ca3-4e87-9df2-fdf7964c4cfe | cf676037-3165-4481-ad7f-9457a01c304c |
		| 4044d11e-645f-4a91-b1c5-af1806719075 | 0c39f19b-4a9d-4897-b296-7bece26c251b |
		| 15a600c8-2f57-4c8c-9bd1-aa74361d5d1b | 0c39f19b-4a9d-4897-b296-7bece26c251b |
		| be2fe9d3-8a2b-49e2-8f54-43a7e9ff554e | cdb79f25-f141-4f41-8647-833d6bd39e33 |
		| da05770b-8960-4595-a366-018f6b4fbfe4 | 00000000-0000-0000-0000-000000000000 |

Scenario Outline: Get Run by scenario id
	Given predefined data imported
	When I call GetByScenarioId method with parameters:
		| Parameter  | Value        |
		| scenarioId | <ScenarioId> |
	Then there should be <ExpectedCount> documents returned

	Examples:
		| ScenarioId                           | ExpectedCount |
		| c5e458f6-4c2f-4fe1-bd3c-5350f5faaa2e | 0             |
		| 54a3a095-5e3d-4f35-98ea-e18b5f7fbd1c | 1             |
		| 00000000-0000-0000-0000-000000000000 | 0             |

Scenario Outline: Get Run by campaigns ids and status
	Given predefined data imported
	When I call GetRunsByCampaignExternalIdsAndStatus method with parameters:
		| Parameter         | Value             |
		| externalIds       | <ExternalIds>     |
		| runStatus         | <RunStatus>       |
	Then there should be <ExpectedCount> documents returned

	Examples:
		| ExternalIds                                                                   | RunStatus  | ExpectedCount |
		|                                                                               | InProgress | 1             |
		| 44c6b6a4-bf54-46b8-89f0-55ed22c70099                                          | InProgress | 2             |
		| 44c6b6a4-bf54-46b8-89f0-55ed22c70099, 0551ee9e-0cf3-47f0-9e8c-aa3e6a5cfbee    | InProgress | 2             |
		| 44c6b6a4-bf54-46b8-89f0-55ed22c70099                                          | Complete   | 2             |
		| 1faf8ac4-b558-4d19-ba35-36fb23d996fd                                          | Complete   | 1             |
		|                                                                               | Errors     | 1             |

Scenario Outline: Search runs
	Given predefined data imported
	When I call Search method with parameters:
		| Parameter           | Value                |
		| description         | <Description>        |
		| executedStartDate   | <StartDate>          |
		| executedEndDate     | <EndDate>            |
		| status              | <Status>             |
		| howManyWordsToMatch | <WordsToMatch>       |
		| wordOrder           | <WordOrder>          |
		| wordComparison      | <WordComparison>     |
		| caseSensitive       | <CaseSensitive>      |
		| charactersToIgnore  | <CharactersToIgnore> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| Description  | StartDate  | EndDate    | Status                 | WordsToMatch | WordOrder  | WordComparison | CaseSensitive | CharactersToIgnore | ExpectedReturnCount |
		| not exist    | 2020-01-15 | 2020-01-15 | NotStarted             | AnyWord      | AnyOrder   | ContainsWord   | True          |                    | 0                   |
		|              | 2020-01-01 | 2020-03-06 | InProgress, Complete   | AnyWord      | AnyOrder   | ContainsWord   | True          |                    | 1                   |
		|              | 2020-01-01 | 2099-01-01 | InProgress, Complete   | AnyWord      | AnyOrder   | ContainsWord   | True          |                    | 2                   |
		| new 001      | 2020-01-01 | 2099-01-02 | InProgress             | AnyWord      | AnyOrder   | ContainsWord   | True          |                    | 1                   |
		| new 002      | 2020-02-03 | 2020-08-08 | InProgress, Complete   | AnyWord      | AnyOrder   | ContainsWord   | True          |                    | 2                   |
		| TestXGNew    |            |            | NotStarted             | AnyWord      | AnyOrder   | ContainsWord   | False         |                    | 1                   |
		| new 004      | 2020-08-07 | 2020-08-10 | Complete               | AnyWord      | AnyOrder   | ContainsWord   | True          |                    | 1                   |
		| TEST new 005 | 2020-05-02 | 2020-05-15 | Complete               | AllWords     | AnyOrder   | ExactWord      | False         |                    | 0                   |
		| test new 001 | 2020-01-01 | 2099-01-15 | InProgress             | AllWords     | AnyOrder   | ExactWord      | False         |                    | 1                   |
		| New test 002 | 2020-02-02 | 2020-02-05 | Complete               | AllWords     | AnyOrder   | ExactWord      | False         |                    | 1                   |
		| test New 003 | 2020-03-20 | 2020-04-02 | Errors                 | AllWords     | AnyOrder   | ExactWord      | False         |                    | 1                   |
		| test new 004 | 2020-02-01 | 2020-02-16 | Complete               | AllWords     | AnyOrder   | ExactWord      | True          |                    | 0                   |
		| test New 002 | 2020-02-20 | 2099-03-06 | InProgress             | AllWords     | ExactOrder | ExactWord      | False         |                    | 1                   |
		| test         | 2099-03-05 | 2099-04-11 | NotStarted, InProgress | AllWords     | ExactOrder | StartsWithWord | False         |                    | 1                   |
		| Test         | 2020-04-01 | 2020-04-02 | InProgress, Errors     | AllWords     | ExactOrder | StartsWithWord | True          |                    | 1                   |
		| 003          |            |            | Errors                 | AllWords     | ExactOrder | EndsWithWord   | False         |                    | 1                   |
		| 004          | 2020-08-06 | 2020-08-08 | Complete               | AllWords     | ExactOrder | EndsWithWord   | True          |                    | 1                   |
		| TeSt New     | 2019-03-01 | 2099-08-10 | InProgress, Complete   | AnyWord      | AnyOrder   | ContainsWord   | False         |                    | 5                   |
		#| TEST New     |            |            | NotStarted             | AnyWord      | AnyOrder   | ContainsWord   | False         | X                  | 2                   |
		| new test     |            |            | NotStarted, Complete   | AnyWord      | AnyOrder   | ContainsWord   | False         | XG                 | 4                   |
		| Testew1      |            |            | NotStarted, InProgress | AnyWord      | AnyOrder   | ContainsWord   | False         | XGN                | 0                   |

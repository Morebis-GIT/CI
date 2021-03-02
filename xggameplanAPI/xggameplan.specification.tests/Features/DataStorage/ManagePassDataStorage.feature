@ManageDataStorage

Feature: ManagePassDataStorage
	In order to manage Passes
	As a user
	I want to store Passes via Pass repository

Background:
	Given there is a Passes repository
	And predefined Passes data

Scenario: Add new Pass
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Passes
	When I create the following documents:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing Pass by id
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	When I get document with id 6
	Then no documents should be returned

Scenario: Get an existing Pass by id
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	When I get document with id 2
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value |
         | Id       | 2     |

Scenario: Get all Passes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Update the Pass
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	When I get document with id 3
	And I update received document by values:
		| Property     | Value               |
		| Name         | Pass_Modified       |
	And I get document with id 3
	Then the received document should contain the following values:
		| Property     | Value               |
		| Name         | Pass_Modified       |
		| DateCreated  | 2019-03-14 09:17:51 |

Scenario: Delete an existing Pass by id
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	When I delete document with id 1
	And I get all documents
	Then there should be 2 documents returned

Scenario: Delete a non-existing Pass by id
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	When I try to delete document with id 5
	Then the exception is thrown

Scenario Outline: Remove Passes by ids
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	When I call Remove method with parameters:
         | Parameter | Value |
         | ids       | <Ids> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids      | ExpectedReturnCount |
	| 1        | 2                   |
	| 1, 3     | 1                   |
	| 3, 1, 2  | 0                   |
	| 1, 3, 7  | 1                   |
	| 9, 6, 13 | 3                   |

Scenario Outline: Find Passes by name
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        | IsLibraried |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                | true        |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 | false       |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                | true        |
	When I call FindByName method with parameters:
         | Parameter   | Value         |
         | name        | <Name>        |
         | isLibraried | <IsLibraried> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Name     | IsLibraried | ExpectedReturnCount |
	| Pass One | false       | 1                   |
	| Pass One | true        | 1                   |
	| Pass Two | false       | 1                   |
	| Pass Two | true        | 0                   |

Scenario Outline: Find Passes by ids
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                |
	When I call FindByIds method with parameters:
         | Parameter | Value |
         | ids       | <Ids> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids      | ExpectedReturnCount |
	| 1        | 1                   |
	| 1, 3     | 2                   |
	| 3, 1, 2  | 3                   |
	| 1, 3, 7  | 2                   |
	| 9, 6, 13 | 0                   |

Scenario: Get library ids
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        | IsLibraried |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                | true        |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 | false       |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                | true        |
	When I call GetLibraryIds method
	Then there should be 2 documents returned

Scenario Outline: Remove Passes by Scenario id
	Given predefined data imported
	When I call RemoveByScenarioId method with parameters:
         | Parameter  | Value        |
         | scenarioId | <ScenarioId> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ScenarioId                           | ExpectedReturnCount |
	| 92D76218-FA28-4B0A-A466-06C5E570033F | 1                   |
	| C8A125AD-0467-4F08-A53B-9EAD28BA529B | 2                   |
	| 00000000-0000-0000-0000-000000000000 | 3                   |

Scenario Outline: Find Passes by Scenario id
	Given predefined data imported
	When I call FindByScenarioId method with parameters:
         | Parameter  | Value        |
         | scenarioId | <ScenarioId> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ScenarioId                           | ExpectedReturnCount |
	| 92D76218-FA28-4B0A-A466-06C5E570033F | 2                   |
	| C8A125AD-0467-4F08-A53B-9EAD28BA529B | 1                   |
	| 00000000-0000-0000-0000-000000000000 | 0                   |

Scenario Outline: Search Passes by filter
	Given the following documents created:
		| Id | Name       | DateCreated         | DateModified        | IsLibraried |
		| 1  | Pass One   | 2019-01-20 08:00:00 | null                | true        |
		| 2  | Pass Two   | 2019-02-08 16:22:13 | 2019-02-12 11:43:37 | false       |
		| 3  | Pass Three | 2019-03-14 09:17:51 | null                | true        |
	When I call Search method with parameters:
         | Parameter           | Value                 |
         | name                | <Name>                |
         | isLibraried         | <IsLibraried>         |
         | howManyWordsToMatch | <HowManyWordsToMatch> |
         | wordOrder           | <WordOrder>           |
         | wordComparison      | <WordComparison>      |
         | caseSensitive       | <CaseSensitive>       |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Name       | IsLibraried | HowManyWordsToMatch | WordOrder  | WordComparison | CaseSensitive | ExpectedReturnCount |
	| Pass One   | null        | AnyWord             | AnyOrder   | ContainsWord   | true          | 3                   |
	| Pass Three | true        | AnyWord             | AnyOrder   | ContainsWord   | true          | 2                   |
	| Pass Two   | false       | AnyWord             | AnyOrder   | ContainsWord   | true          | 1                   |
	| Pass One   | null        | AllWords            | AnyOrder   | ContainsWord   | true          | 1                   |
	| Two Pass   | null        | AllWords            | AnyOrder   | ContainsWord   | true          | 1                   |
	| Two Pass   | null        | AllWords            | ExactOrder | ContainsWord   | true          | 0                   |
	| Pass Three | null        | AllWords            | ExactOrder | ContainsWord   | true          | 1                   |
	| Pass Three | null        | AllWords            | ExactOrder | ExactWord      | true          | 1                   |
	| Pass       | null        | AllWords            | ExactOrder | ExactWord      | true          | 3                   |
	| pass One   | null        | AnyWord             | AnyOrder   | ContainsWord   | false         | 3                   |
	| Pass three | true        | AnyWord             | AnyOrder   | ContainsWord   | false         | 2                   |
	| pass two   | false       | AnyWord             | AnyOrder   | ContainsWord   | false         | 1                   |

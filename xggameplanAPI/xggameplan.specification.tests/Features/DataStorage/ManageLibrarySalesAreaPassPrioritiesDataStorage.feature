@ManageDataStorage

Feature: ManageLibrarySalesAreaPassPrioritiesStorage
	In order to manage LibrarySalesAreaPassPriorities
	As a user
	I want to store pass priorities via LibrarySalesAreaPassPriorities repository

Background:
	Given there is a LibrarySalesAreaPassPriorities repository

Scenario: Add new SalesAreaPassPriorities
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get all SalesAreaPassPriorities
	Given 5 documents created
	When I get all documents
	Then there should be 5 documents returned

Scenario: Get a non-existing SalesAreaPassPriority by id
	Given the following documents created:
		| Id | Uid                                  | Name  | StartTime | EndTime  | DaysOfWeek |
		| 1  | e9d74f37-8697-4d42-9ceb-4821806c4a57 | Test1 | 04:00:00  | 05:00:00 | 1111111    |
		| 2  | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 | Test2 | 06:00:00  | 07:00:00 | 1111111    |
		| 3  | 53bcdd86-2a5c-4c8c-8386-5b26ff0c6d30 | Test3 | 08:00:00  | 09:00:00 | 1111111    |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

Scenario: Get an existing SalesAreaPassPriority by id
	Given the following documents created:
		| Id | Uid                                  | Name  | StartTime | EndTime  | DaysOfWeek |
		| 1  | e9d74f37-8697-4d42-9ceb-4821806c4a57 | Test1 | 04:00:00  | 05:00:00 | 1111111    |
		| 2  | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 | Test2 | 06:00:00  | 07:00:00 | 1111111    |
		| 3  | 53bcdd86-2a5c-4c8c-8386-5b26ff0c6d30 | Test3 | 08:00:00  | 09:00:00 | 1111111    |
	When I get document with id '127e04d5-e08b-47f7-8557-98a9e35ae2b6'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property   | Value                                |
		| Id         | 2                                    |
		| Uid        | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 |
		| Name       | Test2                                |
		| StartTime  | 06:00:00                             |
		| EndTime    | 07:00:00                             |
		| DaysOfWeek | 1111111                              |

Scenario: Update the SalesAreaPassPriority
	Given the following documents created:
		| Id | Uid                                  | Name  | StartTime | EndTime  | DaysOfWeek |
		| 1  | e9d74f37-8697-4d42-9ceb-4821806c4a57 | Test1 | 04:00:00  | 05:00:00 | 1111111    |
		| 2  | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 | Test2 | 06:00:00  | 07:00:00 | 1111111    |
		| 3  | 53bcdd86-2a5c-4c8c-8386-5b26ff0c6d30 | Test3 | 08:00:00  | 09:00:00 | 1111111    |
	When I get document with id '127e04d5-e08b-47f7-8557-98a9e35ae2b6'
	And I update received document by values:
		| Property   | Value       |
		| Name       | TestChanged |
		| StartTime  | 09:00:00    |
		| EndTime    | 10:00:00    |
		| DaysOfWeek | 0111111     |
	And I get document with id '127e04d5-e08b-47f7-8557-98a9e35ae2b6'
	Then the received document should contain the following values:
		| Property   | Value       |
		| Name       | TestChanged |
		| StartTime  | 09:00:00    |
		| EndTime    | 10:00:00    |
		| DaysOfWeek | 0111111     |

Scenario: Remove a non-existing SalesAreaPassPriority
	Given the following documents created:
		| Id | Uid                                  | Name  | StartTime | EndTime  | DaysOfWeek |
		| 1  | e9d74f37-8697-4d42-9ceb-4821806c4a57 | Test1 | 04:00:00  | 05:00:00 | 1111111    |
		| 2  | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 | Test2 | 06:00:00  | 07:00:00 | 1111111    |
		| 3  | 53bcdd86-2a5c-4c8c-8386-5b26ff0c6d30 | Test3 | 08:00:00  | 09:00:00 | 1111111    |
	When I delete document with id 'c40bd2c3-9fe5-4962-904b-42b9541834b1'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Removing an existing SalesAreaPassPriority
	Given the following documents created:
		| Id | Uid                                  | Name  | StartTime | EndTime  | DaysOfWeek |
		| 1  | e9d74f37-8697-4d42-9ceb-4821806c4a57 | Test1 | 04:00:00  | 05:00:00 | 1111111    |
		| 2  | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 | Test2 | 06:00:00  | 07:00:00 | 1111111    |
		| 3  | 53bcdd86-2a5c-4c8c-8386-5b26ff0c6d30 | Test3 | 08:00:00  | 09:00:00 | 1111111    |
	When I delete document with id '999654FE-42B3-4C3D-8654-74D0B28F074F'
	And I get document with id '999654FE-42B3-4C3D-8654-74D0B28F074F'
	Then no documents should be returned

Scenario Outline: Check if SalesAreaPassPriority name unique for create
	Given the following documents created:
		| Id | Uid                                  | Name  | StartTime | EndTime  | DaysOfWeek |
		| 1  | e9d74f37-8697-4d42-9ceb-4821806c4a57 | Test1 | 04:00:00  | 05:00:00 | 1111111    |
		| 2  | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 | Test2 | 06:00:00  | 07:00:00 | 1111111    |
		| 3  | 53bcdd86-2a5c-4c8c-8386-5b26ff0c6d30 | Test3 | 08:00:00  | 09:00:00 | 1111111    |
	When I call IsNameUniqueForCreateAsync method with parameters:
		| Parameter | Value  |
		| name      | <Name> |
	Then result should be <Result>

	Examples:
		| Name    | Result |
		| Test1   | False  |
		| Test11  | True   |
		| qwswsdr | True   |

Scenario Outline: Check if SalesAreaPassPriority name unique for update
	Given the following documents created:
		| Id | Uid                                  | Name  | StartTime | EndTime  | DaysOfWeek |
		| 1  | e9d74f37-8697-4d42-9ceb-4821806c4a57 | Test1 | 04:00:00  | 05:00:00 | 1111111    |
		| 2  | 127e04d5-e08b-47f7-8557-98a9e35ae2b6 | Test2 | 06:00:00  | 07:00:00 | 1111111    |
		| 3  | 53bcdd86-2a5c-4c8c-8386-5b26ff0c6d30 | Test3 | 08:00:00  | 09:00:00 | 1111111    |
	When I call IsNameUniqueForUpdateAsync method with parameters:
		| Parameter | Value  |
		| name      | <Name> |
		| uId       | <Id>   |
	Then result should be <Result>

	Examples:
		| Name    | Id                                   | Result |
		| Test1   | e9d74f37-8697-4d42-9ceb-4821806c4a57 | True   |
		| Test2   | e9d74f37-8697-4d42-9ceb-4821806c4a57 | False  |
		| qwswsdr | e9d74f37-8697-4d42-9ceb-4821806c4a57 | True   |

@ManageDataStorage

Feature: Manage Break data storage
	In order to advertise products
	As an Airtime manager
	I want to store breaks in a data store

Background:
	Given there is a Breaks repository
	And predefined Breaks.SalesAreas.json data
	And predefined data imported

Scenario: Add new Break
	When I create the following documents:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Breaks
	When I create the following documents:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Remove an existing Break
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I delete document with id '9F6A7794-30B1-4325-879F-B774B61EA8FA'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing a non-existing Break
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I delete document with id '999654FE-42B3-4C3D-8654-74D0B28F074F'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing Break by id
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I get document with id '999654FE-42B3-4C3D-8654-74D0B28F074F'
	Then no documents should be returned

Scenario: Get an existing Break by id
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I get document with id '5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value                                |
         | Id       | 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 |

Scenario: Get all Breaks
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all Breaks
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating Break documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Find a Break by external reference
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I call FindByExternal method with parameters:
		| Parameter   | Value  |
		| externalRef | <ExternalRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalRef | ExpectedReturnCount |
	| 2484604-1   | 1                   |
	| 2484604-2   | 1                   |
	| 2393746-3   | 1                   |

Scenario Outline: Find Breaks by external references
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I call FindByExternal method with parameters:
		| Parameter   | Value  |
		| externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalRefs                    | ExpectedReturnCount |
	| 2484604-1, 2484604-2            | 2                   |
	| 2484604-1, 2484604-2, 2393746-3 | 3                   |
	| 2393746-3, 2496466-5            | 1                   |
	| 2496466-5, 2505655-7            | 0                   |

Scenario Outline: Search Breaks by parameters
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I call Search method with parameters:
		| Parameter | Value       |
		| datefrom  | <DateFrom>  |
		| dateto    | <DateTo>    |
		| salesarea | <SalesArea> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| DateFrom   | DateTo     | SalesArea | ExpectedReturnCount |
	| 2019-01-11 | 2019-03-07 | NWS91     | 2                   |
	| 2019-01-11 | 2019-03-02 | NWS91     | 1                   |
	| 2019-03-02 | 2019-04-25 | NWS91     | 1                   |
	| 2019-03-02 | 2019-04-25 | QTQ93     | 1                   |
	| 2019-01-11 | 2019-04-25 | TCN92     | 0                   |
	| 2019-01-11 | 2019-03-07 | QTQ93     | 0                   |

@ignore
Scenario Outline: Search Breaks by date range and sales areas
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I call SearchBySalesAreas method with parameters:
		| Parameter             | Value             |
		| scheduledDatesRange   | <DateRange>       |
		| salesAreaNames        | <SalesAreaNames>  |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| DateRange                 | SalesAreaNames   | ExpectedReturnCount |
	| 2019-01-11 to 2019-02-07  | NWS91            | 1                   |
	| 2019-01-11 to 2019-03-07  | NWS91            | 2                   |
	| 2019-01-11 to 2019-04-22  | NWS91, QTQ93     | 3                   |
	| 2019-03-04 to 2019-04-20  | NWS91, QTQ93     | 1                   |
	| 2019-03-04 to 2019-04-20  | TCN92            | 0                   |
	| 2019-05-11 to 2019-06-07  | QTQ93            | 0                   |

Scenario Outline: Remove Breaks by ids
	Given the following documents created:
		| Id                                   | ScheduledDate       | SalesArea | BreakType | ExternalBreakRef |
		| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9 | 2019-01-16 06:11:24 | NWS91     | PREMIUM   | 2484604-1        |
		| 9F6A7794-30B1-4325-879F-B774B61EA8FA | 2019-03-04 06:26:18 | NWS91     | PREMIUM   | 2484604-2        |
		| 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 2019-04-21 06:40:42 | QTQ93     | BASE      | 2393746-3        |
	When I call Delete method with parameters:
		| Parameter | Value |
		| ids       | <Ids> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids                                                                        | ExpectedReturnCount  |
	| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9                                       | 2                    |
	| 5A9C3130-A885-4F7B-AA27-E4DD6EDC0AA9, 9F6A7794-30B1-4325-879F-B774B61EA8FA | 1                    |
	| 9F6A7794-30B1-4325-879F-B774B61EA8FA, 5D9ACDF3-CDCC-4F23-AB1D-63D7227B8683 | 1                    |
	| 4177FB81-9327-4CEF-BA9E-14FF18D6C09C                                       | 3                    |

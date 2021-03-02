@ManageDataStorage

Feature: Manage Schedule data storage
	In order to advertise products
	As an Airtime manager
	I want to store Schedules in a data store

Background:
	Given there is a Schedules repository
	And predefined Breaks.SalesAreas.json data
	And predefined Schedules data

Scenario: Add new Schedule
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Update Schedule
	Given predefined Breaks.SalesAreas.json data imported
	And the following documents created:
		| Date       | SalesArea |
		| 2019-01-16 | QTQ91     |
		| 2019-02-25 | GTV93     |
		| 2019-03-03 | STW92     |
	When I get document with id 2
	Then there should be 1 documents returned
	When I update received document by values:
		| Property      | Value      |
		| Date          | 2019-01-16 |
		| SalesArea     | QTQ91      |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property      | Value      |
		| Date          | 2019-01-16 |
		| SalesArea     | QTQ91      |

Scenario Outline: Remove a Schedule
	Given the following documents created:
		| Id | Date       | SalesArea |
		| 1  | 2019-01-16 | QTQ91     |
		| 2  | 2019-02-25 | GTV93     |
		| 3  | 2019-03-03 | STW92     |
	When I delete document with id <Id>
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Id | ExpectedReturnCount |
	| 1  | 2                   |
	| 5  | 3                   |

Scenario: Get an existing Schedule by id
	Given the following documents created:
		| Id | Date       | SalesArea |
		| 1  | 2019-01-16 | QTQ91     |
		| 2  | 2019-02-25 | GTV93     |
		| 3  | 2019-03-03 | STW92     |
	When I get document with id 2
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value |
         | Id       | 2     |

Scenario: Get all Schedules
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all Schedules
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating Schedule documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Get Schedules by break ids
	Given predefined data imported
	When I call FindByBreakIds method with parameters:
		| Parameter     | Value      |
		| breakIds      | <BreakIds> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| BreakIds                                                                      | ExpectedReturnCount |
	| AE2B3C5F-728F-41F5-B3DA-36E2D73EA0BF                                          | 1                   |
	| AE2B3C5F-728F-41F5-B3DA-36E2D73EA0BF, 812CE306-0045-4763-852D-23F00664E28F    | 2                   |
	| 2EF01FEA-72B5-4111-B423-E0A6C498CAEB                                          | 0                   |

Scenario Outline: Get Schedule by sales area and date
	Given predefined Breaks.SalesAreas.json data imported
	And the following documents created:
		| Date       | SalesArea |
		| 2019-01-16 | QTQ91     |
		| 2019-02-25 | GTV93     |
		| 2019-03-03 | STW92     |
		| 2019-03-20 | QTQ91     |
	When I call GetSchedule method with parameters:
		| Parameter     | Value           |
		| salesareaname | <SalesAreaName> |
		| scheduledate  | <Date>          |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaName | Date       | ExpectedReturnCount |
	| QTQ91         | 2019-01-16 | 1                   |
	| QTQ91         | 2019-03-20 | 1                   |
	| QTQ91         | 2019-02-25 | 0                   |
	| GTV93         | 2019-02-25 | 1                   |
	| GTV93         | 2019-02-26 | 0                   |
	| STW92         | 2019-03-03 | 1                   |
	| NO_AREA       | 2019-03-03 | 0                   |

Scenario Outline: Get Schedules by sales areas and period of date
	Given predefined Breaks.SalesAreas.json data imported
	And the following documents created:
		| Date       | SalesArea |
		| 2019-01-16 | QTQ91     |
		| 2019-02-25 | GTV93     |
		| 2019-03-03 | STW92     |
		| 2019-03-20 | QTQ91     |
		| 2019-04-15 | STW92     |
	When I call GetSchedule method with parameters:
		| Parameter      | Value            |
		| salesareanames | <SalesAreaNames> |
		| fromdate       | <FromDate>       |
		| todate         | <ToDate>         |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaNames      | FromDate   | ToDate     | ExpectedReturnCount |
	| QTQ91               | 2019-01-05 | 2019-02-17 | 1                   |
	| QTQ91               | 2019-01-05 | 2019-03-25 | 2                   |
	| QTQ91, NO_AREA      | 2019-01-05 | 2019-02-17 | 1                   |
	| GTV93, STW92        | 2019-02-21 | 2019-04-01 | 2                   |
	| GTV93, STW92        | 2019-02-21 | 2019-04-15 | 3                   |
	| STW92, QTQ91, GTV93 | 2019-01-01 | 2019-05-01 | 5                   |
	| STW92, QTQ91, GTV93 | 2019-06-01 | 2019-07-01 | 0                   |
	| NO_AREA             | 2019-01-01 | 2019-05-01 | 0                   |

Scenario Outline: Get Breaks
	Given predefined data imported
	When I call GetBreaks method with parameters:
		| Parameter        | Value              |
		| salesareanames   | <SalesAreaNames>   |
		| fromdate         | <FromDate>         |
		| todate           | <ToDate>           |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaNames      | FromDate   | ToDate     | ExpectedReturnCount |
	| QTQ91               | 2019-03-07 | 2019-03-12 | 7                   |
	| STW92               | 2019-03-07 | 2019-03-12 | 0                   |
	| GTV93               | 2019-03-07 | 2019-03-12 | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-12 | 7                   |
	| QTQ91               | 2019-03-07 | 2019-03-18 | 7                   |
	| STW92               | 2019-03-07 | 2019-03-18 | 7                   |
	| GTV93               | 2019-03-07 | 2019-03-18 | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-18 | 14                  |

Scenario Outline: Get Programmes
	Given predefined data imported
	When I call GetProgrammes method with parameters:
		| Parameter        | Value              |
		| salesareanames   | <SalesAreaNames>   |
		| fromdate         | <FromDate>         |
		| todate           | <ToDate>           |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaNames      | FromDate   | ToDate     | ExpectedReturnCount |
	| QTQ91               | 2019-03-07 | 2019-03-12 | 10                  |
	| STW92               | 2019-03-07 | 2019-03-12 | 0                   |
	| GTV93               | 2019-03-07 | 2019-03-12 | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-12 | 10                  |
	| QTQ91               | 2019-03-07 | 2019-03-18 | 10                  |
	| STW92               | 2019-03-07 | 2019-03-18 | 10                  |
	| GTV93               | 2019-03-07 | 2019-03-18 | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-18 | 20                  |

Scenario Outline: Get Break models
	Given predefined data imported
	When I call GetBreakModels method with parameters:
		| Parameter        | Value              |
		| salesAreaNames   | <SalesAreaNames>   |
		| fromDate         | <FromDate>         |
		| toDate           | <ToDate>           |
		| excludeBreakType | <ExcludeBreakType> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaNames      | FromDate   | ToDate     | ExcludeBreakType | ExpectedReturnCount |
	| QTQ91               | 2019-03-07 | 2019-03-12 | null             | 7                   |
	| STW92               | 2019-03-07 | 2019-03-12 | null             | 0                   |
	| GTV93               | 2019-03-07 | 2019-03-12 | null             | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-12 | null             | 7                   |
	| QTQ91               | 2019-03-07 | 2019-03-12 | PREMIUM          | 2                   |
	| QTQ91               | 2019-03-07 | 2019-03-12 | BASE             | 5                   |
	| QTQ91               | 2019-03-07 | 2019-03-18 | null             | 7                   |
	| STW92               | 2019-03-07 | 2019-03-18 | null             | 7                   |
	| GTV93               | 2019-03-07 | 2019-03-18 | null             | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-18 | null             | 14                  |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-18 | PREMIUM          | 4                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-18 | BASE             | 10                  |

Scenario Outline: Count Breaks and Programmes
	Given predefined data imported
	When I call CountBreaksAndProgrammes method with parameters:
		| Parameter | Value      |
		| dateFrom  | <DateFrom> |
		| dateTo    | <DateTo>   |
	Then the received result should contain the following values:
        | Property       | Value            |
        | BreakCount     | <BreakCount>     |
        | ProgrammeCount | <ProgrammeCount> |

	Examples:
	| DateFrom   | DateTo     | BreakCount | ProgrammeCount |
	| 2019-03-07 | 2019-03-12 | 7          | 10             |
	| 2019-03-07 | 2019-03-18 | 14         | 20             |

Scenario Outline: Get Breaks with Programme
	Given predefined data imported
	When I call GetBreakWithProgramme method with parameters:
		| Parameter      | Value            |
		| salesAreaNames | <SalesAreaNames> |
		| fromDate       | <FromDate>       |
		| toDate         | <ToDate>         |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaNames      | FromDate   | ToDate     | ExpectedReturnCount |
	| QTQ91               | 2019-03-07 | 2019-03-12 | 13                  |
	| STW92               | 2019-03-07 | 2019-03-12 | 0                   |
	| GTV93               | 2019-03-07 | 2019-03-12 | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-12 | 13                  |
	| QTQ91               | 2019-03-07 | 2019-03-18 | 13                  |
	| STW92               | 2019-03-07 | 2019-03-18 | 12                  |
	| GTV93               | 2019-03-07 | 2019-03-18 | 0                   |
	| QTQ91, STW92, GTV93 | 2019-03-07 | 2019-03-18 | 25                  |

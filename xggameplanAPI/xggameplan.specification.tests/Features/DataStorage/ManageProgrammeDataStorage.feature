@ManageDataStorage

Feature: Manage Programme data storage
	In order to advertise products
	As an Airtime manager
	I want to store Programmes in a data store

Background:
	Given there is a Programmes repository
	And predefined Breaks.SalesAreas.json data

Scenario: Add new Programme 
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Programmes
	When I create the following documents:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	And I get all documents
	Then there should be 3 documents returned

Scenario Outline: Remove a Programme
	Given the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I delete document with id '<Id>'
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Id                                  | ExpectedReturnCount |
	| 90032B86-2FA9-4950-9004-F0541B6EFDDD | 2                   |
	| 00000000-0000-0000-0000-000000000000 | 3                   |

Scenario: Get a non-existing Programme by id
	Given the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

Scenario: Get an existing Programme by id
	Given the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I get document with id '6305668B-20F7-4878-A009-640C57EA5A37'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value                                |
         | Id       | 6305668B-20F7-4878-A009-640C57EA5A37 |

Scenario: Get all Programmes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all Programmes
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating Programme documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Find a Programme by external reference
	Given the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I call FindByExternal method with parameters:
		| Parameter   | Value  |
		| externalRef | <ExternalRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalRef | ExpectedReturnCount |
	| TOSH        | 1                   |
	| PAWPA       | 1                   |
	| AUOPTD      | 1                   |
	| EXT_REF     | 0                   |

Scenario Outline: Find Programmes by external references
	Given the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I call FindByExternal method with parameters:
		| Parameter   | Value  |
		| externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalRefs        | ExpectedReturnCount |
	| TOSH, PAWPA         | 2                   |
	| TOSH, AUOPTD, PAWPA | 3                   |
	| AUOPTD, EXT_REF     | 1                   |
	| EXT_REF, EXT_REF_2  | 0                   |

Scenario Outline: Search Programmes by period and sales area
	Given predefined Breaks.SalesAreas.json data imported
	And the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I call Search method with parameters:
		| Parameter | Value       |
		| datefrom  | <DateFrom>  |
		| dateto    | <DateTo>    |
		| salesarea | <SalesArea> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| DateFrom   | DateTo     | SalesArea | ExpectedReturnCount |
	| 2019-01-05 | 2019-01-30 | QTQ91     | 1                   |
	| 2019-02-01 | 2019-02-27 | GTV93     | 1                   |
	| 2019-02-01 | 2019-03-10 | STW92     | 1                   |
	| 2019-03-06 | 2019-03-10 | GTV93     | 0                   |

Scenario Outline: Search Programmes by filter
	Given predefined Breaks.SalesAreas.json data imported
	And the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I call Search method with parameters:
		| Parameter         | Value               |
		| fromDateInclusive | <FromDateInclusive> |
		| toDateInclusive   | <ToDateInclusive>   |
		| salesAreas        | <SalesAreas>        |
		| nameOrRef         | <NameOrRef>         |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| FromDateInclusive | ToDateInclusive | SalesAreas   | NameOrRef | ExpectedReturnCount |
	| 2019-01-05        | null            | null         | null      | 3                   |
	| 2019-02-05        | null            | null         | null      | 2                   |
	| 2019-03-10        | null            | null         | null      | 0                   |
	| null              | 2019-03-10      | null         | null      | 3                   |
	| null              | 2019-02-26      | null         | null      | 2                   |
	| null              | 2019-01-15      | null         | null      | 0                   |
	| 2019-01-10        | 2019-03-10      | null         | null      | 3                   |
	| 2019-02-10        | 2019-03-10      | null         | null      | 2                   |
	| 2019-03-05        | 2019-03-10      | null         | null      | 0                   |
	| null              | null            | QTQ91        | null      | 1                   |
	| null              | null            | QTQ91, STW92 | null      | 2                   |
	| null              | null            | GTV93, S_A   | null      | 1                   |
	| 2019-01-10        | null            | QTQ91        | null      | 1                   |
	| 2019-01-10        | null            | QTQ91, STW92 | null      | 2                   |
	| 2019-02-10        | null            | QTQ91, STW92 | null      | 1                   |
	| 2019-02-27        | null            | QTQ91, GTV93 | null      | 0                   |
	| null              | 2019-02-27      | QTQ91        | null      | 1                   |
	| null              | 2019-02-27      | QTQ91, GTV93 | null      | 2                   |
	| null              | 2019-02-27      | QTQ91, STW92 | null      | 1                   |
	| null              | 2019-02-10      | STW92, GTV93 | null      | 0                   |
	| 2019-01-10        | 2019-02-10      | QTQ91        | null      | 1                   |
	| 2019-01-10        | 2019-02-10      | QTQ91, GTV93 | null      | 1                   |
	| 2019-01-10        | 2019-03-10      | QTQ91, GTV93 | null      | 2                   |
	| 2019-03-10        | 2019-03-20      | STW92, QTQ91 | null      | 0                   |
	| null              | null            | null         | null      | 3                   |

Scenario Outline: Search Programmes by filter Name Or Reference
	Given the following documents created:
		| Id                                   | SalesArea | StartDateTime       | Duration | ExternalReference | ProgrammeName |
		| 3587525F-4441-46FA-A5A5-3CB316677155 | QTQ91     | 2019-01-16 06:00:00 | 03:00:00 | TOSH              | Programme 1   |
		| 90032B86-2FA9-4950-9004-F0541B6EFDDD | GTV93     | 2019-02-25 13:00:00 | 00:30:00 | PAWPA             | Programme 2   |
		| 6305668B-20F7-4878-A009-640C57EA5A37 | STW92     | 2019-03-03 09:15:00 | 02:26:00 | AUOPTD            | Programme 3   |
	When I call Search method with parameters:
		| Parameter         | Value               |
		| fromDateInclusive | <FromDateInclusive> |
		| toDateInclusive   | <ToDateInclusive>   |
		| salesAreas        | <SalesAreas>        |
		| nameOrRef         | <NameOrRef>         |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| FromDateInclusive | ToDateInclusive | SalesAreas   | NameOrRef | ExpectedReturnCount |
	| null              | null            | null         | programme | 3                   |
	| null              | null            | null         | AUOPTD    | 1                   |
	| null              | null            | null         | NONAME    | 0                   |

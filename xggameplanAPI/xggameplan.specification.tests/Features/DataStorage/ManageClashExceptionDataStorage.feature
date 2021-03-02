@ManageDataStorage

Feature: Manage ClashException data storage
	In order to manage clash exceptions
	As an Airtime manager
	I want to store clash exceptions in a data store

Background: 
	Given there is a ClashException repository

Scenario: Add new ClashException
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new ClashExceptions
	When I create the following documents:
		| Id | StartDate           | EndDate             | FromType   | IncludeOrExclude | FromValue | ToValue |
		| 1  | 2019-02-12 07:12:00 | 2019-03-12 07:12:00 | Product    | I                | P136      | P195    |
		| 2  | 2018-01-02 13:45:23 | 2018-04-01 04:04:03 | Product    | E                | P112      | P345    |
		| 3  | 2019-01-01 14:00:00 | 2019-03-03 05:06:07 | Advertiser | I                | Adv1      | Adv2    |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Remove an existing ClashException
	Given the following documents created:
		| Id | StartDate           | EndDate             | FromType   | IncludeOrExclude | FromValue | ToValue |
		| 1  | 2019-02-12 07:12:00 | 2019-03-12 07:12:00 | Product    | I                | P136      | P195    |
		| 2  | 2018-01-02 13:45:23 | 2018-04-01 04:04:03 | Product    | E                | P112      | P345    |
		| 3  | 2019-01-01 14:00:00 | 2019-03-03 05:06:07 | Advertiser | I                | Adv1      | Adv2    |
	When I delete document with id '2'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing a non-existing ClashException
	Given the following documents created:
		| Id | StartDate           | EndDate             | FromType   | IncludeOrExclude | FromValue | ToValue |
		| 1  | 2019-02-12 07:12:00 | 2019-03-12 07:12:00 | Product    | I                | P136      | P195    |
		| 2  | 2018-01-02 13:45:23 | 2018-04-01 04:04:03 | Product    | E                | P112      | P345    |
		| 3  | 2019-01-01 14:00:00 | 2019-03-03 05:06:07 | Advertiser | I                | Adv1      | Adv2    |
	When I delete document with id '123'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing ClashException by id
	Given the following documents created:
		| Id | StartDate           | EndDate             | FromType   | IncludeOrExclude | FromValue | ToValue |
		| 1  | 2019-02-12 07:12:00 | 2019-03-12 07:12:00 | Product    | I                | P136      | P195    |
		| 2  | 2018-01-02 13:45:23 | 2018-04-01 04:04:03 | Product    | E                | P112      | P345    |
		| 3  | 2019-01-01 14:00:00 | 2019-03-03 05:06:07 | Advertiser | I                | Adv1      | Adv2    |
	When I get document with id '111'
	Then no documents should be returned

Scenario: Get an existing ClashException by id
	Given the following documents created:
		| Id | StartDate           | EndDate             | FromType   | IncludeOrExclude | FromValue | ToValue |
		| 1  | 2019-02-12 07:12:00 | 2019-03-12 07:12:00 | Product    | I                | P136      | P195    |
		| 2  | 2018-01-02 13:45:23 | 2018-04-01 04:04:03 | Product    | E                | P112      | P345    |
		| 3  | 2019-01-01 14:00:00 | 2019-03-03 05:06:07 | Advertiser | I                | Adv1      | Adv2    |
	When I get document with id '3'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property         | Value               |
         | Id               | 3                   |
         | StartDate        | 2019-01-01 14:00:00 |
         | EndDate          | 2019-03-03 05:06:07 |
         | FromType         | Advertiser          |
         | IncludeOrExclude | I                   |
         | FromValue        | Adv1                |
         | ToValue          | Adv2                |

Scenario: Get all ClashExceptions
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all ClashExceptions
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating ClashException documents
	Given 7 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Search ClashException
	Given the following documents created:
	| Id | StartDate           | EndDate             | FromType   | IncludeOrExclude | FromValue | ToValue |
	| 1  | 2019-03-26 07:12:00 | 2019-03-30 07:12:00 | Product    | I                | P136      | P195    |
	| 2  | 2019-06-26 13:45:23 | 2019-06-30 04:04:03 | Product    | E                | P112      | P345    |
	| 3  | 2019-01-26 14:00:00 | 2019-01-30 05:06:07 | Advertiser | I                | Adv4      | Adv2    |
	| 4  | 2019-02-12 14:00:00 | 2019-02-24 05:06:07 | Advertiser | I                | Adv6      | Adv3    |
	| 5  | 2019-03-01 14:00:00 | 2019-03-15 05:06:07 | Advertiser | I                | Adv7      | Adv4    |

	When I call Search method with parameters:
		| Parameter | Value       |
		| startDate | <StartDate> |
		| endDate   | <EndDate>   |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| StartDate           | EndDate             | ExpectedReturnCount |
	| null                | null                | 5                   |
	| 2018-01-01 00:00:00 | null                | 5                   |
	| null                | 2020-01-01 00:00:00 | 5                   |
	| 2020-05-05 23:23:23 | 2021-03-03 12:12:12 | 0                   |
	| 2000-03-03 05:05:00 | 2001-04-04 12:12:12 | 0                   |
	| 2019-03-01 00:00:00 | 2019-03-31 23:59:59 | 2                   |
	| 2019-06-26 00:00:00 | 2019-06-30 00:00:00 | 1                   |

Scenario Outline: Delete ClashExceptions by externalRefs
	Given the following documents created:
	| Id | StartDate           | EndDate             | FromType   | IncludeOrExclude | FromValue | ToValue | ExternalRef |
	| 1  | 2019-03-26 07:12:00 | 2019-03-30 07:12:00 | Product    | I                | P136      | P195    | Ref1        |
	| 2  | 2019-06-26 13:45:23 | 2019-06-30 04:04:03 | Product    | E                | P112      | P345    | Ref2        |
	| 3  | 2019-01-26 14:00:00 | 2019-01-30 05:06:07 | Advertiser | I                | Adv4      | Adv2    | Ref3        |
	| 4  | 2019-02-12 14:00:00 | 2019-02-24 05:06:07 | Advertiser | I                | Adv6      | Adv3    | Ref4        |
	| 5  | 2019-03-01 14:00:00 | 2019-03-15 05:06:07 | Advertiser | I                | Adv7      | Adv4    | Ref5        |
	When I call DeleteRangeByExternalRefs method with parameters:
		| Parameter    | Value          |
		| externalRefs | <ExternalRefs> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned
	
	Examples:
	| ExternalRefs | ExpectedReturnCount |
	| Ref1         | 4                   |
	| Ref3, Ref2   | 3                   |
	| Ref6         | 5                   |           

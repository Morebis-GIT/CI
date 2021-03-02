@ManageDataStorage

Feature: Manage Restrictions data storage
	In order to manage Restrictions
	As a user
	I want to store restrictions via RestictionsRepository

Background:
	Given there is a Restrictions repository
	And predefined Restrictions data

Scenario: Add new Restriction
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Restrictions
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all Restrictions
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get an existing Restriction by id
	Given predefined data imported
	When I get document with id '75dcd652-fd1f-4da4-92c9-4d96501b193a'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value                                |
         | Uid      | 75dcd652-fd1f-4da4-92c9-4d96501b193a |

Scenario: Get a non-existing Restriction by id
	Given predefined data imported
	When I get document with id 'deaaaf16-6c52-4dfe-8590-4367b0ec43bb'
	Then no documents should be returned

Scenario: Remove an existing Restriction
	Given predefined data imported
	When I delete document with id '75dcd652-fd1f-4da4-92c9-4d96501b193a'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Removing a non-existing Restriction
	Given predefined data imported
	When I delete document with id 'deaaaf16-6c52-4dfe-8590-4367b0ec43bb'
	And I get all documents
	Then there should be 4 documents returned

Scenario: Truncating Restriction documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Get a Restriction with description by id
	Given predefined data imported
	When I call GetDesc method with parameters:
		| Parameter | Value |
		| id        | <Id>  |
	Then the received document should contain the following values:
		| Property             | Value                  |
		| ProductDescription   | <ProductDescription>   |
		| ProgrammeDescription | <ProgrammeDescription> |
		| ClashDescription     | <ClashDescription>     |
		| AdvertiserName       | <AdvertiserName>       |

	Examples:
		| Id                                   | ProductDescription     | ProgrammeDescription | ClashDescription   | AdvertiserName |
		| 75dcd652-fd1f-4da4-92c9-4d96501b193a |                        |                      | Recycling Products |                |
		| 985bbe70-e35c-42e3-ad9d-7fdd81500033 | ProductForRestriction2 |                      |                    | Advertiser 2   |
		| 2af826f2-a222-4a9a-ad1b-73a137940412 |                        | ProgrammeName2       |                    |                |
		| df0eca6d-6b76-4cb3-8fef-1b7f5763f930 | ProductForRestriction1 | ProgrammeName3       |                    | Advertiser 1   |

Scenario Outline: Search Restrictions
	Given predefined data imported
	When I call SearchRestrictions method with parameters:
		| Parameter                   | Value                         |
		| salesAreaNames              | <SalesAreaNames>              |
		| startDate                   | <StartDate>                   |
		| endDate                     | <EndDate>                     |
		| restrictionType             | <RestrictionType>             |
		| matchAllSpecifiedSalesAreas | <MatchAllSpecifiedSalesAreas> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaNames | StartDate  | EndDate    | RestrictionType | MatchAllSpecifiedSalesAreas | ExpectedReturnCount |
	| null           | null       | null       | null            | false                       | 4                   |
	| null           | null       | null       | null            | true                        | 4                   |
	| GTV91, GTV94   | null       | null       | null            | false                       | 4                   |
	| GTV91, GTV94   | null       | null       | Time            | false                       | 1                   |
	| null           | null       | null       | Programme       | false                       | 1                   |
	| null           | 2019-05-08 | 2019-10-08 | Programme       | false                       | 1                   |
	| null           | 2020-05-08 | 2021-10-26 | null            | false                       | 4                   |
	| TM1            | 2019-05-08 | 2019-10-26 | null            | false                       | 0                   |
	| TM1            | 2019-05-08 | 2019-10-26 | null            | true                        | 0                   |
	| TM1, GTV91     | null       | null       | null            | true                        | 0                   |
	| TM1, GTV91     | null       | null       | null            | false                       | 4                   |

Scenario Outline: Delete Restrictions by criteria
	Given predefined data imported
	When I call DeleteByCriteria method with parameters:
		| Parameter                   | Value                         |
		| salesAreaNames              | <SalesAreaNames>              |
		| startDate                   | <StartDate>                   |
		| endDate                     | <EndDate>                     |
		| restrictionType             | <RestrictionType>             |
		| matchAllSpecifiedSalesAreas | <MatchAllSpecifiedSalesAreas> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| SalesAreaNames | StartDate  | EndDate    | RestrictionType | MatchAllSpecifiedSalesAreas | ExpectedReturnCount |
	| null           | 2019-05-08 | 2019-10-08 | Time            | false                       | 3                   |
	| null           | 2019-05-08 | 2019-10-08 | Programme       | true                        | 4                   |
	| null           | 2020-05-08 | 2020-10-08 | Programme       | true                        | 4                   |

Scenario: Get an existing Restriction by externalIdentifier
	Given predefined data imported
	When I call Get method with parameters:
	| Parameter          | Value       |
	| externalIdentifier | YH000000001 |
	Then there should be 1 documents returned
	And the received document should contain the following values:
	| Parameter          | Value       |
	| externalIdentifier | YH000000001 |

Scenario: Get a non-existing Restriction by externalIdentifier
	Given predefined data imported
	When I call Get method with parameters:
	| Parameter          | Value       |
	| externalIdentifier | YH000000005 |
	Then no documents should be returned

#Scenario: Update existing Restrictions
#	Given predefined data imported
#	When I call UpdateRange method with parameters:
#		| Property                | Value                                |
#		| uid                     | 75dcd652-fd1f-4da4-92c9-4d96501b193a |
#		| startDate               | 2019-09-20T00:00:00.000Z             |
#		| endDate                 | 2020-01-01T00:00:00.000Z             |
#		| restrictionDays         | 11111                                |
#		| startTime               | 00:05:00                             |
#		| endTime                 | 00:10:00                             |
#		| timeToleranceMinsAfter  | 5                                    |
#		| timeToleranceMinsBefore | 5                                    |
#		| schoolHolidayIndicator  | I                                    |
#		| publicHolidayIndicator  | I                                    |
#	And I get document with id '75dcd652-fd1f-4da4-92c9-4d96501b193a'
#	Then there should be 1 documents returned
#	And the received document should contain the following values:
#		| Property                | Value                                |
#		| Uid                     | 75dcd652-fd1f-4da4-92c9-4d96501b193a |
#		| StartDate               | 2019-09-20T00:00:00.000Z             |
#		| EndDate                 | 2020-01-01T00:00:00.000Z             |
#		| RestrictionDays         | 11111                                |
#		| StartTime               | 00:05:00                             |
#		| EndTime                 | 00:10:00                             |
#		| TimeToleranceMinsAfter  | 5                                    |
#		| TimeToleranceMinsBefore | 5                                    |
#		| SchoolHolidayIndicator  | I                                    |
#		| PublicHolidayIndicator  | I                                    |


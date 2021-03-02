@ManageDataStorage

Feature: Manage Product data storage
	In order to advertise products
	As an Airtime manager
	I want to store products in a data store

Background:
	Given there is a Products repository

Scenario: Add new Product
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Update product
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
	When I get document with id '496190D5-DB2F-4648-801B-AFD914C91094'
	Then there should be 1 documents returned
	When I update received document by values:
		| Property              | Value             |
		| Externalidentifier    | 429               |
		| Name                  | Another product   |
	And I get document with id '496190D5-DB2F-4648-801B-AFD914C91094'
	Then the received document should contain the following values:
		| Property              | Value             |
		| Externalidentifier    | 429               |
		| Name                  | Another product   |
		| ClashCode             | P113              |

Scenario: Add new Products
	When I create the following documents:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
	And I get all documents
	Then there should be 3 documents returned

Scenario Outline: Remove a Product
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
	When I delete document with id '<Uid>'
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Uid                                  | ExpectedReturnCount |
	| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 2                   |
	| 00000000-0000-0000-0000-000000000000 | 3                   |

Scenario: Get a non-existing Product by id
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

Scenario: Get an existing Product by id
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
	When I get document with id 'FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value                                |
         | Uid      | FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 |

Scenario: Get all Products
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all Products
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating Product documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario: Find distinct reporting categories
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | ReportingCategory |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | test category 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | test category 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | test category 3   |
		| 09724F12-D042-4626-87D9-A7B07652CA1F | 1605               | Product 4 | test category 3   |
		| 3E2C483C-34D2-42A9-B29F-E0DF8AB0AE4B | 1606               | Product 5 | test category 3   |
	When I call GetReportingCategories method
	Then there should be 3 documents returned

Scenario Outline: Find a Product by external reference
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
	When I call FindByExternal method with parameters:
		| Parameter   | Value        |
		| externalRef | <ProductRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ProductRef | ExpectedReturnCount |
	| 519        | 1                   |
	| 662        | 1                   |
	| 1604       | 1                   |
	| 001        | 0                   |

Scenario Outline: Find Products by external references
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
	When I call FindByExternal method with parameters:
		| Parameter    | Value         |
		| externalRefs | <ProductRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ProductRefs    | ExpectedReturnCount |
	| 519, 662       | 2                   |
	| 519, 662, 1604 | 3                   |
	| 662, 001       | 1                   |
	| 001, 002       | 0                   |


Scenario Outline: Search product advertiser
	Given the current date is '2020-06-26 13:00:00'
	And the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName | AdvertiserLinkStartDate | AdvertiserLinkEndDate |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
		| 21E4B32E-8581-4D0E-B70C-A4AD51D4AB0A | 118032             | Product 4 | 2019-03-15         | 2019-06-19       | P243      | 42511                | Advertiser 2   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
	When I call SearchAdvertiser method with parameters:
		| Parameter | Value       |
		| nameOrRef | <NameOrRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| NameOrRef    | ExpectedReturnCount |
	| 41197        | 1                   |
	| 42511        | 1                   |
	| Advertiser 3 | 1                   |
	| 00001        | 0                   |
	| Advertiser 2 | 1                   |


Scenario Outline: Search product
	Given the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-02         | 2019-04-22       | P113      | 41197                | Advertiser 1   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   |
		| 21E4B32E-8581-4D0E-B70C-A4AD51D4AB0A | 118032             | Product 4 | 2019-03-15         | 2019-06-19       | P243      | 42511                | Advertiser 2   |
		| C0D2E172-2635-4175-A220-2FF70FAF004E | 118385             | Product 2 | 2019-06-03         | 2019-07-26       | P115      | 42511                | Advertiser 2   |
	When I call Search method with parameters:
		| Parameter          | Value                |
		| externalIdentifier | <ExternalIdentifier> |
		| name               | <Name>               |
		| clashCode          | <ClashCode>          |
		| fromDateInclusive  | <FromDateInclusive>  |
		| toDateInclusive    | <ToDateInclusive>    |
		| nameOrRef          | <NameOrRef>          |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalIdentifier | Name      | ClashCode | FromDateInclusive | ToDateInclusive | NameOrRef | ExpectedReturnCount |
	| null               | null      | null      | null              | null            | null      | 5                   |
	| 1604               | null      | null      | null              | null            | null      | 1                   |
	| 1604               | Product 3 | null      | null              | null            | null      | 1                   |
	| 1604               | Product 3 | P280      | null              | null            | null      | 1                   |
	| 1604               | Product 3 | P280      | 2019-01-15        | null            | null      | 1                   |
	| 1604               | Product 3 | P280      | 2019-01-21        | null            | null      | 0                   |
	| 1604               | Product 3 | P280      | null              | 2019-06-15      | null      | 1                   |
	| 1604               | Product 3 | P280      | null              | 2019-06-10      | null      | 0                   |
	| 1604               | Product 3 | P280      | 2019-01-15        | 2019-06-15      | null      | 1                   |
	| 1604               | Product 3 | P280      | 2019-01-25        | 2019-06-15      | null      | 0                   |
	| null               | null      | null      | null              | null            | 1604      | 1                   |
	| null               | null      | null      | null              | null            | Product   | 5                   |
	| null               | null      | null      | null              | null            | 0001      | 0                   |
	| null               | Product 2 | null      | null              | null            | null      | 2                   |
	| null               | Product 2 | null      | 2019-02-01        | null            | null      | 2                   |
	| null               | Product 2 | null      | 2019-03-10        | null            | null      | 1                   |
	| null               | Product 2 | null      | null              | 2019-07-30      | null      | 2                   |
	| null               | Product 2 | null      | null              | 2019-06-10      | null      | 1                   |
	| null               | Product 2 | null      | 2019-02-01        | 2019-05-20      | null      | 1                   |
	| null               | Product 2 | null      | 2019-02-01        | 2019-07-30      | null      | 2                   |
	| null               | null      | P115      | null              | null            | null      | 2                   |
	| null               | null      | P115      | 2019-02-01        | null            | null      | 2                   |
	| null               | null      | P115      | 2019-04-10        | null            | null      | 1                   |
	| null               | null      | P115      | 2019-06-10        | null            | null      | 0                   |
	| null               | null      | P115      | null              | 2019-07-30      | null      | 2                   |
	| null               | null      | P115      | null              | 2019-06-10      | null      | 1                   |
	| null               | null      | P115      | null              | 2019-05-05      | null      | 0                   |
	| null               | null      | P115      | 2019-02-01        | 2019-05-20      | null      | 1                   |
	| null               | null      | P115      | 2019-02-01        | 2019-07-30      | null      | 2                   |
	| null               | null      | null      | 2019-01-01        | 2019-05-15      | null      | 2                   |
	| null               | null      | null      | 2019-01-15        | 2019-06-20      | null      | 3                   |
	| null               | null      | null      | 2019-01-01        | 2019-08-01      | null      | 5                   |
	| null               | null      | null      | 2019-02-01        | null            | null      | 3                   |
	| null               | null      | null      | 2019-01-10        | null            | null      | 4                   |
	| null               | null      | null      | 2019-02-05        | null            | null      | 2                   |
	| null               | null      | null      | 2019-08-01        | null            | null      | 0                   |
	| null               | null      | null      | null              | 2019-05-15      | null      | 2                   |
	| null               | null      | null      | null              | 2019-06-25      | null      | 4                   |
	| null               | null      | null      | null              | 2019-04-05      | null      | 0                   |

Scenario Outline: Find Products by advertiser id
	Given the current date is '2020-06-26 13:00:00'
	And the following documents created:
		| Uid                                  | Externalidentifier | Name      | EffectiveStartDate | EffectiveEndDate | ClashCode | AdvertiserIdentifier | AdvertiserName | AdvertiserLinkStartDate | AdvertiserLinkEndDate |
		| 496190D5-DB2F-4648-801B-AFD914C91094 | 519                | Product 1 | 2019-01-01         | 2019-04-22       | P113      | 41197                | Advertiser 1   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
		| 46E7BF9F-011F-4068-BC9C-75DFE823F688 | 662                | Product 2 | 2019-02-04         | 2019-05-10       | P115      | 42511                | Advertiser 2   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
		| FFD9A2F8-DA5B-4C0B-A367-1A1F0DB82487 | 1604               | Product 3 | 2019-01-20         | 2019-06-13       | P280      | 70012                | Advertiser 3   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
		| 21E4B32E-8581-4D0E-B70C-A4AD51D4AB0A | 118032             | Product 4 | 2019-03-15         | 2019-06-19       | P243      | 42511                | Advertiser 2   | 2020-06-20 06:00:00     | 2020-06-30 05:59:59   |
	When I call FindByAdvertiserId method with parameters:
		| Parameter     | Value           |
		| advertiserIds | <AdvertiserIds> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| AdvertiserIds | ExpectedReturnCount |
	| 41197         | 1                   |
	| 42511         | 2                   |
	| 41197, 42511  | 3                   |
	| 70012, 00001  | 1                   |
	| 00001, 00002  | 0                   |

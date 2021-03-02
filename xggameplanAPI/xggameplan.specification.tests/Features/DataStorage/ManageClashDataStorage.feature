@ManageDataStorage

Feature: Manage Clash data storage
	In order to advertise products
	As an Airtime manager
	I want to store clashes in a data store

Background:
	Given there is a Clashes repository

Scenario: Add new Clash
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Clashes
	When I create the following documents:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Remove an existing Clash
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	When I delete document with id '9CE6215F-A43F-4436-B6B5-C96CA063FB3B'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing a non-existing Clash
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	When I delete document with id '5536F139-235B-4303-BE64-A7D609C7CE99'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing Clash by id
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	When I get document with id '8B038BFB-EA51-4650-B85D-B7002CD4C79D'
	Then no documents should be returned

Scenario: Get an existing Clash by id
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	When I get document with id '4A2933E0-A2D2-478F-95DB-ED187FC1126A'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value                                |
         | Uid      | 4A2933E0-A2D2-478F-95DB-ED187FC1126A |

Scenario: Get all Clashes
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Counting all Clashes
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating Clash documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario Outline: Find a Clash by external reference
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	When I call FindByExternal method with parameters:
		| Parameter   | Value  |
		| externalRef | <ExternalRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalRef | ExpectedReturnCount |
	| P468        | 1                   |
	| P488        | 1                   |
	| P523        | 1                   |
	| P777        | 0                   |

Scenario Outline: Find Clashes by external references
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	When I call FindByExternal method with parameters:
		| Parameter   | Value  |
		| externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalRefs     | ExpectedReturnCount |
	| P468, P523       | 2                   |
	| P468, P488, P523 | 3                   |
	| P523, P777       | 1                   |
	| P777, P888       | 0                   |

Scenario Outline: Find Clash with Description by external references
	Given the following documents created:
		| Uid                                  | Externalref | Description             |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | Clothing Stores         |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | Laundromat/Dry Cleaners |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | Clocks & Watches        |
	When I call GetDescriptionByExternalRefs method with parameters:
		| Parameter   | Value  |
		| externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| ExternalRefs     | ExpectedReturnCount |
	| P468, P523       | 2                   |
	| P468, P488, P523 | 3                   |
	| P523, P777       | 1                   |
	| P777, P888       | 0                   |


Scenario Outline: Search Clashes
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description             | DefaultPeakExposureCount | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores         | 1                        | 1                           |
		| 9CE6215F-A43F-4436-B6B5-C96CA063FB3B | P488        | I9                       | Laundromat/Dry Cleaners | 1                        | 1                           |
		| 1BC194EF-7976-40BA-9166-91920732C03C | P523        | I42                      | Clocks & Watches        | 1                        | 1                           |
	When I call Search method with parameters:
		| Parameter | Value       |
		| nameOrRef | <NameOrRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| NameOrRef               | ExpectedReturnCount |
	| P488                    | 1                   |
	| Clothing                | 1                   |
	| P523                    | 1                   |
	| Laundromat/Dry Cleaners | 1                   |
	| P523 Clocks & Watches   | 0                   |

Scenario: Update existing Clashes
	Given the following documents created:
		| Uid                                  | Externalref | ParentExternalidentifier | Description     | DefaultOffPeakExposureCount |
		| 4A2933E0-A2D2-478F-95DB-ED187FC1126A | P468        | I41                      | Clothing Stores | 1                           |
	When I call UpdateRange method with parameters:
		| Property                    | Value                                |
		| uid                         | 4A2933E0-A2D2-478F-95DB-ED187FC1126A |
		| parentExternalIdentifier    | I9                                   |
		| description                 | Laundromat/Dry Cleaners              |
		| defaultOffPeakExposureCount | 2                                    |
	And I get document with id '4A2933E0-A2D2-478F-95DB-ED187FC1126A'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property                    | Value                                |
		| Uid                         | 4A2933E0-A2D2-478F-95DB-ED187FC1126A |
		| Externalref                 | P468                                 |
		| ParentExternalidentifier    | I9                                   |
		| Description                 | Laundromat/Dry Cleaners              |
		| DefaultOffPeakExposureCount | 2                                    |

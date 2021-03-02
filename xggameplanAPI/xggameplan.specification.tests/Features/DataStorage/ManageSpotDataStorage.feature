@ManageDataStorage

Feature: ManageSpotsDataStorage
	In order to manage Spots
	As a Airtime manager
	I want to store Spots via Spot repository

Background: 
	Given there is a Spots repository
	And predefined Spots.SalesAreas.json data
	And predefined data imported

Scenario: Add new Spot
	When I create the following documents:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	And I get all documents
	Then there should be 1 documents returned
	
Scenario: Add new Spots
	When I create the following documents:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
		| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
		| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Update Spot
	Given the following documents created:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
		| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
		| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	When I get document with id 'D43383EB-82AD-4A41-9F08-817FAE6D54B3'
	Then there should be 1 documents returned
	When I update received document by values:
		| Property                  | Value     |
		| ExternalCampaignNumber    | H1566707  |
		| SalesArea                 | NWS91     |
	And I get document with id 'D43383EB-82AD-4A41-9F08-817FAE6D54B3'
	Then the received document should contain the following values:
		| Property                  | Value     |
		| ExternalCampaignNumber    | H1566707  |
		| SalesArea                 | NWS91     |
		| ExternalSpotRef           | G75669243 |

Scenario: Counting all Spots
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Get Spot by id
	Given the following documents created:
	| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
	| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 176003204       | 2019-03-15 17:00:00 |
	| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 176003202       | 2019-03-13 17:00:00 |
	When I get document with id 'D43383EB-82AD-4A41-9F08-817FAE6D54B3'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value  							  |
		| Uid      | D43383EB-82AD-4A41-9F08-817FAE6D54B3 |

Scenario: Delete Spot by id
	Given the following documents created:
	| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
	| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 176003204       | 2019-03-15 17:00:00 |
	| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 176003202       | 2019-03-13 17:00:00 |
	When I delete document with id 'D43383EB-82AD-4A41-9F08-817FAE6D54B3'
	And I get all documents
	Then there should be 4 documents returned

Scenario: Get all Spots
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario Outline: Search Spots by date and sales area
	Given the following documents created:
	| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
	| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 176003204       | 2019-03-15 17:00:00 |
	| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 176003202       | 2019-03-13 17:00:00 |
	When I call Search method with parameters:
         | Parameter | Value       |
         | datefrom  | <DateFrom>  |
         | dateto    | <DateTo>    |
         | salesarea | <SalesArea> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
	| DateFrom   | DateTo     | SalesArea | ExpectedReturnCount |
	| 2019-03-01 | 2019-03-12 | TCN91     | 3                   |
	| 2019-03-11 | 2019-03-15 | TCN91     | 0                   |
	| 2019-03-11 | 2019-03-14 | NWS91     | 1                   |
	| 2019-03-11 | 2019-03-16 | NWS91     | 2                   |
	| 1971-01-01 | 2000-01-01 | TCN91     | 0                   |
	| 2019-03-01 | 2019-03-12 | AAA92     | 0                   |

Scenario Outline: Search Spots by date and sales areas
	Given the following documents created:
	| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
	| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 176003204       | 2019-03-15 17:00:00 |
	| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 176003202       | 2019-03-13 17:00:00 |
	When I call Search method with parameters:
         | Parameter  | Value        |
         | datefrom   | <DateFrom>   |
         | dateto     | <DateTo>     |
         | salesareas | <SalesAreas> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
	| DateFrom   | DateTo     | SalesAreas  | ExpectedReturnCount |
	| 2019-03-01 | 2019-03-12 | TCN91       | 3                   |
	| 2019-03-11 | 2019-03-16 | TCN91,NWS91 | 2                   |
	| 2019-03-09 | 2019-03-14 | TCN91,NWS91 | 4                   |
	| 2019-03-11 | 2019-03-16 | NWS91,AAA92 | 2                   |
	| 2000-01-01 | 2010-01-01 | TCN91,NWS91 | 0                   |
	| 2019-03-09 | 2019-03-14 | AAA92,BBB93 | 0                   |

Scenario Outline: Find Spot by external spot ref
	Given the following documents created:
	| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
	| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 176003204       | 2019-03-15 17:00:00 |
	| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 176003202       | 2019-03-13 17:00:00 |
	When I call FindByExternalSpotRef method with parameters:
         | Parameter       | Value             |
         | externalSpotRef | <ExternalSpotRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
	| ExternalSpotRef | ExpectedReturnCount |
	| 176003202       | 1                   |
	| 176003204       | 1                   |
	| 000000000       | 0                   |

Scenario Outline: Find Spots by external campaign ref
	Given the following documents created:
	| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
	| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 176003204       | 2019-03-15 17:00:00 |
	| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 176003202       | 2019-03-13 17:00:00 |
	When I call FindByExternal method with parameters:
         | Parameter   | Value     |
         | externalRef | <CampRef> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
	| CampRef  | ExpectedReturnCount |
	| H1566707 | 2                   |
	| GRID     | 3                   |
	| A0000000 | 0                   |

Scenario Outline: Find Spots by external spot refs
	Given the following documents created:
	| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalSpotRef | StartDateTime       |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | G75669243       | 2019-03-10 11:00:00 |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | G75669239       | 2019-03-10 11:00:00 |
	| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | G75669235       | 2019-03-10 11:00:00 |
	| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 176003204       | 2019-03-15 17:00:00 |
	| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 176003202       | 2019-03-13 17:00:00 |
	When I call FindByExternal method with parameters:
         | Parameter    | Value          |
         | externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
	| ExternalRefs                  | ExpectedReturnCount |
	| G75669243                     | 1                   |
	| G75669239                     | 1                   |
	| G75669243,G75669235           | 2                   |
	| 176003202,176003204,A00000000 | 2                   |
	| A00000000,B00000000           | 0                   |

Scenario Outline: Find Spots by external break refs
	Given the following documents created:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalBreakNo | StartDateTime       |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | 6318937-3       | 2019-03-10 11:00:00 |
		| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | 4698316-5       | 2019-03-10 11:00:00 |
		| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | 4785138-1       | 2019-03-10 11:00:00 |
		| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 5698753-4       | 2019-03-15 17:00:00 |
		| 66B8F35C-BB99-4A30-9D9E-FEB4163E6C67 | H1566707               | NWS91     | 6518973-7       | 2019-03-13 17:00:00 |
	When I call FindByExternalBreakNumbers method with parameters:
		| Parameter            | Value                  |
		| externalBreakNumbers | <ExternalBreakNumbers> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
	| ExternalBreakNumbers              | ExpectedReturnCount |
	| 6318937-3                         | 1                   |
	| 4698316-5                         | 1                   |
	| 6318937-3, 4698316-5              | 2                   |
	| 4785138-1, 5698753-4, 0000001-3   | 2                   |
	| 0000001-3, 0000003-5              | 0                   |

Scenario Outline: Delete Spots by ids
	Given the following documents created:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalBreakNo | StartDateTime       |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | 6318937-3       | 2019-03-10 11:00:00 |
		| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | 4698316-5       | 2019-03-10 11:00:00 |
		| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | 4785138-1       | 2019-03-10 11:00:00 |
	When I call Delete method with parameters:
		| Parameter | Value |
		| ids       | <Ids> |
	And I get all documents
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids                                                                        | ExpectedReturnCount  |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3                                       | 2                    |
	| D43383EB-82AD-4A41-9F08-817FAE6D54B3, 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | 1                    |
	| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D, 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | 1                    |
	| 6F985262-8D85-4BFC-986C-D91429E781D2                                       | 3                    |

Scenario: Search Multipart Spots
	Given the following documents created:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalBreakNo | MultipartSpot   |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | 6318937-3       |                 |
		| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | 4698316-5       | TT              |
		| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | 4785138-1       | SB              |
		| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 5698753-4       | SB              |
	When I call GetAllMultipart method
	Then there should be 3 documents returned

Scenario: Truncate Spots documents
	Given 3 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario: Create spot via InsertOrReplace method
	Given the following documents created:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalBreakNo | MultipartSpot | ExternalSpotRef | Id |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | 6318937-3       |               | 1               | 1  |
		| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | 4698316-5       | TT            | 2               | 2  |
		| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | 4785138-1       | SB            | 3               | 3  |
		| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 5698753-4       | SB            | 4               | 4  |
	When I call InsertOrReplace method with parameters:
		| Property               | Value                                |
		| id                     | 75dcd652-fd1f-4da4-92c9-4d96501b193a |
		| externalCampaignNumber | 545                                  |
		| salesArea              | SCFR4                                |
		| externalBreakNo        | 5698753-7                            |
		| multipartSpot          | SD                                   |
		| externalSpotRef        | 5                                    |
	And I get all documents
	Then there should be 5 documents returned

Scenario: Replace spot via InsertOrReplace method
	Given the following documents created:
		| Uid                                  | ExternalCampaignNumber | SalesArea | ExternalBreakNo | MultipartSpot | ExternalSpotRef | Id |
		| D43383EB-82AD-4A41-9F08-817FAE6D54B3 | GRID                   | TCN91     | 6318937-3       |               | 1               | 1  |
		| 57AC239E-C6D7-45BF-BDC8-01B35E93EC9D | GRID                   | TCN91     | 4698316-5       | TT            | 2               | 2  |
		| 4C5AAB5E-EFDF-476A-ADC1-C89D66CDBB2A | GRID                   | TCN91     | 4785138-1       | SB            | 3               | 3  |
		| D8F1C734-670F-4B70-9BEA-6CD7AA5B8422 | H1566707               | NWS91     | 5698753-4       | SB            | 4               | 4  |
	When I call InsertOrReplace method with parameters:
		| Property               | Value                                |
		| id                     | 75dcd652-fd1f-4da4-92c9-4d96501b193a |
		| externalCampaignNumber | 545                                  |
		| salesArea              | SCFR4                                |
		| externalBreakNo        | 5698753-7                            |
		| multipartSpot          | SD                                   |
		| externalSpotRef        | 4                                    |
	And I get all documents
	Then there should be 4 documents returned

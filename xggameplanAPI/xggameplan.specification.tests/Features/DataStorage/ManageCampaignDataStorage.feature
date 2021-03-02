@ManageDataStorage

Feature: Manage Campaign data storage
	In order to advertise products
	As an Airtime manager
	I want to store campaigns in a data store

Background:
	Given there is a Campaigns repository
	And predefined Campaigns data

Scenario: Add new Campaign
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Campaigns
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned


Scenario: Remove an existing Campaign
	Given predefined data imported
	When I delete document with id 'FC2C1BF6-82B8-4759-9286-0B872D2D3FAE'
	And I get all documents
	Then there should be 3 documents returned


Scenario: Removing a non-existing Campaign
	Given predefined data imported
	When I delete document with id '735D6DCC-B101-49F1-9B54-36D3D8DAD0D7'
	And I get all documents
	Then there should be 4 documents returned


Scenario: Get a non-existing Campaign by id
	Given predefined data imported
	When I get document with id '735D6DCC-B101-49F1-9B54-36D3D8DAD0D7'
	Then no documents should be returned


Scenario: Get an existing Campaign by id
	Given predefined data imported
	When I get document with id 'D3D6667D-4A91-49CC-80A3-68E31324F8E2'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value                                |
		| Id       | D3D6667D-4A91-49CC-80A3-68E31324F8E2 |

Scenario: Get all Campaigns
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get all flat Campaign models
	Given 3 documents created
	When I call GetAllFlat method
	Then there should be 3 documents returned

Scenario: Counting all Campaigns
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Truncating Campaign documents
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned


Scenario: Get all active Campaigns
	Given predefined data imported
	When I call GetAllActive method
	Then there should be 2 documents returned


Scenario Outline: Find Campaigns by list of id
	Given predefined data imported
	When I call Find method with parameters:
		| Parameter | Value  |
		| uids      | <uids> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| uids                                                                       | ExpectedReturnCount |
		| B438FAF1-DC2E-444A-BC1B-E124EAD57936, FC2C1BF6-82B8-4759-9286-0B872D2D3FAE | 2                   |
		| FC2C1BF6-82B8-4759-9286-0B872D2D3FAE, 3042C3FD-C8E8-4979-B655-BB9D87497D64 | 1                   |
		| 00000000-0000-0000-0000-000000000001, 00000000-0000-0000-0000-000000000002 | 0                   |


Scenario: Get Campaign by external reference
	Given predefined data imported
	When I call FindByRef method with parameters:
		| Parameter   | Value      |
		| externalref | 15G1572617 |
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property   | Value                                |
		| Id         | FC2C1BF6-82B8-4759-9286-0B872D2D3FAE |
		| ExternalId | 15G1572617                           |


Scenario: Find distinct business types
	Given predefined data imported
	When I call GetBusinessTypes method
	Then there should be 1 documents returned


Scenario Outline: Find Campaigns by external references
	Given predefined data imported
	When I call FindByRefs method with parameters:
		| Parameter    | Value          |
		| externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| ExternalRefs                       | ExpectedReturnCount |
		| 30G1572616, 15G1572617             | 2                   |
		| 15G1572616, 15G1572617, 30G1572617 | 3                   |
		| 15G1572616, 15G1572617, EMPTY_ONE  | 2                   |
		| EMPTY_ONE, EMPTY_TWO               | 0                   |
		

Scenario Outline: Find Campaign names by external references
	Given predefined data imported
	When I call FindNameByRefs method with parameters:
		| Parameter    | Value          |
		| externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| ExternalRefs                       | ExpectedReturnCount |
		| 30G1572616, 15G1572617             | 2                   |
		| 15G1572616, 15G1572617, 30G1572617 | 3                   |
		| 15G1572616, 15G1572617, EMPTY_ONE  | 2                   |
		| EMPTY_ONE, EMPTY_TWO               | 0                   |


Scenario Outline: Get Campaigns by group name
	Given predefined data imported
	When I call GetByGroup method with parameters:
		| Parameter | Value   |
		| group     | <Group> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| Group    | ExpectedReturnCount |
		| G1572617 | 2                   |
		| G1572616 | 2                   |
		| G1572618 | 0                   |


Scenario Outline: Find missing Campaigns from group
	Given predefined data imported
	When I call FindMissingCampaignsFromGroup method with parameters:
		| Parameter     | Value           |
		| externalRefs  | <ExternalRefs>  |
		| campaignGroup | <CampaignGroup> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| ExternalRefs                                   | CampaignGroup        | ExpectedReturnCount |
		| 30G1572616, 15G1572617                         | G1572617, G1572616   | 2                   |
		| 15G1572616, 15G1572617                         | G1572616, EMPTY_ONE  | 1                   |
		| 15G1572616, 15G1572617, 30G1572616, 30G1572617 | G1572616, G1572617   | 0                   |
		| EMPTY_ONE, EMPTY_TWO                           | EMPTY_ONE, EMPTY_TWO | 0                   |


#it seems that the repository query can't be processed by in-memory provider
Scenario Outline: Get Campaigns with product
	Given predefined data imported
	When I call GetWithProduct method with parameters:
		| Parameter   | Value         |
		| status      | <Status>      |
		| startDate   | <StartDate>   |
		| endDate     | <EndDate>     |
		| description | <Description> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| Status    | StartDate  | EndDate    | Description  | ExpectedReturnCount |
		| All       | null       | null       | null         | 4                   |
		| All       | null       | null       | g1572616     | 2                   |
		| All       | 2019-01-08 | 2019-04-26 | null         | 4                   |
		| All       | 2019-04-28 | 2019-06-30 | null         | 2                   |
		| All       | 2019-04-28 | 2019-06-30 | Advertiser 2 | 2                   |
		| All       | 2019-01-10 | 2019-05-15 | EMPTY_ONE    | 0                   |
		| Active    | null       | null       | null         | 2                   |
		| Active    | null       | null       | G1572617     | 1                   |
		| Active    | 2019-02-15 | 2019-06-03 | null         | 2                   |
#		TODO: Revise what is wrong with next line. Note Campaigns.json does not contains Campaign with name
#		| Active    | 2019-02-15 | 2019-06-03 | Adv 2        | 1                   |
		| Active    | 2019-02-15 | 2019-06-03 | EMPTY_ONE    | 0                   |
		| Cancelled | null       | null       | null         | 2                   |
		| Cancelled | null       | null       | G1572616     | 1                   |
		| Cancelled | 2019-02-15 | 2019-04-30 | null         | 2                   |
		| Cancelled | 2019-02-15 | 2019-04-30 | Campaign One | 1                   |
		| Cancelled | 2019-02-15 | 2019-04-30 | EMPTY_ONE    | 0                   |


Scenario: Update Campaign by external reference
	Given predefined data imported
	When I call FindByRef method with parameters:
		| Parameter   | Value      |
		| externalref | 15G1572617 |
	And I update received document by values:
		| Property | Value        |
		| Name     | UpdatedName1 |
	And I call FindByRef method with parameters:
		| Parameter   | Value      |
		| externalref | 15G1572617 |
	Then the received document should contain the following values:
		| Property   | Value                                |
		| Id         | FC2C1BF6-82B8-4759-9286-0B872D2D3FAE |
		| ExternalId | 15G1572617                           |
		| Name       | UpdatedName1                         |

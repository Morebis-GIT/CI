@ManageDataStorage

Feature: Manage Scenarios data storage
	In order to manage Scenarios
	As a user
	I want to store restrictions via ScenariosRepository

Background:
	Given there is a Scenarios repository
	And predefined Scenarios data

Scenario: Add new Scenario
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new Scenarios
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get an existing Scenario by id
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | True        |
	When I get document with id '3c597a5a-f21f-4a21-b79a-8ddcd07c7c30'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value                                |
		| Id       | 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 |

Scenario: Get a non-existing Scenario by id
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | True        |
	When I get document with id '50acfdea-6d6c-4726-a43f-30990155148f'
	Then no documents should be returned

Scenario: Remove an existing Scenario
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | True        |
	When I delete document with id '3c597a5a-f21f-4a21-b79a-8ddcd07c7c30'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Removing a non-existing Scenario
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | True        |
	When I delete document with id '50acfdea-6d6c-4726-a43f-30990155148f'
	And I get all documents
	Then there should be 3 documents returned

Scenario: Update Scenario
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | True        |
	When I get document with id '3c597a5a-f21f-4a21-b79a-8ddcd07c7c30'
	Then there should be 1 documents returned
	When I update received document by values:
		| Property    | Value       |
		| Name        | ScenarioNew |
		| CustomId    | 7890        |
		| IsLibraried | True        |
	And I get document with id '3c597a5a-f21f-4a21-b79a-8ddcd07c7c30'
	Then the received document should contain the following values:
		| Property    | Value       |
		| Name        | ScenarioNew |
		| CustomId    | 7890        |
		| IsLibraried | True        |

Scenario Outline: Find Scenario by name
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | False       |
	When I call FindByName method with parameters:
		| Parameter   | Value         |
		| name        | <Name>        |
		| isLibraried | <IsLibraried> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| Name          | IsLibraried | ExpectedReturnCount |
		| Scenario1     | True        | 1                   |
		| Scenario1     | False       | 1                   |
		| Scenario2     | True        | 0                   |
		| Scenario2     | False       | 1                   |
		| TestScenario3 | True        | 0                   |
		| Scenario      | True        | 0                   |
		| ""            | True        | 0                   |
		| Sdrgiowef     | False       | 0                   |

Scenario Outline: Find Scenario by Ids
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | False       |
	When I call FindByIds method with parameters:
		| Parameter | Value |
		| ids       | <Ids> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| Ids                                                                       | ExpectedReturnCount |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42,3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | 2                   |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30                                      | 1                   |
		| e762ab40-a492-41fa-a04c-5cf1fdaaf863                                      | 0                   |

Scenario: Get scenarios with isLibrary flag
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | True        |
	When I call GetLibraried method
	Then there should be 2 documents returned

Scenario: Get Scenario by Pass id
	Given predefined data imported
	When I call GetByPassId method with parameters:
		| Parameter | Value |
		| passId    | 5412  |
	Then there should be 2 documents returned

Scenario: Get Scenarios with Pass id
	Given predefined data imported
	When I call GetScenariosWithPassId method
	Then there should be 7 documents returned

Scenario: Get Scenarios with Pass id by ids
	Given predefined data imported
	When I call GetScenariosWithPassId method with parameters:
		| Parameter   | Value                                                                      |
		| scenarioIds | 36a6b642-96eb-480a-9155-ac8182b9c5db, 41210db5-d545-4785-8da4-811ab0954b7f |
	Then there should be 4 documents returned

Scenario: Remove Scenarios by ids
	Given the following documents created:
		| Id                                   | Name      | CustomId | DateCreated | DateModified | IsLibraried |
		| 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42 | Scenario1 | 2303     | 2020-01-01  | 2020-01-15   | True        |
		| 3c597a5a-f21f-4a21-b79a-8ddcd07c7c30 | Scenario2 | 4502     | 2020-02-02  | 2020-02-15   | False       |
		| ebe99aeb-25e8-4bf2-baee-4cf7ca20903f | Scenario3 | 4578     | 2020-03-03  | 2020-03-17   | True        |
		| efc1510c-4da2-462a-90ab-d43479d10b1b | Scenario4 | 4478     | 2020-03-03  | 2020-03-17   | True        |
	When I call Remove method with parameters:
		| Parameter | Value                                                                      |
		| ids       | 2aa2b6f3-dcb5-4baf-838b-f5c1114cbb42, ebe99aeb-25e8-4bf2-baee-4cf7ca20903f |
	And I get all documents
	Then there should be 2 documents returned

Scenario: Get all Scenarios
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

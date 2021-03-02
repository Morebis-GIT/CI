@ManageDataStorage

Feature: ManageUniverseDataStorage
	In order to manage universes
	As a user
	I want to store universes via Universes repository

Background:
	Given there is a Universes repository
	And predefined Universe.SalesAreas.json data
	And predefined data imported

Scenario: Add new universes
	Given the following documents created:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 3D71B599-B981-4C4D-B528-358112A37C04 | 7         | 3           | 2018-3-5  | 2018-9-7  | 16            |
	When I create the following documents:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 13BE3EDC-F1EF-4818-9D71-67E87AD490D7 | 9         | 6           | 2018-2-17 | 2018-5-19 | 21            |
		| ED40849D-DB1A-4CA6-84FA-6FDBB8A15849 | 12        | 9           | 2018-6-12 | 2018-10-3 | 315           |
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get all universes
	Given the following documents created:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 3D71B599-B981-4C4D-B528-358112A37C04 | 7         | 3           | 2018-3-5  | 2018-9-7  | 16            |
		| 13BE3EDC-F1EF-4818-9D71-67E87AD490D7 | 9         | 6           | 2018-2-17 | 2018-5-19 | 21            |
		| ED40849D-DB1A-4CA6-84FA-6FDBB8A15849 | 12        | 9           | 2018-6-12 | 2018-10-3 | 315           |
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get universe by id
	Given the following documents created:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 3D71B599-B981-4C4D-B528-358112A37C04 | 7         | 3           | 2018-3-5  | 2018-9-7  | 16            |
		| 13BE3EDC-F1EF-4818-9D71-67E87AD490D7 | 9         | 6           | 2018-2-17 | 2018-5-19 | 21            |
		| ED40849D-DB1A-4CA6-84FA-6FDBB8A15849 | 12        | 9           | 2018-6-12 | 2018-10-3 | 315           |
	When I get document with id '3D71B599-B981-4C4D-B528-358112A37C04'
	Then there should be 1 documents returned

Scenario: Get a non-existing universe by id
	Given the following documents created:
		| Id                                   | SalesArea |
		| 720906C7-EB98-4057-9175-962AF459EA55 | 7         |
		| 2259294C-24E6-45B7-B1EF-2BAA53CF4278 | 9         |
		| 6D5304D7-5848-4577-8F31-65305E8DA0BD | 12        |
	When I get document with id 'DA60CF83-C553-41EB-BFDE-344DA2020FFD'
	Then no documents should be returned

Scenario: Delete universe by id
	Given the following documents created:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 3D71B599-B981-4C4D-B528-358112A37C04 | 7         | 3           | 2018-3-5  | 2018-9-7  | 16            |
		| 13BE3EDC-F1EF-4818-9D71-67E87AD490D7 | 9         | 6           | 2018-2-17 | 2018-5-19 | 21            |
		| ED40849D-DB1A-4CA6-84FA-6FDBB8A15849 | 12        | 9           | 2018-6-12 | 2018-10-3 | 315           |
	When I delete document with id '13BE3EDC-F1EF-4818-9D71-67E87AD490D7'
	And I get all documents
	Then there should be 2 documents returned

Scenario: Truncate universes
	Given 5 documents created
	When I truncate documents
	And I get all documents
	Then no documents should be returned

Scenario: Update a universe
	Given the following documents created:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 3D71B599-B981-4C4D-B528-358112A37C04 | 7         | 3           | 2018-3-5  | 2018-9-7  | 16            |
		| 13BE3EDC-F1EF-4818-9D71-67E87AD490D7 | 9         | 6           | 2018-2-17 | 2018-5-19 | 21            |
		| ED40849D-DB1A-4CA6-84FA-6FDBB8A15849 | 12        | 9           | 2018-6-12 | 2018-10-3 | 315           |
	When I get document with id 'ED40849D-DB1A-4CA6-84FA-6FDBB8A15849'
	And I update received document by values:
		| Property      | Value    |
		| SalesArea     | U7115    |
		| Demographic   | GS 55-18 |
		| UniverseValue | 2        |
	And I get document with id 'ED40849D-DB1A-4CA6-84FA-6FDBB8A15849'
	Then the received document should contain the following values:
		| Property      | Value     |
		| SalesArea     | U7115     |
		| Demographic   | GS 55-18  |
		| StartDate     | 2018-6-12 |
		| EndDate       | 2018-10-3 |
		| UniverseValue | 2         |

Scenario: Get universes by sales area and demographic values
	Given the following documents created:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 3D71B599-B981-4C4D-B528-358112A37C04 | 7         | 3           | 2018-3-5  | 2018-9-7  | 16            |
		| 13BE3EDC-F1EF-4818-9D71-67E87AD490D7 | 9         | 6           | 2018-2-17 | 2018-5-19 | 21            |
		| ED40849D-DB1A-4CA6-84FA-6FDBB8A15849 | 12        | 9           | 2018-6-12 | 2018-10-3 | 315           |
		| 2A806C6C-B332-4E47-8758-A105CC977046 | 9         | 6           | 2018-4-21 | 2018-8-11 | 46            |
	When I call GetBySalesAreaDemo method with parameters:
		| Parameter   | Value |
		| salesarea   | 9     |
		| demographic | 6     |
	Then there should be 2 documents returned

Scenario Outline: Delete universes by combinations
	Given the following documents created:
		| Id                                   | SalesArea | Demographic | StartDate | EndDate   | UniverseValue |
		| 3D71B599-B981-4C4D-B528-358112A37C04 | 7         | 3           | 2018-3-5  | 2018-9-7  | 16            |
		| 13BE3EDC-F1EF-4818-9D71-67E87AD490D7 | 9         | 6           | 2018-2-17 | 2018-5-19 | 21            |
		| ED40849D-DB1A-4CA6-84FA-6FDBB8A15849 | 12        | 9           | 2018-6-12 | 2018-10-3 | 315           |
		| 2A806C6C-B332-4E47-8758-A105CC977046 | 9         | 6           | 2018-4-21 | 2018-8-11 | 46            |
	When I call DeleteByCombination method with parameters:
		| Parameter   | Value         |
		| salesArea   | <salesArea>   |
		| demographic | <demographic> |
		| startDate   | <startDate>   |
		| endDate     | <endDate>     |
	And I get all documents
	Then there should be <expectedReturnCount> documents returned

	Examples:
		| salesArea | demographic | startDate | endDate   | expectedReturnCount |
		| 9         | 6           | null      | null      | 2                   |
		| 9         | 6           | 2018-2-10 | 2018-5-22 | 3                   |
		| 7         | 3           | 2018-3-6  | 2018-9-8  | 4                   |

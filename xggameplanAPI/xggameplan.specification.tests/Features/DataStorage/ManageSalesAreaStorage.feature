@ManageDataStorage

Feature: ManageSalesAreaStorage
	In order to manage sales areas
	As a user
	I want to store sales areas via SalesArea repository

Background:
	Given there is a SalesArea repository

Scenario: Add new SalesArea
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Add new SalesAreas
	When I create 3 documents
	And I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing SalesArea by id
	Given the following documents created:
		| Id                                   | Name  | ShortName | CustomId |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | 1        |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | 2        |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | 3        |
	When I get document with id '00000000-0000-0000-0000-000000000000'
	Then no documents should be returned

Scenario: Get an existing SalesArea by id
	Given the following documents created:
		| Id                                   | Name  | ShortName | CustomId |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | 1        |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | 2        |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | 3        |
	When I get document with id '6DAD8F17-04E5-447D-9DEA-1DEA6950A40E'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property | Value                                |
         | Id       | 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E |

Scenario: Counting all SalesAreas
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario: Update the SalesArea
	Given the following documents created:
		| Id                                   | Name  | ShortName | CustomId |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | 1        |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | 2        |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | 3        |
	When I get document with id '5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB'
	And I update received document by values:
		| Property     | Value    |
		| CurrencyCode | GBP      |
		| StartOffset  | 06:27:15 |
		| DayDuration  | 05:15:42 |
	And I get document with id '5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB'
	Then the received document should contain the following values:
		| Property     | Value    |
		| Name         | STW92    |
		| ShortName    | S92      |
		| CurrencyCode | GBP      |
		| StartOffset  | 06:27:15 |
		| DayDuration  | 05:15:42 |

Scenario: Delete SalesArea by id
	Given the following documents created:
		| Id                                   | Name  | ShortName | CustomId |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | 1        |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | 2        |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | 3        |
	When I delete document with id '423D0C30-4661-4132-99ED-56ED9C64F205'
	And I get all documents
	Then there should be 2 documents returned

Scenario Outline: Find SalesAreas by names
	Given the following documents created:
		| Id                                   | Name  | ShortName | CustomId |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | 1         |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | 2         |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | 3         |
	When I call FindByNames method with parameters:
		| Parameter | Value   |
		| names     | <Names> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Names                | ExpectedReturnCount |
	| QTQ91                | 1                   |
	| GTV93, STW92         | 2                   |
	| STW92, QTQ91, GTV93  | 3                   |
	| GTV93, NAME_1, GTV93 | 1                   |
	| QTQ91, NAME_1        | 1                   |
	| NAME_1, NAME_2       | 0                   |

Scenario Outline: Find SalesAreas by custom ids
	Given the following documents created:
		| Id                                   | Name  | ShortName | CustomId |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | 1        |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | 2        |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | 3        |
	When I call FindByIds method with parameters:
		| Parameter | Value |
		| Ids       | <Ids> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids     | ExpectedReturnCount |
	| 1, 2    | 2                   |
	| 1, 2, 3 | 3                   |
	| 3, 1, 6 | 2                   |
	| 2       | 1                   |
	| 7, 9    | 0                   |

Scenario Outline: Find SalesArea by name
	Given the following documents created:
		| Id                                   | Name  | ShortName | CustomId |
		| 423D0C30-4661-4132-99ED-56ED9C64F205 | QTQ91 | Q91       | 1        |
		| 6DAD8F17-04E5-447D-9DEA-1DEA6950A40E | GTV93 | G93       | 2        |
		| 5DCC4E91-EDDE-4F8C-9001-56AACE72C9EB | STW92 | S92       | 3        |
	When I call FindByName method with parameters:
		| Parameter | Value   |
		| name     | <Name> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Name   | ExpectedReturnCount |
	| QTQ91  | 1                   |
	| gtv93  | 1                   |
	| Stw92  | 1                   |
	| NAME_1 | 0                   |

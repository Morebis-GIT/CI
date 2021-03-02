@ManageDataStorage

Feature: ManageSpotPlacementDataStorage
	In order to manage SpotPlacement
	As a Airtime manager
	I want to store SpotPlacement via SpotPlacement repository

Background:
	Given there is a SpotPlacement repository

Scenario: Add new SpotPlacement
	When I create a document with values:
		| Id | ModifiedTime     | ExternalSpotRef | ExternalBreakRef | ResetExternalBreakRef |
		| 1  | 2019-03-04 23:45 | S2345           | B4523            | B533                  |
	And I call GetByExternalSpotRef method with parameters:
		| Parameter       | Value |
		| externalSpotRef | S2345 |
	Then there should be 1 documents returned

Scenario: Add new SpotPlacements
	When I create the following documents:
		| Id | ModifiedTime     | ExternalSpotRef | ExternalBreakRef | ResetExternalBreakRef |
		| 1  | 2019-03-04 23:45 | S2345           | B4523            | B533                  |
		| 2  | 2019-05-04 23:45 | S2346           | B4524            | B534                  |
		| 3  | 2019-06-04 23:45 | S2347           | B4525            | B535                  |
	And I call GetByExternalSpotRefs method with parameters:
		| Parameter        | Value       |
		| externalSpotRefs | S2345,S2347 |
	Then there should be 2 documents returned

Scenario: Remove SpotPlacement by id
	Given the following documents created:
		| Id | ModifiedTime     | ExternalSpotRef | ExternalBreakRef | ResetExternalBreakRef |
		| 1  | 2019-03-04 23:45 | S2345           | B4523            | B533                  |
		| 2  | 2019-05-04 23:45 | S2346           | B4524            | B534                  |
		| 3  | 2019-06-04 23:45 | S2347           | B4525            | B535                  |
	When I delete document with id '2'
	And I call GetByExternalSpotRef method with parameters:
		| Parameter       | Value |
		| externalSpotRef | S2346 |
	Then no documents should be returned

Scenario: Remove SpotPlacement by spotRef
	Given the following documents created:
		| Id | ModifiedTime     | ExternalSpotRef | ExternalBreakRef | ResetExternalBreakRef |
		| 1  | 2019-03-04 23:45 | S2345           | B4523            | B533                  |
		| 2  | 2019-05-04 23:45 | S2346           | B4524            | B534                  |
		| 3  | 2019-06-04 23:45 | S2347           | B4525            | B535                  |
	When I call DeleteByExternalSpotRef method with parameters:
		| Parameter       | Value |
		| externalSpotRef | S2346 |
	And I call GetByExternalSpotRef method with parameters:
		| Parameter       | Value |
		| externalSpotRef | S2346 |
	Then no documents should be returned

Scenario: Remove SpotPlacement before date
	Given the following documents created:
		| Id | ModifiedTime     | ExternalSpotRef | ExternalBreakRef | ResetExternalBreakRef |
		| 1  | 2019-03-04 23:45 | S2345           | B4523            | B533                  |
		| 2  | 2019-05-04 23:45 | S2346           | B4524            | B534                  |
		| 3  | 2019-06-04 23:45 | S2347           | B4525            | B535                  |
	When I call DeleteBefore method with parameters:
		| Parameter    | Value      |
		| modifiedTime | 2019-06-01 |
	And I call GetByExternalSpotRefs method with parameters:
		| Parameter        | Value       |
		| externalSpotRefs | S2345,S2346 |
	Then no documents should be returned

Scenario: Update SpotPlacement
	Given the following documents created:
		| Id | ModifiedTime     | ExternalSpotRef | ExternalBreakRef | ResetExternalBreakRef |
		| 1  | 2019-03-04 23:45 | S2345           | B4523            | B533                  |
		| 2  | 2019-05-04 23:45 | S2346           | B4524            | B534                  |
		| 3  | 2019-06-04 23:45 | S2347           | B4525            | B535                  |
	When I call GetByExternalSpotRef method with parameters:
		| Parameter       | Value |
		| externalSpotRef | S2346 |
	Then there should be 1 documents returned
	When I update received document by values:
		| Property              | Value      |
		| ModifiedTime          | 2020-01-01 |
		| ExternalBreakRef      | B333       |
		| ResetExternalBreakRef | B222       |
	When I call GetByExternalSpotRef method with parameters:
		| Parameter       | Value |
		| externalSpotRef | S2346 |
	Then the received document should contain the following values:
		| Property              | Value      |
		| ModifiedTime          | 2020-01-01 |
		| ExternalSpotRef       | S2346      |
		| ExternalBreakRef      | B333       |
		| ResetExternalBreakRef | B222       |

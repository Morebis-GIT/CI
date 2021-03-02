@ManageDataStorage

Feature: ManageTaskInstanceDataStorage
	In order to manage TaskInstances
	As an api user
	I want to store TaskInstance via TaskInstance repository

Background:
	Given there is a TaskInstance repository

Scenario: Add new TaskInstance
	When I create a document
	And I get all documents
	Then there should be 1 documents returned

Scenario: Get all TaskInstances
	Given 3 documents created
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing TaskInstance by id
	Given the following documents created:
		| Id                                   | TaskId   | TenantId | Status     | TimeCreated      | TimeCompleted    | TimeLastActive   |
		| 9fa57568-4414-4491-aa28-d6b64dc25283 | StartRun | 1        | InProgress | 2019-01-01 09:30 | 2019-01-01 10:30 | 2019-01-01 10:00 |
		| 5ffe88ad-6c2e-4c33-90a0-c9b853f332d2 | StartRun | 1        | Starting   | 2019-10-10 09:30 | 2019-10-10 10:30 | 2019-10-10 10:00 |
		| 4f068201-0018-4eb8-8543-8053d78dc147 | StartRun | 1        | InProgress | 2019-11-11 09:30 | 2019-11-11 10:30 | 2019-11-11 10:00 |
	When I get document with id '020d7e6b-64f7-44cc-b728-36330f10be6f'
	Then no documents should be returned

Scenario: Get an existing TaskInstance by id
	Given the following documents created:
		| Id                                   | TaskId   | TenantId | Status     | TimeCreated      | TimeCompleted    | TimeLastActive   |
		| 9fa57568-4414-4491-aa28-d6b64dc25283 | StartRun | 1        | InProgress | 2019-01-01 09:30 | 2019-01-01 10:30 | 2019-01-01 10:00 |
		| 5ffe88ad-6c2e-4c33-90a0-c9b853f332d2 | StartRun | 2        | Starting   | 2019-10-10 09:30 | 2019-10-10 10:30 | 2019-10-10 10:00 |
		| 4f068201-0018-4eb8-8543-8053d78dc147 | StartRun | 3        | InProgress | 2019-11-11 09:30 | 2019-11-11 10:30 | 2019-11-11 10:00 |
	When I get document with id '5ffe88ad-6c2e-4c33-90a0-c9b853f332d2'
	Then there should be 1 documents returned
	And the received document should contain the following values:
         | Property       | Value                                |
         | Id             | 5ffe88ad-6c2e-4c33-90a0-c9b853f332d2 |
         | TaskId         | StartRun                             |
         | TenantId       | 2                                    |
         | Status         | Starting                             |
         | TimeCreated    | 2019-10-10 09:30                     |
         | TimeCompleted  | 2019-10-10 10:30                     |
         | TimeLastActive | 2019-10-10 10:00                     |

Scenario: Update a TaskInstance
	Given the following documents created:
		| Id                                   | TaskId   | TenantId | Status     | TimeCreated      | TimeCompleted    | TimeLastActive   |
		| 9fa57568-4414-4491-aa28-d6b64dc25283 | StartRun | 1        | InProgress | 2019-01-01 09:30 | 2019-01-01 10:30 | 2019-01-01 10:00 |
		| 5ffe88ad-6c2e-4c33-90a0-c9b853f332d2 | StartRun | 2        | Starting   | 2019-10-10 09:30 | 2019-10-10 10:30 | 2019-10-10 10:00 |
		| 4f068201-0018-4eb8-8543-8053d78dc147 | StartRun | 3        | InProgress | 2019-11-11 09:30 | 2019-11-11 10:30 | 2019-11-11 10:00 |
	When I get document with id '5ffe88ad-6c2e-4c33-90a0-c9b853f332d2'
	And I update received document by values:
         | Property       | Value            |
         | TaskId         | Completed        |
         | TenantId       | 5                |
         | Status         | InProgress       |
         | TimeCreated    | 2020-05-05 04:30 |
         | TimeCompleted  | 2020-05-05 05:30 |
         | TimeLastActive | 2020-05-05 04:00 |
	And I get document with id '5ffe88ad-6c2e-4c33-90a0-c9b853f332d2'
	Then the received document should contain the following values:
         | Property       | Value                                |
         | Id             | 5ffe88ad-6c2e-4c33-90a0-c9b853f332d2 |
         | TaskId         | Completed                            |
         | TenantId       | 5                                    |
         | Status         | InProgress                           |
         | TimeCreated    | 2020-05-05 04:30                     |
         | TimeCompleted  | 2020-05-05 05:30                     |
         | TimeLastActive | 2020-05-05 04:00                     |

Scenario: Remove an existing TaskInstance
	Given the following documents created:
		| Id                                   | TaskId   | TenantId | Status     | TimeCreated      | TimeCompleted    | TimeLastActive   |
		| 9fa57568-4414-4491-aa28-d6b64dc25283 | StartRun | 1        | InProgress | 2019-01-01 09:30 | 2019-01-01 10:30 | 2019-01-01 10:00 |
		| 5ffe88ad-6c2e-4c33-90a0-c9b853f332d2 | StartRun | 2        | Starting   | 2019-10-10 09:30 | 2019-10-10 10:30 | 2019-10-10 10:00 |
		| 4f068201-0018-4eb8-8543-8053d78dc147 | StartRun | 3        | InProgress | 2019-11-11 09:30 | 2019-11-11 10:30 | 2019-11-11 10:00 |
	When I delete document with id '5ffe88ad-6c2e-4c33-90a0-c9b853f332d2'
	And I get document with id '5ffe88ad-6c2e-4c33-90a0-c9b853f332d2'
	Then no documents should be returned

Scenario: Removing a non-existing TaskInstance
	Given the following documents created:
		| Id                                   | TaskId   | TenantId | Status     | TimeCreated      | TimeCompleted    | TimeLastActive   |
		| 9fa57568-4414-4491-aa28-d6b64dc25283 | StartRun | 1        | InProgress | 2019-01-01 09:30 | 2019-01-01 10:30 | 2019-01-01 10:00 |
		| 5ffe88ad-6c2e-4c33-90a0-c9b853f332d2 | StartRun | 2        | Starting   | 2019-10-10 09:30 | 2019-10-10 10:30 | 2019-10-10 10:00 |
		| 4f068201-0018-4eb8-8543-8053d78dc147 | StartRun | 3        | InProgress | 2019-11-11 09:30 | 2019-11-11 10:30 | 2019-11-11 10:00 |
	When I delete document with id '839271af-4b88-491a-ac83-0a89e06de769'
	And I get all documents
	Then there should be 3 documents returned

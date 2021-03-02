@ManageDataStorage

Feature: Manage Smooth Failure data storage
    In order to manage smooth failures
    As an airtime manager
    I want to store smooth failures in a data store

Background:
	Given there is a SmoothFailures repository

Scenario: Add several new smooth failures
	When I create the following documents:
		| RunId                                | TypeId | SalesArea |
		| 1771D896-912C-42D3-8131-3740805D7772 | 1      | TCN93     |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 1      | TCN93     |
		| 4AB41BB6-5CED-421F-93E1-F8C355728877 | 1      | TCN93     |
	And I call GetByRunId method with parameters:
		| Parameter | Value                                |
		| runId     | C499E915-3AFB-46B4-B8ED-35A68D551ED4 |
	Then there should be 1 documents returned

Scenario Outline: Get smooth failures by run id
	Given the following documents created:
		| RunId                                | TypeId | SalesArea |
		| 1771D896-912C-42D3-8131-3740805D7772 | 1      | TCN93     |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 1      | TCN91     |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 1      | TCN92     |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 1      | TCN93     |
		| 4AB41BB6-5CED-421F-93E1-F8C355728877 | 1      | TCN92     |
		| 4AB41BB6-5CED-421F-93E1-F8C355728877 | 1      | TCN93     |
	When I call GetByRunId method with parameters:
		| Parameter | Value   |
		| runId     | <RunId> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| RunId                                | ExpectedReturnCount |
		| 1771D896-912C-42D3-8131-3740805D7772 | 1                   |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 3                   |
		| 4AB41BB6-5CED-421F-93E1-F8C355728877 | 2                   |

Scenario Outline: Delete smooth failures by run id
	Given the following documents created:
		| RunId                                | TypeId | SalesArea |
		| 1771D896-912C-42D3-8131-3740805D7772 | 1      | TCN93     |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 1      | TCN91     |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 1      | TCN92     |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 1      | TCN93     |
		| 4AB41BB6-5CED-421F-93E1-F8C355728877 | 1      | TCN93     |
	When I call RemoveByRunId method with parameters:
		| Parameter | Value   |
		| runId     | <RunId> |
	And I call GetByRunId method with parameters:
		| Parameter | Value   |
		| runId     | <RunId> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
		| RunId                                | ExpectedReturnCount |
		| 1771D896-912C-42D3-8131-3740805D7772 | 0                   |
		| C499E915-3AFB-46B4-B8ED-35A68D551ED4 | 0                   |
		| 4AB41BB6-5CED-421F-93E1-F8C355728877 | 0                   |

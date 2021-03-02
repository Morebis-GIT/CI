@ManageDataStorage

Feature: ManageResultsFileDataStorage
	In order to manage results file
	As a user
	I want to store results file via ResultsFile repository

Background:
	Given there is a ResultFiles repository

Scenario: Check ResultFile
	Given there is result file '193215' for scenario '16FCC85E-FE88-490F-A965-D2170D248128'
	When I call Exists method with parameters:
         | Parameter  | Value                                |
         | scenarioId | 16FCC85E-FE88-490F-A965-D2170D248128 |
         | fileId     | 193215                               |
	Then the received result should contain the following values:
         | Property | Value |
         | Result   | True  |

Scenario: Insert ResultFile
	Given there is '131766' result file
	When I call Insert method with parameters:
         | Parameter  | Value                                |
         | scenarioId | 8462277A-39B0-4980-99AE-E9CF109946BB |
         | fileId     | 131766                               |
	And I call Exists method with parameters:
         | Parameter  | Value                                |
         | scenarioId | 8462277A-39B0-4980-99AE-E9CF109946BB |
         | fileId     | 131766                               |
	Then the received result should contain the following values:
         | Property | Value |
         | Result   | True  |

Scenario: Delete ResultFile
	Given there is result file '176254' for scenario '61D0A38A-7424-450F-AD61-4799F3F227ED'
	When I call Delete method with parameters:
         | Parameter | Value                                |
         | scenario  | 61D0A38A-7424-450F-AD61-4799F3F227ED |
         | fileId    | 176254                               |
	And I call Exists method with parameters:
         | Parameter  | Value                                |
         | scenarioId | 61D0A38A-7424-450F-AD61-4799F3F227ED |
         | fileId     | 176254                               |
	Then the received result should contain the following values:
         | Property | Value |
         | Result   | False |

Scenario Outline: Get ResultFile
	Given there is result file '<FileName>' for scenario '<ScenarioId>'
	When I call Get method with parameters:
         | Parameter  | Value        |
         | scenarioId | <ScenarioId> |
         | fileId     | <FileName>   |
         | compressed | <Compressed> |
	Then the result file '<ResultFile>' exists

	Examples:
	| FileName | ScenarioId                           | Compressed | ResultFile |
	| 368112   | 13BA19BF-984A-47EA-9103-00F36962DF22 | False      | 368112     |
	| 752943   | 9F9E9549-E249-403E-9F9C-2CDF40EE7B36 | True       | 752943.zip |

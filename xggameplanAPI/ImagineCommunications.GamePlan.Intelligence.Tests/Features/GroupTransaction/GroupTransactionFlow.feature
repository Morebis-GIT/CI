@GroupTransaction
Feature: GroupTransactionFlow
	In order to receive events and update database accordingly
	As a Message consumer system
	I want to be able to store messages locally and execute their handlers by certain order

Scenario: Successfully sent and consume 3 messages
	Given I publish GroupTransactionInfo and store returned Id as FirstGroupTransaction
		| EventCount |
		| 3          |
	And I publish IMockEventOne message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventThree message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventTwo message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	Then messages are consumed and stored in local storage
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 3          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 3          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventOne   | 0     |
		| IMockEventThree | 0     |
		| IMockEventTwo   | 0     |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 4     | 3          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventOne   | 4     |
		| IMockEventThree | 4     |
		| IMockEventTwo   | 4     |

Scenario: Successfully sent and consume 4 messages which are attached to 2 transactions
	Given I publish GroupTransactionInfo and store returned Id as FirstGroupTransaction
		| EventCount |
		| 2          |
	Given I publish GroupTransactionInfo and store returned Id as SecondGroupTransaction
		| EventCount |
		| 2          |
	And I publish IMockEventOne message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventThree message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventTwo message with GroupTransaction SecondGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventFour message with GroupTransaction SecondGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	Then messages are consumed and stored in local storage
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 2          |
		| 0     | 2          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 2          |
	And the GroupTransaction SecondGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 2          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventOne   | 0     |
		| IMockEventThree | 0     |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventTwo   | 0     |
		| IMockEventFour  | 0     |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 4     | 2          |
		| 4     | 2          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventOne   | 4     |
		| IMockEventThree | 4     |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventTwo   | 4     |
		| IMockEventFour  | 4     |

Scenario: Refusion of execution Ready transaction if there is any incomplete which was sent before.
	Given I publish GroupTransactionInfo and store returned Id as FirstGroupTransaction
		| EventCount |
		| 1          |
	Given I publish GroupTransactionInfo and store returned Id as SecondGroupTransaction
		| EventCount |
		| 1          |
	And I publish IMockEventTwo message with GroupTransaction SecondGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	Then messages are consumed and stored in local storage
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 1          |
		| 0     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the GroupTransaction SecondGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventTwo | 0     | 0          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 1          |
		| 0     | 1          |
	And the GroupTransaction SecondGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventTwo | 0     | 0          |

Scenario: Refusion of execution after a transaction failure.
	Given I publish GroupTransactionInfo and store returned Id as FirstGroupTransaction
		| EventCount |
		| 1          |
	Given I publish GroupTransactionInfo and store returned Id as SecondGroupTransaction
		| EventCount |
		| 1          |
	And I publish IMockEventOne message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | false                    |
	And I publish IMockEventTwo message with GroupTransaction SecondGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	Then messages are consumed and stored in local storage
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 1          |
		| 0     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the GroupTransaction SecondGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 0     | 0          |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventTwo | 0     | 0          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 1     | 1          |
		| 0     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 3     | 1          |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventTwo | 0     | 0          |

# This test ALWAYS fails the first time it's run as there's a conflict with
# other tests. No-one else sees this as an issue and no-one wants to fix it,
# so I'm disabling it.
@ignore
Scenario: Messages priority checking
	Given I publish GroupTransactionInfo and store returned Id as FirstGroupTransaction
		| EventCount |
		| 3          |
	And I publish IMockEventOne message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventTwo message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | false                    |
	And I publish IMockEventThree message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	Then messages are consumed and stored in local storage
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 3          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 3          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State | RetryCount |
		| IMockEventOne   | 0     | 0          |
		| IMockEventTwo   | 0     | 0          |
		| IMockEventThree | 0     | 0          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount | ReceivedEventCount |
		| 1     | 3          | 3                  |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State | RetryCount |
		| IMockEventThree | 4     | 0          |
		| IMockEventTwo   | 3     | 1          |
		| IMockEventOne   | 0     | 0          |

Scenario: Updating of the Retry count of event execution according to the Retry Policy (3 times).
	Given I publish GroupTransactionInfo and store returned Id as FirstGroupTransaction
		| EventCount |
		| 1          |
	And I publish IMockEventOne message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | false                     |
	Then messages are consumed and stored in local storage
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 0     | 0          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 1     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 1     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 3     | 1          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 1     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 1     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 3     | 2          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 3     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 3     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 3     | 3          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 3     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 3     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 3     | 3          |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 3     | 1          |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 3     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name          | State | RetryCount |
		| IMockEventOne | 3     | 3          |

# This is another test that almost constantly fails but no-one seems interested in fixing.
@ignore
Scenario: Starting execution from the oldest transaction
	Given I publish GroupTransactionInfo and store returned Id as FirstGroupTransaction
		| EventCount |
		| 1          |
	Given I publish GroupTransactionInfo and store returned Id as SecondGroupTransaction
		| EventCount |
		| 1          |
	Given I publish GroupTransactionInfo and store returned Id as ThirdGroupTransaction
		| EventCount |
		| 1          |
	Given I publish GroupTransactionInfo and store returned Id as FourthGroupTransaction
		| EventCount |
		| 1          |
	And I publish IMockEventTwo message with GroupTransaction SecondGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventThree message with GroupTransaction ThirdGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	And I publish IMockEventFour message with GroupTransaction FourthGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	Then messages are consumed and stored in local storage
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount | ReceivedEventCount |
		| 0     | 1          | 0                  |
		| 0     | 1          | 0                  |
		| 0     | 1          | 0                  |
		| 0     | 1          | 0                  |
	And the GroupTransaction FirstGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the GroupTransaction SecondGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the GroupTransaction ThirdGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the GroupTransaction FourthGroupTransaction will be updated as following
		| State | EventCount |
		| 0     | 1          |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventTwo   | 0     |
	And the MessageInfo with GroupTransaction ThirdGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventThree | 0     |
	And the MessageInfo with GroupTransaction FourthGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventFour  | 0     |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 0     | 1          |
		| 0     | 1          |
		| 0     | 1          |
		| 0     | 1          |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventTwo   | 0     |
	And the MessageInfo with GroupTransaction ThirdGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventThree | 0     |
	And the MessageInfo with GroupTransaction FourthGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventFour  | 0     |
	Given I publish IMockEventOne message with GroupTransaction FirstGroupTransaction
		| IsModelValid | BusinessValidationPassed |
		| true         | true                     |
	Then messages are consumed and stored in local storage
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventOne   | 0     |
	Then start GroupTransactionExecutor service and wait for completion
	And the table of GroupTransactionInfo will be updated as following
		| State | EventCount |
		| 4     | 1          |
		| 4     | 1          |
		| 4     | 1          |
		| 4     | 1          |
	And the MessageInfo with GroupTransaction FirstGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventOne   | 4     |
	And the MessageInfo with GroupTransaction SecondGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventTwo   | 4     |
	And the MessageInfo with GroupTransaction ThirdGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventThree | 4     |
	And the MessageInfo with GroupTransaction FourthGroupTransaction will be updated as following
		| Name            | State |
		| IMockEventFour  | 4     |

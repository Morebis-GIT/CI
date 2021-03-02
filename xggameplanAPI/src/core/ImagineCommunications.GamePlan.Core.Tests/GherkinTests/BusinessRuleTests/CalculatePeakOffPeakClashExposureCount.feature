@ClashExposureCount
Feature: Calculate peak/off peak clash exception count
	In order to manage spot placement during peak or off-peak times
	As an Airtime manager
	I want to specify rules for clash exception counts based on break sales area
	And/or time period
	And/or day of the week
	And calculate the resulting clash exposure count to use

Background:
	Given a new break
	And the break is in sales area 'NWS91'
	And the break is 3 minutes long
	And a new clash
	And the clash has default off peak exposure count of 10
	And the clash has default peak exposure count of 8
	And no peak time

Scenario: No differences are defined for the clash exposure counts and no peak time is defined
	Given a clash with no exposure count differences
	When I calculate the effective clash exposure count
	Then the result is the default off peak exposure count

Scenario: No differences are defined for the clash exposure counts and the break is in offpeak time
	Given a clash with no exposure count differences
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default off peak exposure count

Scenario: No differences are defined for the clash exposure counts and the break is in peak time
	Given a clash with no exposure count differences
	And the break starts at '25 Dec 2019 12:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference not matching the break's sales area is defined and the break is in offpeak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow | PeakExposureCount | OffPeakExposureCount |
		| QTQ93     |           |         |            | 2                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default off peak exposure count

Scenario: A difference not matching the break's sales area is defined and the break is in peak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow | PeakExposureCount | OffPeakExposureCount |
		| QTQ93     |           |         |            | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference matching the break's sales area is defined and the break is in offpeak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow | PeakExposureCount | OffPeakExposureCount |
		| NWS91     |           |         |            | 2                 | 3                    |
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference matching the break's sales area is defined and the break is in peak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow | PeakExposureCount | OffPeakExposureCount |
		| NWS91     |           |         |            | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 2

Scenario: A difference for all sales areas is before the break and the break is in offpeak time
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		|            | 25 Dec 2019 | 25 Dec 2019 | startTime=08:00:00, endTime=08:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default off peak exposure count

Scenario: A difference for specific sales areas is just before the start of a three day clash period
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '9 May 2020 05:59:59'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 10

Scenario: A difference for specific sales areas is at the start of a three day clash period
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '9 May 2020 06:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for specific sales areas is in the middle of a three day clash period
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '10 May 2020 00:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for specific sales areas is in the start of the middle of a three day clash period
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '10 May 2020 06:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for specific sales areas is at the end of a three day clash period
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '11 May 2020 00:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for specific sales areas is just before the end of a three day clash period
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '11 May 2020 05:59:59'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for specific sales areas is just after the end of a three day clash period
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '12 May 2020 06:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 10

Scenario: A difference for specific sales areas is at the start of a three day clash period with no end time
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                           | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | startTime=06:00:00, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '12 May 2020 05:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 10

Scenario: A difference for specific sales areas is at the start of a three day clash period with no start time
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | End Date    | Time And Dow                         | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | 11 May 2020 | endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '9 May 2020 4:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for specific sales areas is at the start of a three day clash period with no end date
	Given a clash with these clash exposure differences
		| Sales Area | Start Date  | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 09 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '15 May 2020 06:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for specific sales areas is at the start of a three day clash period with no start date
	Given a clash with these clash exposure differences
		| Sales Area | End Date    | Time And Dow                                             | Peak Exposure Count | Off Peak Exposure Count |
		| NWS91      | 11 May 2020 | startTime=06:00:00, endTime=05:59:59, daysOfWeek=1111111 | 2                   | 3                       |
	And the break starts at '01 May 2020 06:00:00'
	And a peak time running from '180000' to '220000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for all sales areas is before the break and the break is in peak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=08:00:00, endTime=08:59:59, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference for all sales areas contains the break and the break is in offpeak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=08:30:00, endTime=09:30:00, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for all sales areas contains the break and the break is in peak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=12:00:00, endTime=12:59:59, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 2

Scenario: A difference for all sales areas is after the break and the break is in offpeak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=20:00:00, endTime=20:59:59, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default off peak exposure count

Scenario: A difference for all sales areas is after the break and the break is in peak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=20:00:00, endTime=20:59:59, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference for all sales areas contains the break and the break is in offpeak time but the day is restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=08:30:00, endTime=09:30:00, daysOfWeek=0100000 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default off peak exposure count

Scenario: A difference for all sales areas contains the break and the break is in peak time but the day is restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=12:00:00, endTime=12:59:59, daysOfWeek=0100000 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference for all sales areas contains the break and the break is in offpeak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=08:30:00, endTime=09:30:00, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for all sales areas contains the break and the break is in peak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 | 25 Dec 2019 | startTime=12:00:00, endTime=12:59:59, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 2

Scenario: A difference for all sales areas contains the break day and the break is in offpeak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           |           |         | startTime=08:30:00, endTime=09:30:00, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for all sales areas contains the break day and the break is in peak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           |           |         | startTime=12:00:00, endTime=12:59:59, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 2

Scenario: A difference for all sales areas does not contains the break day and the break is in offpeak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           |           |         | startTime=08:30:00, endTime=09:30:00, daysOfWeek=1111111 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for a specific sales areas does not contains the break day and the break is in offpeak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		| NWS91     |           |         | startTime=08:30:00, endTime=09:30:00, daysOfWeek=1111111 | 4                 | 3                    |
	And the break starts at '25 Dec 2019 9:00:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 3

Scenario: A difference for all sales areas does not contains the break day and the break is in peak time
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           |           |         | startTime=12:00:00, endTime=12:59:59, daysOfWeek=0100000 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference for all sales areas without a start date contains the break and the break is in peak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           |           | 25 Dec 2019 | startTime=12:00:00, endTime=12:59:59, daysOfWeek=0010000 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 2

Scenario: A difference for all sales areas without a start date does not contain the break and the break is in peak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate     | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           |           | 24 Dec 2019 | startTime=12:00:00, endTime=12:59:59, daysOfWeek=0010000 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference for all sales areas without an end date contains the break and the break is in peak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 25 Dec 2019 |         | startTime=12:00:00, endTime=12:59:59, daysOfWeek=0010000 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the clash exposure count is 2

Scenario: A difference for all sales areas without an end date does not contain the break and the break is in peak time and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate   | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		|           | 26 Dec 2019 |         | startTime=12:00:00, endTime=12:59:59, daysOfWeek=0010000 | 2                 | 3                    |
	And the break starts at '25 Dec 2019 12:05:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default peak exposure count

Scenario: A difference for a specific sales areas does not contains the break day and the break is in offpeak time before the peak and the day is not restricted
	Given a clash with these clash exposure differences
		| SalesArea | StartDate | EndDate | TimeAndDow                                               | PeakExposureCount | OffPeakExposureCount |
		| NWS91     |           |         | startTime=08:30:00, endTime=09:30:00, daysOfWeek=1111111 | 4                 | 3                    |
	And the break starts at '25 Dec 2019 7:30:00'
	And a peak time running from '100000' to '140000'
	When I calculate the effective clash exposure count
	Then the result is the default off peak exposure count
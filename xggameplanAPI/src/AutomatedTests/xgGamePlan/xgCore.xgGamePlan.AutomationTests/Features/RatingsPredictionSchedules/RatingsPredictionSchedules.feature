@RatingsPredictionSchedules
Feature: Ratings Prediction Schedules API
	In order to check Ratings Prediction Schedules functionality
	As an API user
	I want to test available Ratings Prediction Schedules API endpoints

Background:
	Given I have a valid Demographic
	And I have a valid Sales Area

Scenario:  As an API User I want to search Ratings Prediction Schedules
	Given I have added 1 Ratings Prediction Schedule
	When I search for Ratings Prediction Schedules
	Then at least 1 Ratings Prediction Schedule is returned

Scenario:  As an API User I want to remove all Ratings Prediction Schedules
	Given I have added 1 Ratings Prediction Schedule
	When I delete all Ratings Prediction Schedules
	Then no Ratings Prediction Schedules are returned

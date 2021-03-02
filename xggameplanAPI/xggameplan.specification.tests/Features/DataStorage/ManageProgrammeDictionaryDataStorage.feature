@ManageDataStorage

Feature: ManageProgrammeDictionaryDataStorage
	In order to manage ProgrammeDictionaries
	As a Airtime manager
	I want to store ProgrammeDictionaries via ProgrammeDictionary repository

Background: 
	Given there is a ProgrammeDictionaries repository

Scenario: Get all ProgrammeDictionaries
	Given the following documents created:
		| Id  | Externalreference |
		| 418 | MYSU              |
		| 417 | TVSH              |
		| 386 | AUOPTA            |
		| 387 | AUOPTLN           |
	When I get all documents
	Then there should be 4 documents returned

Scenario: Get ProgrammeDictionary by id
	Given the following documents created:
		| Id  | Externalreference |
		| 418 | MYSU              |
		| 417 | TVSH              |
		| 386 | AUOPTA            |
		| 387 | AUOPTLN           |
	When I get document with id '418'
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property | Value |
		| Id       | 418   |

Scenario: Counting all ProgrammeDictionaries
	Given 3 documents created
	When I count the number of documents
	Then there should be 3 documents counted

Scenario Outline: Get ProgrammeDictionaries by external references
	Given the following documents created:
		| Id  | ExternalReference |
		| 418 | MYSU              |
		| 417 | TVSH              |
		| 386 | AUOPTA            |
		| 387 | AUOPTLN           |
	When I call FindByExternal method with parameters:
         | Parameter    | Value          |
         | externalRefs | <ExternalRefs> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples: 
	| ExternalRefs          | ExpectedReturnCount |
	| MYSU                  | 1                   |
	| TVSH, AUOPTA          | 2                   |
	| MYSU, AUOPTA, AUOPTLN | 3                   |
	| AFAF                  | 0                   |
	| DASF, RGDW, FGDF      | 0                   |
	| MYSU, AUOPTA, FFFF    | 2                   |
	| TVSH, FFFF            | 1                   |

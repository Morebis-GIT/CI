@ManageDataStorage

Feature: ManageUsersDataStorage
	In order to manage Users
	As an api user
	I want to store User via Users repository

Background:
	Given there is a Users repository

Scenario: Add new User
	When I create a document with values:
		| Id | Name  | Surname | Email            | ThemeName | Location      | Role  | TenantId | DefaultTimeZone |
		| 1  | Chris | Smith   | chris@smith.com  | White     | Europe/London | Admin | 1        | GMT +1          |
	And I get document with id 1
	Then there should be 1 documents returned

Scenario: Get all Users
	Given the following documents created:
		| Id | Name  | Surname | Email            | ThemeName | Location      | Role  | TenantId | DefaultTimeZone |
		| 1  | Chris | Smith   | chris@smith.com  | White     | Europe/London | Admin | 1        | GMT +1          |
		| 2  | James | Jonson  | james@jonson.com | Black     | Europe/Kyiv   | Admin | 2        | GMT +3          |
		| 3  | John  | Lennon  | john@lennon.com  | Yellow    | Heaven        | User  | 7        | GMT +0          |
	When I get all documents
	Then there should be 3 documents returned

Scenario: Get a non-existing User by id
	Given the following documents created:
		| Id | Name  | Surname | Email            | ThemeName | Location      | Role  | TenantId | DefaultTimeZone |
		| 1  | Chris | Smith   | chris@smith.com  | White     | Europe/London | Admin | 1        | GMT +1          |
		| 2  | James | Jonson  | james@jonson.com | Black     | Europe/Kyiv   | Admin | 2        | GMT +3          |
		| 3  | John  | Lennon  | john@lennon.com  | Yellow    | Heaven        | User  | 7        | GMT +0          |
	When I get document with id 45
	Then no documents should be returned

Scenario: Get an existing User by id
	Given the following documents created:
		| Id | Name  | Surname | Email            | ThemeName | Location      | Role  | TenantId | DefaultTimeZone |
		| 1  | Chris | Smith   | chris@smith.com  | White     | Europe/London | Admin | 1        | GMT +1          |
		| 2  | James | Jonson  | james@jonson.com | Black     | Europe/Kyiv   | Admin | 2        | GMT +3          |
		| 3  | John  | Lennon  | john@lennon.com  | Yellow    | Heaven        | User  | 7        | GMT +0          |
	When I get document with id 2
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property        | Value            |
		| Id              | 2                |
		| Name            | James            |
		| Surname         | Jonson           |
		| Email           | james@jonson.com |
		| ThemeName       | Black            |
		| Location        | Europe/Kyiv      |
		| Role            | Admin            |
		| TenantId        | 2                |
		| DefaultTimeZone | GMT +3           |

Scenario: Update a User
	Given the following documents created:
		| Id | Name  | Surname | Email            | ThemeName | Location      | Role  | TenantId | DefaultTimeZone |
		| 1  | Chris | Smith   | chris@smith.com  | White     | Europe/London | Admin | 1        | GMT +1          |
		| 2  | James | Jonson  | james@jonson.com | Black     | Europe/Kyiv   | Admin | 2        | GMT +3          |
		| 3  | John  | Lennon  | john@lennon.com  | Yellow    | Heaven        | User  | 7        | GMT +0          |
	When I get document with id 2
	And I update received document by values:
		| Property        | Value            |
		| Name            | Jimmy            |
		| Surname         | Jason            |
		| Email           | james@jonson.org |
		| ThemeName       | Default          |
		| Location        | Europe/Lviv      |
		| Role            | User             |
		| TenantId        | 9                |
		| DefaultTimeZone | GMT +5           |
	And I get document with id 2
	Then the received document should contain the following values:
		| Property        | Value            |
		| Id              | 2                |
		| Name            | Jimmy            |
		| Surname         | Jason            |
		| Email           | james@jonson.org |
		| ThemeName       | Default          |
		| Location        | Europe/Lviv      |
		| Role            | User             |
		| TenantId        | 9                |
		| DefaultTimeZone | GMT +5           |

Scenario: Get User by email
	When I create the following documents:
		| Id | Name  | Surname | Email            | ThemeName | Location      | Role  | TenantId | DefaultTimeZone |
		| 1  | Chris | Smith   | chris@smith.com  | White     | Europe/London | Admin | 1        | GMT +1          |
		| 2  | James | Jonson  | james@jonson.com | Black     | Black         | Admin | 2        | GMT +3          |
		| 3  | John  | Lennon  | john@lennon.com  | Yellow    | Heaven        | User  | 7        | GMT +0          |
	And I try to call GetByEmail method with parameters:
		| Parameter | Value           |
		| email     | test@server.com |
	Then no documents should be returned

	When I call GetByEmail method with parameters:
		| Parameter | Value            |
		| email     | james@jonson.com |
	Then there should be 1 documents returned
	And the received document should contain the following values:
		| Property        | Value            |
		| Id              | 2                |
		| Name            | James            |
		| Surname         | Jonson           |
		| Email           | james@jonson.com |
		| ThemeName       | Black            |
		| Location        | Black            |
		| Role            | Admin            |
		| TenantId        | 2                |
	 	| DefaultTimeZone | GMT +3           |

Scenario Outline: Get Users by user ids
	Given the following documents created:
		| Id | Name  | Surname | Email            |
		| 1  | Chris | Smith   | chris@smith.com  |
		| 2  | James | Jonson  | james@jonson.com |
		| 3  | John  | Lennon  | john@lennon.com  |
	When I call GetByIds method with parameters:
		| Parameter | Value |
		| ids       | <Ids> |
	Then there should be <ExpectedReturnCount> documents returned

	Examples:
	| Ids     | ExpectedReturnCount |
	| 3, 5    | 1                   |
	| 1, 2    | 2                   |
	| 1, 2, 3 | 3                   |
	| 5, 7    | 0                   |

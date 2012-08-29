Feature: Finding people
	In order to find known people
	As a curious fan boy
	I want to filter people base on their age

Background: 
	Given The people are
	| FirstName | LastName    | Age | Id                                     |
	| Scarlett  | Johansson   | 27  | {8c319634-935d-4681-adcc-02d5347fe6c4} |
	| Jessica   | Alba        | 31  | {32a84597-8c3d-44bc-a1a5-6538188e9d25} |
	| Penelope  | Cruz        | 38  | {5aa0eb59-3961-472f-b829-7d54ac8eeeef} |
	| Reese     | Witherspoon | 36  | {77fcd741-3839-4692-925f-a3a0eb19cf42} |
	| Charlize  | Theron      | 36  | {e37290f6-d376-44e2-944d-d0af13c1a75c} |
	| Mouloud   | Achour      | 31  | {18af541a-a5dc-41d2-af47-479b1c06e216} |

Scenario: Filter on age
	Given I have written a query against the provider
	And I have added a Age >= 36 where clause
	Given The people finder service filters on Age >= 36
	When I execute the query
	Then The service parameter should be Age IsGreaterThan 36
	And The result count should be 3
	And The result should validate the servicePredicate

Scenario: Filter on unsupported clause
	Given I have written a query against the provider
	And I have added a Not correctly formatted => not parsable where clause
	Given The people finder service filters on null
	When I execute the query
	Then An error should occur

Scenario Outline: Filter on a single criterion
	Given I have written a query against the provider
	And I have added a <predicate> where clause
	Given The people finder service filters on <servicePredicate>
	When I execute the query
	Then The service parameter should be <serviceParameter>
	And The result count should be <resultsCount>
	And The result should validate the servicePredicate
	Examples: 
	| predicate						| servicePredicate				| serviceParameter				| resultsCount	|
	|								|								|								| 6				|
	|								| false							|								| 0				|
	| false							|								|								| 0				|
	| false							| false							|								| 0				|
	| Age >= 36						| Age >= 36						| Age IsGreaterThan 36			| 3				|
	| Age > 36						| Age > 36						| Age IsStrictlyGreaterThan 36	| 1				|
	| Age <= 36						| Age <= 36						| Age IsLessThan 36				| 5				|
	| Age < 36						| Age < 36						| Age IsStrictlyLessThan 36		| 3				|
	| Age == 36						| Age == 36						| Age Equals 36					| 2				|
	| Age != 36						| Age != 36						| Age NotEquals 36				| 4				|
	| Age >= 36						|								| Age IsGreaterThan 36			| 6				|
	| Age > 36						|								| Age IsStrictlyGreaterThan 36	| 6				|
	| Age <= 36						|								| Age IsLessThan 36				| 6				|
	| Age < 36						|								| Age IsStrictlyLessThan 36		| 6				|
	| Age == 36						|								| Age Equals 36					| 6				|
	| Age != 36						|								| Age NotEquals 36				| 6				|
	| FirstName == Scarlett			| FirstName == Scarlett			| FirstName Equals Scarlett		| 1				|
	| FirstName != Scarlett			| FirstName != Scarlett			| FirstName NotEquals Scarlett	| 5				|
	| FirstName.StartsWith("Scar")	| FirstName.StartsWith("Scar")	| FirstName StartsWith Scar		| 1				|
	| FirstName.Contains("rl")		| FirstName.Contains("rl")		| FirstName Contains rl			| 2				|
	| FirstName == Scarlett			|								| FirstName Equals Scarlett		| 6				|
	| FirstName != Scarlett			|								| FirstName NotEquals Scarlett	| 6				|
	| FirstName.StartsWith("Scar")	|								| FirstName StartsWith Scar		| 6				|
	| FirstName.Contains("rl")		|								| FirstName Contains rl			| 6				|
	| LastName == Alba				| LastName == Alba				| LastName Equals Alba			| 1				|
	| LastName != Alba				| LastName != Alba				| LastName NotEquals Alba		| 5				|
	| LastName == Alba				|								| LastName Equals Alba			| 6				|
	| LastName != Alba				|								| LastName NotEquals Alba		| 6				|

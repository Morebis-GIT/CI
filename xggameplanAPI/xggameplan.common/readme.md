# readme for xggameplan.common

## unit tests for xggameplan.common

* Unit tests for this project must be written in the project 
  _ImagineCommunications.GamePlan.Domain.Tests_
* Within that project there is a folder called _Common.Tests_
* Within that folder, create a folder with a name matching the folder of the 
  class you're testing, with the suffix _.Tests_ For example
    * Common.Tests
      * Extension.Tests  <-- Tests for the _Extension_ classes
      * Utilities.Tests

## caveat

_ImagineCommunications.GamePlan.Domain.Tests_ uses NUnit so all tests within 
must also be NUnit. The project is scheduled for conversion to xUnit.

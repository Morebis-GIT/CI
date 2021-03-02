# Data transfer objects (DTOs)

These objects are *only* for transferring data from one place to another.

They do not and must not contain logic. The only methods are those required for 
an object to be complete, such as constructors, operators and Clone(), if 
required.

If you want to add functionality to any of these objects, think twice about it. 
If you still think it is required, move the object to the *Models* namespace and 
folder.

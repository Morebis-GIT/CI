# Types

These objects are self-contained basic *units* of information that can be used 
anywhere. They are small, typically one or two pieces of data at the most, that 
combine to give meaning, such as the types from the Framework: DateTime, String, 
Int and so on.

Typically they will be either enums (a single logical unit of integer values with 
a named representation), structs (single value objects that are small, fast and 
atomic) or classes (same definition as for structs but try to use structs rather
than classes for performance reasons).

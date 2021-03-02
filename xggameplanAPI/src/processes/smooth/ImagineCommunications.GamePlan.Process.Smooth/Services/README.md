# Services

These objects are contain functionality that manipulate or modify other objects.
These would typically be static methods as they should not hold any state. Because
they hold no state they should also be pure and thread-safe wherever possible.

Factory methods may also be found here, but sometimes a factory should be in the 
model the factory creates.

If a service method can be moved to a *model* then try to do so.

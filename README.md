# LifeInsuranceCalculator

I've written my version of the Life Insurance Calculator, with accompanying tests.
There is no Proxy set for the call to postcodes.io, so if you are behind the proxy, this will need to be set.

I've not writen the console application yet, but if you look on the physical folder where you have downloaded the solution to, I have started to put a website together that consumes this.
I've not referenced this in the solution as this is very much a work in progress and contains quite a bit of experimentation.

The Life Insurance Calcualtor itself is complete.
In the tests for the Calculator, you will see that I have mocked out the Address Finder, rather than use a real one.
This means I have complete control over what this dependancy does/how it behaves.
It also means I don't need an internet connection to test the calculator.

I've also not used any Dependency Injection Containers to resolve dependancies yet - Though you will notice that the Calculator depends on  the interface IAddressFinder, rather than a concrete implementation, and is therefore loosely coupled.
Wiring up DI would be quite simple.
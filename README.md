Isla <img src="https://ci.appveyor.com/api/projects/status/j05g1l2n73nmftq0">
====

Some nice logging and testing capabilities built for Castle Windsor and Moq

To date this includes a JSON logging interceptor.  When used with the log4net.ext.json layout log entries are entirely JSON.
This makes for easily searchable log files for example using the included JsonLogreader.

It also provides the MoqAutoMocker, my own take on automocking.
The automocked instance of the type you specify works with property injection as this is my preference.
However, time and again I found that I also needed standalone mocks in addition to automocked dependencies
of my class under test.  I didn't like that I would have to VerifyAll on my class under test and on any 
standalone mock instances/mock repository.
So using Castle dynamic proxy I added a MockRepository instance using via the mixin feature meaning I can use
my automocked instance to obtain any additional mocks I require and verify everything in one place.
Might not be for everyone but I like using it.

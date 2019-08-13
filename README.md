# WARNING
The API of this framework WILL CHANGE! It is not yet stable.

# Primoris.Universe.Stargen
This repository is a fork of Starform.NET.

The code of Primoris.Universe.Stargen has been heavily refactored to make it a Framework on which to build future version of Stargen, including new algorithms that can be added to it instead of changing the main code base. 

Starform.NET, as described by George Legeza, is an effort to port the Accrete/Starform/StarGen solar system generator to C# and .NET. Starform.NET is little more than a naive port of Jim Burrows' StarGen in its current form, but the ultimate goal is to create an accessible and useful tool for game developers, sci-fi writers, and anyone else who might have a need to quickly generate interesting planets and star systems.

## Differences with Starform.NET
The object model has been heavily changed. This includes class names, access modifiers for class members, refactorization of classes and namespaces and so on. All that work is to make it a Framework on which to build other softwares instead of a standalone program.

All the code has been ported to .NET Core 3. My goal is to have it always on the cutting edge of development. So when a new major .NET Core version gets release, Primoris.Universe.Stargen will be updated accordingly. This includes C# versions as well. 

Also the command line utility, the main frontend to actually use Stargen, has been switched to a PowerShell Core Cmdlet. Since PowerShell Core is compatible with all major Operating System, it makes it the best choice for a command line utility. 

## Just a Dash of Science (from Starform.NET)
The original inspiration of the Accrete program (at least as far as I can tell) is the 1970 paper *Formation of Planetary Systems by Aggregation: A Computer Simulation* by Stephen H. Dole (Stephen H. Dole, Icarus, 13 494-509). A variety of authors have implemented the original simulation described by Dole in that paper, and many of the versions available online have expanded on it as well. 

Of course, science has moved on in the intervening years and the model described by Dole is much less complex than our current understanding of how stellar systems are formed. What this means in practical terms is that Starform.NET's, and now Primoris.Universe.Stargen, output will hopefully be scientifically *plausible*, but far from scientifically *accurate*. The underlying model shouldn't be considered anything more than a convenient method of generating reasonable and interesting output.

A very thorough consolidation of all the versions of Accrete/Stargen/Starfrom to date since the 1960s can be found at https://github.com/zakski/accrete-starform-stargen


## Long Term Plans
The long term plans for Primoris.Universe.Stargen is to add more functionalities and algorithms to the Framework, making it up to date in terms of science. This will not be done by myself since I'm not an expert at all in the field. But I hope people will join. 

## Credits
A big thank you to George Legeza, the author of Starform.NET, who did the initial long and tedious work of translating plain old C/C++ to .NET. Without him I would have abandonned this effort long ago. With a good C# codebase to start with the rest was le lot more fun.

## Credits (from Starform.NET)
Aside from porting the code from C to C#, none of the work on the accretion simulation is mine. Starform.NET's core is a direct port of [Jim Burrows' StarGen](http://www.eldacur.com/~brons/NerdCorner/StarGen/StarGen.html). The original readme and credits from that version are included in this repository as StarGen-ReadMe.txt and StarGen-Credits.txt. The StarGen-Credits.txt file contains a more complete history of all of the authors who have contributed to Accrete/Starform/StarGen as well as a bibliography for the papers and books used to design it.

The planet sprites used in the output are Master484's planet sprite set from [OpenGameArt.org](http://opengameart.org/content/pixel-planets).

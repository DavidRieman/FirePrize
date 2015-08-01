FirePrize
=========

## ABORTING
While Firebase may be a nice choice for Java development, stability through FireSharp leaves a lot to be desired.
The first issues were with using Firebase lists; after pushing items to the list, a callback which is supposed to present the changed item fails to render the full item.
Having to reconstruct full object trees based solely on individual property changes and Firebase paths is just too much to take on.
To work around this, I moved to committing whole Collections, knowing this would kill "simultaneous editing of different parts of any list" from supported reliable feature sets.
Turned out the FireSharp callbacks would now give us full objects, but NEW full objects - so we needed to bring in a deep object comparison library to fix synchronizations.
After solving that, Firebase simply stopped letting us make changes to one of the collections altogether.
Absolutely no idea why, but it's clear this is still way more of an uphill battle than it should be.
Going to step back and start over with less targeted features, perhaps even straight in Excel.
Mainly, eliminating online synchronization and multiple editors will eliminate tons of complexity, at the expense of solving less of last year's prize drawing pains.

## Overview
Prize drawing software for Windows. This software is being built for a charity event, but could also be utilized by raffles.

## Goals
* Paperless process improvements for performing prize drawings, through virtual tickets.
* Simplify processing/handling of hundreds of prizes.
* Better participant experience.

## Completed Key Features

## TODO
* Multiple roaming clients operating against shared data, to distribute ticket sales, and facilitate prize discovery.
* Multiple prize pools the users can choose to apply their tickets to.
* Ability to move (or clone) tickets from one pool to another, to support complex drawing game rules.
* Automatic, randomly chosen winners for prize pools.
* Different display screens suitable for administration and for projected display to players.
* Sorting options by prize or by player.
* Look up results by pool, prize, or by player.

## Architecture
* Firebase will be utilized through the FireSharp library (https://github.com/ziyasal/FireSharp).
* ClickOnce installation through Github (with a strategy like this: https://github.com/BlythMeister/Gallifrey/wiki/Click-Once-Deployment-In-GitHub).

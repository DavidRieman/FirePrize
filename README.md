FirePrize
=========

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

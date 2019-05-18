# Ballance Package Manager

A ~~*awful*~~ package manager for Ballance.

## Progress

**Project-alice-in-wonderland** has been done because I have solved the major issues of deploying.

**Project-Sakura** will separate the bpm kernel and the user interface for multi-platform development. This is a hard work.

## Planned project structure

```
                                                   +-------------------+
    +-----------+                                  | BPMScriptDebugger |
    | BPMClover <---+                              +---------^---------+
    +-----------+   |                                        |
                    |                                        |
  +-------------+   |   +------------------------+      +----+-----+      +-----------+
  | BPMShamrock <-------+ BallancePackageManager <------+ ShareLib +------> BPMServer |
  +-------------+   |   +------------------------+      +----------+      +-----------+
                    |
+---------------+   |
| BPMCornflower <---+
+---------------+

```

* ShareLib: The core lib which define some important and shared values and methods.
* BPMServer: a .Net Core app. It distribute resources for each clients.
* BPMScriptDebugger: a .Net Core app. It can hele you to test and debug your package script.
* BallancePackageManager: The core of BPM client. It is under .Net Standard and can be loaded in any environment.
* BPMCornflower: A WPF app. It is served for Windows players and provides more fluent experience.
* BPMClover: A .Net Core console app, served for multi-platform using. 
* BPMShamrock: A .Net Core GUI app using Avalonia. Multi-platform app. **\(This project is postponed due to the CJK character render error.\)**

# Ballance Package Manager

A ~~*awful*~~ package manager for Ballance.

## Progress

**Project-alice-in-wonderland** has been done because I have solved the major issues of deploying.

**Project-Sakura** will separate the bpm kernel and the user interface for multi-platform development. This is a hard work.

## Planned project structure

```
+----------------------------+           +--------------+           +-------------+
|                            |  +----->  |              |  +------> |             |
|   BallancePackageManager   |           |   ShareLib   |           |  BPMServer  |
|                            |  <-----+  |              |  <------+ |             |
+-----+----------------------+           +--------------+           +-------------+
      |
      |
      |
      +--------------------------------+
      |                                |
      |                                |
      v                                v
+-----+-------------+         +--------+----------+
|                   |         |                   |
|    BPMShamrock    |         |     BPMClover     |
|                   |         |                   |
+-------------------+         +-------------------+

```

* ShareLib: The shared lib between server and client core.
* BPMServer: a .Net Core app.
* BallancePackageManager: The core of BPM client. It is under .Net Standard and can be loaded in any environment.
* BPMShamrock: A WPF app. It is under .Net Framework 4.0, served for Windows platform especially for Windows XP \(Because of the running environment limitation of Ballance\).
* BPMClover: A .Net Core console app, served for multi-platform using. 

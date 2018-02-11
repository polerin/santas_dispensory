# Santa's Catch
This is a child friendly Unity3D/HTC Vive game, initially a project from the 2017 Austin VR Jam, but significantly cleaned up and updated as learning project.  Free range, hand crafted, artisanal Art assets were graciously provided by the always amazing Penelope Nederlander and the equally talented Josh Johnson.  Additional thanks to the VRTK slack for technical assistance.  Game code is the result of Erin McCargar's finger wiggles and caffeine intake.

## Environment and build information
The project is being built in Unity 2017.1.1f1, but will probably work through all of 2017.\* and probably 2018.\*

Required plugins and versions:
 * VRTK 3.3.\* (Built on 3.3.0 alpha)
 * Zenject (Need Version info)
 * GameEventBus (need link to modified version)
 * SteamVR (Need Version info)

## General Architecture
Almost all class dependencies are injected using Zenject, and are configured in the `/Coordination/Installers/CatchSceneInstaller.cs` MonoInstaller.  This is a good place to start looking through the system.  This lets us get away from singletons, and towards a testable system, even though the project is currently untested. 

That said, a brief overview of the setup might be useful.  Code responsibilities are split up into two basic categories, coordination and object scripts.  There is a concerted effort to keep the object scripts focused on MonoBehavior derived classes satisfying a specific mechanical need in the scene.  This leaves much of the heavy lifting up to the coordination and management classes, most of which are plain objects.  

The root of the management and coordination structure is the GameManager.  This is responsible for listening for game start/stop signals, creating and holding a reference to the current Game, and providing access to said reference to all consuming objects.  Some of these responsibilities may be split off later because SRP, but for now it's simple enough.  Interestingly enough, however, the GameManager is actually not the most commonly injected object.

That title falls to the EventBus.  This is a GameEventBus object, which helps centralize game-wide event subscription and tracking.  It is important to note that while not every interaction between classes goes through the bus, most major game events (such as Game Start, Bin Scored, or Item Dispensed) have at least a notification sent out on the bus.  This object is injected into almost every management class, into some of the Object Script classes, and even into a few of the strategy implementation classes.  All available events are contained within the `/Coordination/Events` directory.

Speaking of strategies, the RoundManager relies fairly heavily on injeected lists of strategies to implement much of the actual control mechanisms.  Currently input (ControlSet), scoring (ScoringStrategy), and round status monitoring (RoundInspector) are all implemented in strategies, collected as lists in the CatchSceneInstaller, and injected into the RoundManager for use.  This leaves the RoundManager responsible for figuring out which of those strategies to use and activating them.

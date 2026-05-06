# 1.0.3
- Updated the water gravity script to fix a couple issues:
  - Enemies that spawned underwater would sometimes not be affected by water gravity
  - Enemies launched by Breaching Fin would float upward until they rose above the water
- Fixed Lemurian Eggs not spawning if Artifact of Devotion is enabled

# 1.0.2
- Added a config option to toggle EnemiesReturns' archer bugs (disabled by default)
- Simplified the code used to register Wetland Downpour and replace Wetland Aspect at appropriate times. Should hopefully function exactly the same as before
- If 'Post-Loop Variant' is set to false, Wetland Aspect and Wetland Downpour will now both have their weights set to half the normal value to prevent making Wetland variants more common than other stages
- README: Fixed a typo in the Simulacrum preview image's link causing it to not load
- Hallowed the map, Conceptual style

# 1.0.1
- Fixed the Simulacrum variant's scene def having "Should Include in Logbook" enabled (oops)
- Fixed the Simulacrum variant's DLC1 ADCCS not having a target DCCS file (oops 2)
- Fixed Solus Transporters having a selection weight of 0 and thus never appearing (oops 3). Also made them appear after 5 stage completions
- Added extra functionality to the water, based on the water mechanics in [Fogbound Lagoon](https://thunderstore.io/package/JaceDaDorito/FogboundLagoon/)
  - Gravity is now lower in the ponds
  - Music now gets muffled while the camera is submerged underwater (also added a config option to toggle this)

# 1.0.0
- Wetland Downpour can now appear in the Simulacrum!
  - Added a config option to toggle the Simulacrum variant
  - Added a config option to toggle Wetland Downpour, in case you only want the Simulacrum version for whatever reason
- Adjusted lighting and fog again (lol). Fog is a bit thicker, cliff wall material is darker, sun is less bright
  - The map should still be significantly brighter than it was on iniital release. Let me know if I made it too dark again
- Replaced grass texture with a new one that's slightly less visually noisy
- Mud texture no longer appears on the outer cliff walls
- Slightly lowered a pond to fix a tiny corner not connecting to the ground
- Added the artifact code tablet from Wetland Aspect (thanks to Viliger for providing the script used to add this!)
- Updated logbook screenshot, preview images and README to reflect changes I've made to the map over time
- I also intended to add a couple extra effects to the water (audio muffling in particular) for this update but I looked into it for like 20 minutes, learned that I could not just steal the script from [Fogbound Lagoon](https://thunderstore.io/package/JaceDaDorito/FogboundLagoon/) and have it magically work with the ponds in this map, and determined I am too stupid to figure it out. Sorry

# 0.2.3
- Added Simplified Chinese translation (thanks to JunJun5406 on github for submitting this!)
- Updated code for adding monsters from EnemiesReturns to not use deprecated config options

# 0.2.2
- Adjusted fog post-processing again to re-saturate the map's colors and bring them closer to the README preview screenshots. Also upped sun brightness again to compensate for this
- Adjusted most materials. The differences are probably borderline indistinguishable without before-after shots, but generally they should look slightly better
- Reduced the Lynx Totem texture's file size
- Fixed a map node that was missing a gate, causing interactables to sometimes spawn inside a wall

# 0.2.1
- Made a couple changes to increase the map's brightness in an effort to (hopefully meaningfully) help with visibility in dark areas, particularly when the screen is dimmed by the damage vignette or other effects
  - Adjusted fog colors ("fog start" color is brighter and more translucent)
  - Slightly increased sun intensity (1 -> 1.4)
- Halved the selection weight for larvae relative to other basic monsters
- Changed spawn distance for Lynx Scouts (Far -> Standard)
- Fixed the tents not having an assigned surface def
- Fixed monsters not using the geyser to chase players onto the cliff
- Fixed a map node that should've had a gate assigned, but did not

# 0.2.0
- Added monsters from Starstorm 2!
  - Followers, Wayfarers, and Archer Bugs can appear if the mod is enabled
  - Added config options to toggle each enemy
- Added monsters from EnemiesReturns!
  - Lynx Scouts, Lynx Totems, and Spitters can appear if the mod is enabled
  - Added config options to toggle each enemy
  - Lynx Shamans are disabled by default but can also appear if desired
  - Lynx interactables do not spawn. I wasn't sure how to add them. Sorry!!
- Monsters added by mods should respect their respective mod's settings (ex. if SS2 beta is off, Archer Bugs won't appear)
- Shrunk the "sea"grass in the giant lake
- Reversed change log version order

# 0.1.4
- Revised the stage's subtitle

# 0.1.3
- Attempted fix for language files not loading
- Attempted fix for README images not embedding

# 0.1.2
- Maybe actually fully fixed the readme this time haha

# 0.1.1
- Fixed readme formatting (hopefully)

# 0.1.0
- Initial Release




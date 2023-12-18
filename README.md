# AxieTest
This project is a simple turn-based simulation. This document describes some highlight logic design and modules communication. The project logic is hard driven by Event and Assets. You can use this [extension](https://github.com/ThinhHB/Unity-SimpleReferenceFinder) (included in project) to aid your logic tracing.

Assets used in project:
- Libraries: [Dotween](https://dotween.demigiant.com/documentation.php), [UniTask](https://github.com/Cysharp/UniTask)
- [Personal plugins](https://github.com/Aluminum18/unity-lib)
- Assets: [Hovl Studio RPG](https://assetstore.unity.com/packages/vfx/particles/spells/rpg-vfx-bundle-133704), [Sidearm Studios](https://assetstore.unity.com/packages/audio/sound-fx/ultimate-sound-fx-bundle-151756), [GUI - Casual Fantasy](https://assetstore.unity.com/packages/2d/gui/gui-casual-fantasy-265651)

## Content
- [Main Objects communication](#main-objects-communication)
- [Grid Map Coordinate](grid-map-coordinate)
- [Character Attack](character-attack)
- [Select and Expose An Axie Stats](select-and-expose-an-axie-stats)

## Main Objects Communication
Follow diagram describes communication of main objects

![image](https://github.com/Aluminum18/AxieTest/assets/14157400/5c2d2056-75ae-4820-83a1-d45b3ac1e9ba)

Start flow from "**SceneLoaded**" event. The "**CharacterSpawner**" starts spawning Axies as soon as the scene is loaded. For each successfully spawning, "**CharacterSpawner**" notifies this information to "**CharacterTracker**". "**CharacterSpawner**" requires 2 data objects to do its works. "**TeamFormation**" describes "**Attacker**" and "**Defender**" units coordinate. For this demo, the team is on rectangle formation, so the formation is presented by 2 end points of rectangle diagonal. "**MapSettings**" contains the information of map coordinate system: zero point, map size, cell size and cell offset (this setting helps align the axie to the center of a cell).

The "**CharacterTracker**" is responsible for recording Axies coordinate and status by listening events from Axies. "CharacterTracker" contains some services related to Axies such as: Find nearest target, check cell is movable, get an axie at coordinate. "CharacterTracker" writes its record to "TeamStats" which "Referee" uses to decide game result.

"**Referee**" is the decision maker of game process. Based on "**TeamStats**", it will decide how game continues at the end of a turn (EndTurnEvent).

"**TurnSimulator**" raises a sequence of events ("TurnEvents") when receiving "NewTurn" event. All axies listen "TurnEvents" to make action, they scan target in "StartedTurn" event and Move/Attack/Idle in "MiddleTurn".

## Grid Map Coordinate
Follow image describes the Grid coordinate used in game

![image](https://github.com/Aluminum18/AxieTest/assets/14157400/06991a3e-3a87-4391-902a-8ac0f00ec054)

All above settings are stored and configured in "MapSettings". "MapSettings" also contains converting between position and coordinate value services. The green dot of cell indicates cell pivot point in the world space. The yellow dot indicates the shifted cell pivot point when offset is applied. Offset helps aligning axie to a "look good" position in the world space.

## Character Attack
Axies use melee attack so they need to move forward to their target when attacking. Follow diagram describes the attack direction calculation of axies

![image](https://github.com/Aluminum18/AxieTest/assets/14157400/ae0663d6-6c9b-429f-94d1-73414b757683)

If 2 axies are in a direct engagement (they target each other), their engage position is calculated based on the middle point. Otherwise, axies will move forward to engage position of their target.

## Select and Expose an Axie Stats
Axies can be selected and exposed their stats without attaching Physics component to them.

![image](https://github.com/Aluminum18/AxieTest/assets/14157400/f7edd164-cbbb-4b8d-9623-3c03d5a25804)

When screen is clicked, using ray cast to identify the clicked point on the ground and write its position to "MapTouchPosition". "CharacterTracker" uses "MapSettings" to remap "MapTouchPosition" into map coordinate, then use coordinate to identify and have character at this coordinate write/expose their stats to "ExposedStats". "StatsPanel" then refresh its content based on updated data in "ExposedStats".


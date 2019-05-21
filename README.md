# Super_Soy_Boy
05/20/2019
Due to missing meta files the project cannot be opened in Unity, currently fixing this 'small' error

2D platformer that will lead to hair loss

This game is based on the tutorial in "Unity Games by Tutorial" distributed by raywenderlich.com
There are several major changes/additions

The game consists of 6 levels I made contained in json files in 'Assets/'; I warn you, they are difficult

There is also an editor if you want to make your own levels.  Open the Unity project and open the 'Level_template' Scene.  You can build your level there by adding prefabs to the existing 'Level' GameObject.  In the inspector you can find buttons to save your current level and to load a previous level for editing.  Make sure you only use prefabs.  Be careful saving and loading as there is no pre-prompt for saving or loading.

If you want to build the game, make sure to place the level json files in the Application.dataPath location for your platform

##Additions
1. Added several new hazards including falling rocks, false platforms, and swinging saws
2. Added a victory animation for SoyBoy complete with a refreshing noodle splash
3. New player controller that clings to wall more consistently
4. Jump sounds now play correctly; the sliding clip no longer plays over it
5. Added new Gameobjects for level design: floors, walls, ramps, and rocks
6. Added functionality to load previous levels for editing, using a custom editor
7. Added a level boundary object so the player will no longer fall forever




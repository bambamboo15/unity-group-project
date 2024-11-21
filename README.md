# Unity Group Project - CPSC 24500

Current game title: **The Great Snake Escape**

## Instructions 
To run this project, first `pull` this repository (or `clone` it if you are doing this for the first time). Then, open Unity Hub, browse to the project folder `unity-group-project/Unity Group Project`, and open it.

To commit to `README.md`, do the instructions above and `pull` the repository, make changes, and run `git commit -m "README" README.md` then `git push`.

## Role-specific instructions 
To **test the game**, you should `pull` the repository, build the project to a .exe, and test it (or play it in Unity).

To **design a map**, [to be determined]

For the **pixel art designer**, here are some of the 16x16 pixel art sprites that we need (make it kind of simple, similar to the ones that we have):
- Snake body sprite 
- Snake head sprite 
- Gold sprite 
- Exit sprite 
- Item sprites 
- Player sprite 
- Golden door sprite 
- Map-related sprites 

For the **sound effect searcher**, the following is maybe neeeded:
- Title screen music 
- Game music 
- Lose sound effect (or snake capture sound effect)
- Item collection (or usage) sound effects 
- Gold collection sound effect 
- Golden door opening sound effect 
- Exit reached sound effect (optional)
- Player movement sound effect (optional)
- Snake movement sound effect (optional)
- Health reduction sound effect (optional)
- Healing sound effect (optional)
- Menu sound effect (optional)

## Game Progress 
- **(9/24/24)** Made a grid 
- **(9/25/24)** Made a player 
- **(9/26/24)** Made player movement 
- **(10/01/24)** Revamped tilemaps and made walls to work 
- **(10/01/24)** Made snake design and some core snake tile functionality 
- **(10/02/24), map designer** Added a room to the map as a test 
- **(10/04/24)** Made snake loading functionality 
- **(10/06/24)** Made snake moving functionality 
- **(10/06/24)** Made snake chase player in an easy manner 
- **(10/09/24)** Simple title screen prototype with scene switching 
- **(10/14/24)** Action tile prototype with gold counter 
- **(10/15/24)** Fail screen when snake catches you 
- **(10/21/24)** Made gold work better 
- **(10/21/24)** Started working without internet 
- **(10/21/24)** Made snake faster when more gold collected 
- **(10/22/24)** Made a map select prototype (which does not work yet)
- **(10/22/24)** Laid out a golden door idea (which does not work yet)
- **(10/23/24), image designer** Made custom sprites 
- **(10/23/24)** Imported custom sprites and refactored filesystem 
- **(10/25/24)** Made some prototype sprites and verified player alignment 
- **(10/25/24)** Swapped custom floor and wall sprites 
- **(10/25/24)** Made a great-looking title screen prototype 
- **(10/25/24)** Made a few changes to design, such as canvas world space 
- **(10/27/24)** Completely revamped gold system, made it all on one tilemap 
- **(10/27/24)** Fixed some other unrelated canvas issues 
- **(10/28/24), image designer** Made more custom sprites 
- **(10/28/24)** Added a "void" tilemap where you and the snake should not fall to 
- **(10/29/24)** Renamed that tilemap to "NonBackground" and made it work on player 
- **(10/29/24)** Added a "You Won!" screen prototype that does not work 
- **(10/29/24)** Made that and the fail screen look extra detailed 
- **(10/29/24)** Adjusted some details for the menu screen 
- **(10/30/24)** Added player health feature, and made no health equate to failure 
- **(10/30/24)** Made the map look a little better 
- **(10/30/24)** Added an "exit" tilemap that does not work 
- **(10/31/24)** Did lots of map experimentation 
- **(11/01/24)** Complete map overhaul, added URP and post-processing 
- **(11/01/24)** Decided on a more arcade-style map 
- **(11/01/24), undocumented start** We started really working on the project 
- **(11/08/24), undocumented end** More than half-way done with the presentation 
- **(11/08/24)** Made gold trigger its sound effect when collected 
- **(11/13/24), undocumented end** Two days after the demo 
- **(11/13/24)** Made fundamental item sprite structure 
- **(11/13/24), image designer** Designed lots of pixel art sprites 
- **(11/13/24)** Made gold and item functionality much better 
- **(11/17/24), undocumented end**
- **(11/17/24)** Made the first working prototype of the speedup item!
- **(11/20/24), undocumented end**
- **(11/20/24), image designer** Made inventory prototype sprite 
- **(11/20/24)** Transformed prototype inventory design into a more working one 
- **(11/20/24), at most** Made random item spawners with probabilities 
- **(11/20/24), at most** Made item collection really work 
- **(11/21/24)** Made inventory functional 
- **(11/21/24)** Made items usable from inventory, each with their own different functions...
- **(11/24/24)** ...and gave us a massive understanding of interfaces and abstract classes 
- **(OBJECTIVE)** Add mobile support 
- **(OBJECTIVE)** Export the first version of the game on Google Drive (after building it)

![alt text](./image.png)
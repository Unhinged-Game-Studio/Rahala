
WHEN adding enemy ship to the game:
1. Make sure the ship's collider is above the water not inside the plane.
2. Make sure to reference the player rigid body in the script of the ship.
3. The Enemy cannon ball has a 'enemyball' tag, make sure to add a 'OnCollisionEnter'
   for the player ship which will be the damage taken, take references from
   `Detection.cs` line:96 to line:111. Or reference 'Friendly shop.cs'.
4. Make sure the friendly/player cannonball has the tag 'friendlycannnonball'.
5. Make sure that the land with collisions has a layer called 'Land'

WHEN adding a bandit into the game:
1. Make sure bandit is on land
2. Make sure to add player's reference
3. To add waypoints make an empty game object and position it on the map, 
   you would need atleast 2 to work properly. It goes in order so positioning them
   in a way that makes sense. Waypoints should be above land, preferably on level with
   bandit. 
4. Change the model whenever it is available to the actual model (Within the prefab)


GENERAL:
Play around with the values like 'wiggleMagnitude' for the size of the circle it will
cruise around.
Play around the the detection radiuses and sideFireDistance (for ship) depending on where you place ships.

Reminder:
Player and Enemy will have 2 separate cannonball prefabs.
Please add particle effect for player shooting cannons.

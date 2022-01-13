# WIMHome

## Description

This is a VR Application about World in Miniature Home Decoration for Oculus Quest. It helps users to manipulate furnitures in one room or several rooms, in an easy and convenient way. To achieve this goal, we combined techniques of Bubble Cursor, World In Miniature and Avartar Manipulation.

## Project Video

Link: https://youtu.be/mz_4HhC8xgQ

[![df816dd3193e300a91b9ef1f959efb6](https://user-images.githubusercontent.com/35856355/149073366-fca1c883-6e82-487d-81d1-29c0d02acbec.png)](https://youtu.be/mz_4HhC8xgQ)

## Instruction

### How to Run

You are very welcome to download the zip file of this repository and install the FinalVersion.apk file in your Quest using the SideQuest application.

### User Instructions

  #### Left Controller

  ***Left controller always manipulate the World in Miniature board (reference)***

  1. Move the thumbstick left (right) could rotate the WIM clockwise (counter-clockwise)
  2. Move the thumbstick up (down) could uniformly scale the WIM up (down).
  3. When trigger is pressed, move the thumbstick to the left or right could change the model of room shown in WIM
  4. Press "x" could activate/inactivate the "warehouse" of furnitures where you can add new furnitures to the rooms.

  #### Right Controller

  ***Right controller always manipulate the furnitures and camera***

  1. When the right controller goes into the bounding box of WIM, it will turn into bubble cursor which could only select furnitures from the WIM.
  2. When the right controller leaves the bounding box of WIM but inside the virtual room, it will turn into laser pointer which could interact with the objects in the virtual room.
  3. Press and hold grasp button to grasp the object selected. (Either covered by the bubble cursor or pointed by the laser pointer)
  4. Move the thumbstick up (down) could scale the furniture you are grasping in x and z axis based on the direction you are facing.
  5. Move the thumbstick left (right) could rotate the furniture you are grasping clockwise (counter-clockwise)
  6. Select and press "B" button for 0.5 seconds to delete the furniture.

  #### Some Other Hints

  1. Camera avator could be grasped, rotated and translated in the similar way as other furnitures.
  2. When you are grabbing the furniture or camera avator, there will be indicator showing whether they are currently on a valid position (indicator turns green).
  3. If you place an object on the invalid position (indicator turns red), it will return to its original state (position, rotation and scale). If the furniture comes from the "warehouse", the furniture will be discarded.
  4. You can grasp a furniture or camera in one room of WIM, use left controller to change the room in WIM, and release it in another room.

## References

***Models:***
1. https://quaternius.com/ (furnitures)
2. https://assetstore.unity.com/packages/tools/particles-effects/arrow-waypointer-22642 (waypoints)

***Teatures:***

1. https://assetstore.unity.com/packages/2d/textures-materials/sky/10-skyboxes-pack-day-night-32236#description
2. https://assetstore.unity.com/packages/2d/textures-materials/handpainted-grass-ground-textures-187634
3. https://assetstore.unity.com/packages/2d/textures-materials/wood/wood-pattern-material-170794#description
4. https://assetstore.unity.com/packages/2d/textures-materials/wood/wooden-floor-materials-150564


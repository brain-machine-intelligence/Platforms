# Platforms
This repository contains various environments for testing human cognitive models. Each environment is designed to test the developmental cognitive skills (Intuitive physics, Physical structure learning, self-play, etc.)


# 2020-11-27
We share the four Unity environments. In this version, we don't support an integrated interface.

## requirements (Library + Dataset)
**Unity 2018.3.0f2** (only tested on 3.0f2)

**Unity ml-agent** : The project requires a Unity-mlagents package which accessible from the official link (https://github.com/Unity-Technologies/ml-agents). You should put Ml-agents directory in the project Asset folder. 

**YCB objects**    : You can download the ycb models from the official website (https://www.ycbbenchmarks.com/). The prefab models should be placed in ./Asset/Prefabs/objects.


## Recognition memory
Recognition_memory/Assets/Scenes/SampleScene.unity

## Intuitive physics
Assets/Robotic_industrial_dynamic_arm/Scenes/Robot_Arm_prior_collect.unity

## Spatial memory
Assets/Robotic_industrial_dynamic_arm/Scenes/SpatialMemory.unity

## Uncertain pong
Uncertain_Pong/Assets/Scenes/Mode2.unity



# Unity-Survival-Game
# 3D Survival Game (Unity)

## Overview

This project is a small 3D survival game I developed in Unity as part of my coursework. The goal was to build a functional survival system that combines player needs, AI behavior, and interactive world elements into a single experience.

The player must manage hunger, thirst, and health while exploring the environment and interacting with objects like campfires and animals.

---

## What I Built

### Player Systems

* Implemented hunger, thirst, and health mechanics that update in real time
* Added environmental interaction (detecting water to restore thirst)
* Designed simple survival balancing so the player has to constantly manage resources

### AI (Wolf Enemy)

* Created a wolf AI using Unity’s NavMesh system
* Behavior includes:

  * Random wandering
  * Detecting the player within a range
  * Playing a howl animation before chasing
  * Attacking with cooldown logic
* Worked with Animator blend trees to make movement and attack transitions smooth

### Building & Interaction System

* Implemented a basic building system (campfire placement)
* Added placement preview before confirming position
* Campfire can be used for cooking food

### Cooking Mechanic

* Raw food can be cooked over time using the campfire
* Introduced a timer based system to convert items (raw to cooked)

### Audio & Feedback

* Added 3D sound effects (footsteps, wolf howl, fire)
* Used distance based audio for better immersion

---

## What I Learned

* How to use NavMeshAgents for AI movement and behavior
* Working with Animator blend trees and animation timing
* Structuring gameplay systems (player stats, interactions, world logic)
* Debugging complex behaviors (AI states, animations not looping, detection issues)
* Managing project structure and integrating multiple systems together

---

## Challenges

* Syncing animation speed with actual AI movement
* Fixing cases where the NavMeshAgent would fail to move correctly
* Making sure systems interact properly (cooking only works under the right conditions)
* Handling transitions between AI states without bugs or jitter

---

## Screenshots

<img width="1919" height="1083" alt="Screenshot (106)" src="https://github.com/user-attachments/assets/5307ffd1-3a43-4317-9439-14a0dc43d76f" />
<img width="1922" height="1083" alt="Screenshot (105)" src="https://github.com/user-attachments/assets/5a73c6a3-afc3-4877-91d1-f078311687ee" />
<img width="1921" height="1080" alt="Screenshot (101)" src="https://github.com/user-attachments/assets/2d2d91b9-8df0-4543-89f6-af4ffc96123f" />


---

## Notes

This project focuses more on systems and mechanics rather than visuals. The main goal was to build a working survival gameplay loop and understand how different systems connect together.

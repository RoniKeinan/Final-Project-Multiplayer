# Multiplayer Escape Room Project

**Final Project for Software Systems Development**

## 📋 Description

A real-time multiplayer digital escape room built with Unity, Photon PUN, and Convai AI. Players join a lobby, select or create rooms, navigate through interactive puzzles, and receive dynamic hints from an AI guide. Results are saved to Firebase for leaderboards.

## 🚀 Features

- **Lobby & Room Management**: Create or join rooms, manage up to configurable player count.
- **Multiplayer Synchronization**: Real-time player movement and state sync via Photon PUN.
- **AI-Powered Hints**: Convai NPC provides context-aware hints and dialogue.
- **Player Movement**: Smooth walking, jumping, and animations.
- **Persistent Leaderboard**: Match results stored and retrieved from Firebase Realtime Database.

## ⚙️ Architecture Overview

The game consists of two main Unity scenes:

1. **MainMenu Scene**
   - Player enters name, creates or joins rooms, and selects characters.
2. **Game Scene**
   - Players move in the environment, solve puzzles, and interact with AI.

## 🛠️ Core Classes & Responsibilities

| Class                  | Responsibility                                | Key Method                           |
| ---------------------- | --------------------------------------------- | ------------------------------------ |
| **RoomList.cs**        | Manages lobby UI and room creation/join logic | `OnCreateRoomButtonClicked()`        |
| **PhotonSetup.cs**     | Connects to Photon Cloud and spawns avatars   | `InitGame()`                         |
| **BasicMovement.cs**   | Handles player input, movement & animations   | `HandleMovement()`                   |
| **ConvaiNPC.cs**       | Interfaces with Convai API for dynamic hints  | `StartListening()`                   |
| **FirebaseManager.cs** | Saves and fetches match results               | `SaveScore(playerName, score, room)` |
| **MatchData.cs**       | Data model for storing player scores          | Constructor & JSON serialization     |

## 📸 ScreenShots
(Images/Screenshot%202025-07-19%20224614.png)
(Images/Screenshot%202025-07-29%20103035.png)
(Images/Screenshot%202025-07-29%20103712.png)

# POP_KHUTON

Unity project that visualizes a grid-based farm and updates crops from a backend detection stream (SSE). Includes login/registration UI, plant detail view, and harvesting logic.

## Features
- Grid farm generation based on farm mesh size and cell size.
- Real-time plant add/update/harvest from SSE detection data.
- Plant detail UI with camera zoom and sprite info.
- Login/sign-up panels and basic auth integration.

## Requirements
- Unity 6000.0.48f1
- DOTween / DOTweenPro
- cakeslice Outline Effect
- Newtonsoft.Json
- TextMeshPro

## Quick start
1. Open the project at `POP_KHUTON/POP_KHUTON` in Unity Hub.
2. Open `Assets/Scenes/SampleScene.unity`.
3. Press Play.

## Backend integration (optional)
- SSE stream: `http://localhost:8000/detection_stream/{user_id}`
- User data: `http://localhost:8000/user_data/`
- Auth server: `http://localhost:8001/` (register/login/update)
- Configure in the scene:
  - `SSEObjectReceiver.mainServerBaseAddress` and `SSEObjectReceiver.authServerBaseAddress`
  - `SSEObjectReceiver.sseTargetUserId` (or call `LoginUserAsync`)
- Detection payload fields:
  - `sector_row`, `sector_col`
  - `Level`: `v1` to `v4`
  - `type`: `cabbage` | `tomato` | `eggplant`

## Controls
- Right mouse drag: move camera.
- Left click on plant: open detail view.
- Esc: close detail view.

## Project structure
- `Assets/Scripts01/Farm`: farm grid and plant list management.
- `Assets/Scripts01/Plants`: plant data, controller, hover interactions.
- `Assets/Scripts01/UI`: login/sign-up panels and UI transitions.
- `Assets/Scripts01/SSEObjectReceiver.cs`: SSE + REST client for backend.
- `Assets/WorldSingleton.cs`: shared data, sprites, harvested plant export.

## Notes
- Build settings currently reference `Assets/Scenes/SampleScene.unity`.
- Plant sprites are loaded from `Assets/Resources/Crops`.

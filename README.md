# Super Unity 2D (Super Mario)

A full **Super Mario 2D** platformer project in **Unity 6**, named **"Super Unity 2D"**!

---

## 🎮 How to Play / Run

You have two instant ways to play and develop:

### 1. Open Directly in Unity Editor
- Double-click **`Open_In_Unity.bat`** (located in `D:\game drive\` or `D:\game drive\super unity in 2d\`).
- Unity 6 will open the project directly to `SuperUnityScene`.
- Hit **Play (▶️)** at the top of the Unity Editor!
- *Menu Shortcut:* You can also click **Tools -> Super Unity -> Build Full Mario Game Scene** in the top Unity menu bar to regenerate the full World 1-1 layout at any time.

### 2. Instant Playable Web Preview
- Double-click **`Play_Super_Mario.bat`** in `D:\game drive\` or `D:\game drive\super unity in 2d\`.
- Your browser will open the high-performance retro HTML5 Canvas version with full 8-bit sound effects, music, and physics!

---

## 🕹️ Controls

| Action | Primary Key | Alternative Key |
| :--- | :--- | :--- |
| **Move Left / Right** | Left / Right Arrows | A / D |
| **Jump** (Variable height) | Spacebar | W / Up Arrow |
| **Run / Sprint (Dash)** | Left Shift | Right Shift / X |
| **Crouch** (Super / Fire) | Down Arrow | S |
| **Shoot Fireball** (Fire Mario) | F | Left Ctrl / C |
| **Restart Level** | R | |
| **Mute / Unmute Audio** | M | |

---

## 🌟 Game Features

### 1. Mario Movement & States
- **Small Mario**: 1-tile height, agile jumping, stomping enemies.
- **Super Mario**: 2-tile height, can break brick blocks from below.
- **Fire Mario**: Can shoot bouncing fireballs to defeat enemies from a distance.
- **Classic Physics**: Acceleration, deceleration, momentum skid animation, and variable jump height (short hops on tap, high jumps on hold).
- **Damage & Invulnerability**: Shrinking down with blinking effect when hit; dramatic death jump sequence when falling or losing all health.

### 2. Enemies
- **Goomba**: Classic walking patrol; gets squished flat when stomped from above.
- **Koopa Troopa**: Retreats into its shell when stomped. Mario can kick the shell to slide at high speed, wiping out enemies and bouncing off pipes!
- **Piranha Plant**: Emerges periodically from green warp pipes (hides when Mario is standing close to the pipe).

### 3. Interactive Blocks & Collectibles
- **Question (`?`) Blocks**: Animated pulsing blocks that bounce up when hit from underneath, awarding coins or power-up items (Super Mushroom / Fire Flower).
- **Brick Blocks**: Bumps up when hit by Small Mario; shatters into 4 rotating debris pieces when hit by Super or Fire Mario.
- **Spinning Coins & 1-Up System**: Collecting 100 coins awards an extra life.

### 4. Goal & World 1-1 Layout
- Authentic **World 1-1** progression: ground pits, pipe obstacles, staircase pyramids.
- **Flagpole**: Sliding down the flagpole triggers the flag to lower, awards height-based bonus points, and walks Mario safely into the castle with stage clear music!

### 5. Audio & Visuals
- Built-in procedural 8-bit NES synthesizer: Plays Jump, Stomp, Coin, Power-up, Power-down, Shell kick, Brick break, Flagpole slide, Stage Clear, and Overworld Theme music automatically!
- 42 retro pixel-art sprites with Point filtering for clean, crisp pixel graphics.

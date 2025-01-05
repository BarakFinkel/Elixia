# Elixia Game Project Documentation

---

## 1. Characteristics of Game Objects

### **Main Features**
- **Player**:
    - **Stats**:
        - Offensive: Strength, Agility, Intelligence, Vitality, Damage, Crit Chance, Crit Power.
        - Defensive: Max HP, Armor, Evasion, Magic Resist.
        - Magic: Fire, Ice, Lightning damage, with associated effects (Ignite, Chill, Shock).
    - **Special Skills**: Running, jumping, dashing, and attacking, with customization based on stats.
- **Skeletons**:
    - Role: Melee enemies with simple AI.
    - Stats: Damage, Max HP, Armor, Speed (slower than player).
- **Planned Enemies**:
    - **Slime**: Resistant to physical damage, weak to fire, high HP.
    - **Archer**: Long-range attacks, low HP, relies on agility to evade.

---

## 2. Balancing and Numerical Features

### **Initial Example Values**:
- **Player**:
    - Strength: 10 → Base Damage: 15, Crit Power: 160%.
    - Agility: 8 → Evasion: 8%, Crit Chance: 8%.
    - Vitality: 12 → Max HP: 36.
    - Intelligence: 6 → Magic Resist: 18.
- **Skeleton**:
    - Damage: 12, Max HP: 50, Armor: 5, Speed: 3.

Balancing will be refined through testing after all features are developed. The aim is a Souls-like "hard but fair" experience.

---

## 3. Placement of Key Items

### **Item Placement Plan**
- Items will encourage exploration and risk-taking:
    - **Health Potion**: Near the starting area to teach healing mechanics.
    - **Fire Sword**: Hidden in a cave guarded by skeletons, effective against slimes.
    - **Amulet of Agility**: On a cliff requiring precise platforming.

### **Start Location Map Sketch**


---

## 4. Behaviors of Key Objects and Characters

- **Player**: Fully controlled by the player in real-time, stats and skills affect performance.
- **Skeletons**:
    - Patrol designated areas.
    - Engage in combat when spotting the player, retreat if heavily damaged.
- **Planned Enemy Behaviors**:
    - **Slime**: Slowly moves toward the player, ignores armor, weak to fire.
    - **Archer**: Attacks from a distance, retreats if approached.
- **Emergent Behavior**:
    - Dynamic combat scenarios arise when multiple enemies interact with the player simultaneously.

---

## 5. Economic System

- Players collect souls from defeated enemies.
    - Souls can be spent on upgrading stats or unlocking skills.
- Items will be found in the world but may require specific stats to equip.

### **Example Costs**
- Strength Upgrade: 100 souls (scaling upwards with levels).
- Skills: 300–500 souls.
- Items: Found, not purchased, but require minimum stats to use.

---

## 6. Game Information and Player Perspective

### **Information Displayed**
- Health bar.
- Stamina bar.
- Current stats.
- Collected souls.

### **Perspective**
- 2D side-scrolling view with a simple, clear interface inspired by Souls-like games.

---

## 7. Control System

- **Real-Time Combat**:
    - Players directly control movement, combat, and skill usage.
- **Justification**:
    - Aligns with the challenging, engaging nature of Souls-like games.

---

## 8. Player Strategies

- Players must:
    - Learn enemy attack patterns.
    - Use skills effectively.
    - Explore to find items and upgrades.

### **Dominant and Dominated Strategies**
- **Dominant**: Using ranged magic against slow melee enemies.
- **Dominated**: Engaging in prolonged melee against magic-resistant enemies.

---
# PetCarrier

[GitHub Repository](https://github.com/Vonny1412/Valheim.PetCarrier)

PetCarrier allows you to **pack tamed creatures into portable crates**, carry them in your inventory, and release them somewhere else.

Filled crates preserve creature data and display an image of their occupant, so you can tell which creature is inside.

---

## Features

* Pack tamed creatures into portable crates
* Preserve creature data, including level, stars, and taming status
* Carry a filled crate in both hands and choose where to release its occupant
* Creature sprites on inventory icons, dropped crates, and equipped crates
* Purchase empty crates from Haldor
* Configurable weight, empty-crate stacking, price, portal access, and cross-world restrictions
* English and German localization

---

## How to Use

### Getting a Crate

Purchase empty pet crates from **Haldor**.

### Packing a Creature

1. Put an empty pet crate on your hotbar.
2. Aim at a tamed creature.
3. Use the crate from your hotbar.

The empty crate is consumed and a filled crate containing the creature is created.

Creatures cannot be packed while alerted, aggravated, or pregnant.

### Carrying and Releasing a Creature

A filled crate can be carried in your inventory or equipped in both hands.

Equip the crate to activate its placement marker. Aim at a suitable location and **left-click** to release the creature. The filled crate is consumed when the creature is released.

Unequipping or sheathing a filled crate puts it away completely instead of displaying it on your back or waist.

### Creature Sprites

Filled crates display an image of their occupant on the **inventory icon**, the **sides of a dropped crate**, and the **equipped crate**.

If the creature cannot be identified, an unknown-creature image is shown. If its prefab is unavailable, an unavailable-creature image is shown instead.

---

## Configuration

PetCarrier provides the following settings in the `Common` section of its BepInEx configuration file:

| Option | Default | Description |
|---|---|---|
| `BaseWeight` | `10` | Base weight of a crate, excluding the occupant |
| `WeightMassFactor` | `1` | Multiplier applied to the creature's mass when calculating filled-crate weight |
| `MaxStackSize` | `10` | Maximum stack size of empty crates |
| `Teleportable` | `true` | Allows filled crates through portals |
| `Price` | `100` | Purchase price of an empty crate at Haldor |
| `LockCreaturesToWorld` | `false` | Restricts captured creatures to their original world |

Filled-crate weight is calculated as `BaseWeight + (CreatureMass × WeightMassFactor)`. Empty crates are always teleportable.

When `LockCreaturesToWorld` is enabled, a creature cannot be released in another world. World restrictions are stored with the crate, so changing the setting does not necessarily remove a restriction already applied when the creature was packed.

---

## Installation

1. Install **BepInEx** and **Jötunn** for Valheim.
2. Place PetCarrier in your `BepInEx/plugins` directory.
3. Launch the game.

**Jötunn is a required dependency.** PetCarrier uses it to register its custom crate prefabs and support their integration in multiplayer.

---

## Translations

PetCarrier currently supports **English** and **German**.

---

## Multiplayer

**PetCarrier has been used in singleplayer without any known issues.**

Unfortunately, I currently have no opportunity to test it in multiplayer. The mod includes ownership checks and network synchronization for packing creatures and displaying equipped-crate sprites, so multiplayer is expected to work, but **this has not yet been verified in a multiplayer session**.

PetCarrier and its dependency **Jötunn** must be installed on the server and all connecting clients. Jötunn supports the registration and network availability of the custom crate prefabs; PetCarrier handles the creature data and equipped-crate sprite synchronization.

Multiplayer feedback and bug reports are especially welcome.

---

## Compatibility

PetCarrier is intended to support vanilla tameable creatures as well as compatible creatures from other mods.

A creature's prefab must be available to release it. If you remove a mod that supplies a creature stored in a crate, that crate cannot release the creature until the prefab becomes available again.

---

## License

MIT License

---

*Created with ♥️ — and AI assistance*

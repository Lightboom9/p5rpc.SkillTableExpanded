# Skill Table Expanded

Skill Table Expanded is a Reloaded-II mod that expands the SKILL.TBL, allowing for more custom skills and technicals to be added.

## What this mod actually does?

It expands the available skill slots from 1056 to 2500, and also allows to make skills with id of 800 or above active.

Additionally, it expands the amount of technical combos from 17 to 50, allowing addition of new technicals.

## Usage

### Provided files

Inside the mod's folder, you will find the `ForModders.zip` archive. Files inside that archive will be refered to as "provided files".

### Installation

1. Download the mod from gamebanana.
2. Add a dependency on this mod to your mod (`p5rpc.SkillTableExpanded`).
3. In your mod's root folder (where `ModConfig.json` is located), create a new folder named `ExpandedSkillTable`.
4. Copy `SKILL_EXPANDED.TBL` from the provided files into that new folder.

Congratulations! It should work now. Of course, now you will have to edit the `SKILL_EXPANDED.TBL`.

### Editing expanded skill table

To edit the `SKILL_EXPANDED.TBL`, you will have to use the 010 Editor. Use `p5r_tbl_expanded.bt` template from the provided files.

The template is basically the same as for `SKILL.TBL`, except the first segment is version, and the last (traits) isn't present.

Do *not* edit the version segment on your own; it's used to distinguish different version by table parser.

Besides that, just edit the table as you would edit `SKILL.TBL`.

### Things to note

1. By default, `SKILL_EXPANDED.TBL` has the same data in it as the unedited `SKILL.TBL`, plus the expanded skill slots and techicals.
2. As you may notice, passive skills now also have `ActiveSkillData` entries. I suggest to leave all `ActiveSkillData` entries for passives empty. *Technically speaking*, as all skills have `ActiveSkillData`, all skills can be made active. That does include passives. However using skills with "Passive" skill elements is a bit of undefined behaviour. The game may not crash but something might go wrong. Generally, the only thing that controls whether the skill is usable is the "Area Type" field in `ActiveSkillData` entry.
3. For active skills, there *must* be a corresponding BED file located in `P5REssentials/CPK/YourFolderName/BATTLE/SKILL` folder. Otherwise, when used, the game will be stuck in an infinite loop trying and failing to load the file. Sound file is optional.
4. You will also want to change the name for your new skills. You can do it as normal with `NAME.TBL`. You can find the expanded version with in the provided files. Edit is as normal (I suggest to use P5NameTBLEditor).
5. The trait segment is not expanded.

### Editing skill descriptions

It is presumed that you know how to edit skill descriptions of vanilla `SKILL.TBL`, at least by replacing the `DATMSG.PAK` file.

This is a short guide on how to correctly edit skill descriptions so that your changes can be merged with other mods. You can read more about it in P5R Essentials' [Documentation](https://sewer56.dev/p5rpc.modloader/usage/#example-bmd-in-archives).

First, add dependencies on "File Emulation Framework: Base Mod" (`reloaded.universal.fileemulationframework`) and "BMD Emulator For File Emulation Framework" (`reloaded.universal.fileemulationframework.bmd`).

In your mod's root folder, create a new folder named `FEmulator`. Now, create a folder path `FEmulator/PAK/INIT/DATMSG.PAK` and place an empty dummy filed named `datSkillHelp.bmd` inside the `DATMSG.PAK` folder. You will not be editing it manually, it's supposed to be left empty.

Create another folder path `FEmulator/BMD`, then create a new file named `datSkillHelp.msg` inside the `BMD` folder.

Inside of it, add a new message for the description of your skill as normal. Below is an example description for skill id 1800 (which is 708 in hex). These are all the lines contained in the file.
```
[msg skill_708]
[s]Medium Fire dmg to[n]one foe. High chance of[n]inflicting Rage.[n][e]
```

That's it. If you did everything correctly, the description of skill id 1800 should be changed to the one above.

### Example mod

You can find the `ExampleMod.zip` archive in the provided files. This is an example mod for Expanded Skill Table. It has 2 changes in it.

Firstly, it adds a new skill with skill id 1800. It's a simple fire skill that deals damage and has a chance to inflict rage. It has custom effect, name, description, vfx and sound.

Secondly, it adds a new technical combo for id 18. Now, almighty damage is technical for brainwashed enemies and deals 2 times the damage to them.

## Technical details

### Merging

All `SKILL.TBL`s and `SKILL_EXPANDED.TBL`s are automatically merged by the mod. Priority is by the load order, i.e. the mod at the bottom will have the highest priority. `SKILL_EXPANDED.TBL` also has priority over `SKILL.TBL` within the same mod.

Naturally `SKILL.TBL` will contribute less entries than `SKILL_EXPANDED.TBL`. It will, however, still overwrite changes to first 1056 `SkillElements`, 800 `ActiveSkillData` and 17 `TechnicalMaps` made by any `SKILL.TBL` or `SKILL_EXPANDED.TBL` loaded before it. Essentially the merging priority here is exactly the same as in P5REssentials.

Note that it merges by data type, i.e. a short will be merged as a short. If 2 different mods make changes to 2 different bytes of the same short, the full short from the mod with the highest priority will be present. Notably, in the provided template, "Flags", "Unknown[4]" and "Ailment List 1-3", "Effect List 1-3" and "Buffs/Debuffs" are all uints and will be merged as such.

### Implementation

The mod doesn't actually expand the original data arrays but instead creates new ones and redirect all read access to the new arrays.

The loading, parsing and merging of tables is done by the mod.

### Reading the table data from your code mods

If you want to get the addresses of the new data arrays in your code mod, you will have to get them from the mod's plugin data.

``` C#
var expandedSkillTableMod = _modLoader.GetActiveMods().FirstOrDefault(mod => mod.Generic.ModId == "p5rpc.SkillTableExpanded");
if (expandedSkillTableMod != null)
{
    if (expandedSkillTableMod.Generic is IModConfigV5 expandedSkillTableConfigV5)
    {
        if (expandedSkillTableConfigV5.PluginData["ExpandedTableAddresses"] is ulong[] expandedTableAddresses)
        {
            // Here, expandedTableAddresses contains the addresses of the skill data
            
            unsafe
            {
                var pSkillElements = (byte*)expandedTableAddresses[0];
                var pActiveSkillData = (byte*)expandedTableAddresses[1];
                var pTechnicalComboMaps = (byte*)expandedTableAddresses[2];
                
                // Skill element for skill id 12 (Agidyne)
                var agidyneSkillElement = pSkillElements[12 * 8];
            }
        }
    }
}
```
using System.Diagnostics;
using System.Drawing;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using SkillTableExpanded.Template;
using SkillTableExpanded.Configuration;
using Project.Utils;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Utilities;
using SkillTableExpanded.Utils;
using IReloadedHooks = Reloaded.Hooks.Definitions.IReloadedHooks;

namespace SkillTableExpanded;

public class Mod : ModBase // <= Do not Remove.
{
    /// <summary>
    /// Provides access to the mod loader API.
    /// </summary>
    private readonly IModLoader _modLoader;

    /// <summary>
    /// Provides access to the Reloaded.Hooks API.
    /// </summary>
    /// <remarks>This is null if you remove dependency on Reloaded.SharedLib.Hooks in your mod.</remarks>
    private readonly IReloadedHooks? _hooks;

    /// <summary>
    /// Provides access to the Reloaded logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Entry point into the mod, instance that created this class.
    /// </summary>
    private readonly IMod _owner;

    /// <summary>
    /// Provides access to this mod's configuration.
    /// </summary>
    private Config _configuration;

    /// <summary>
    /// The configuration of the currently executing mod.
    /// </summary>
    private readonly IModConfig _modConfig;


    private const uint ExpandedSkillSlotCount = 2502;
    private const uint ExpandedTableSkillSlotCount = 2500;
    
    private const uint SkillElementLength = 8;
    private const uint VanillaSkillElementsCount = 1056;
    
    private const uint ActiveSkillDataLength = 48;
    private const uint VanillaActiveSkillDataCount = 800;
    
    private const uint TechnicalComboMapLength = 40;
    private const uint VanillaTechnicalComboMapsCount = 17;
    private const uint ExpandedTechnicalComboMapsCount = 50;
    
    
    [Function(CallingConventions.Microsoft)]
    private unsafe delegate void LoadSkillTbl(uint* param1);
    private IHook<LoadSkillTbl>? _LoadSkillTbl_Hook;
    

    // is an expanded table, path
    private List<(bool, string)> _skillTableTuples = new();
    
    private readonly ScannerWrapper _scanner;
    private readonly Hooker _hooker;
    
    private readonly Pinnable<byte> _pinnedSkillElements = new(new byte[ExpandedSkillSlotCount * SkillElementLength]);
    
    private readonly Pinnable<byte> _pinnedActiveSkillData = new(new byte[ExpandedSkillSlotCount * ActiveSkillDataLength]);
    
    private readonly Pinnable<byte> _pinnedTechnicalComboMaps = new(new byte[ExpandedTechnicalComboMapsCount * TechnicalComboMapLength]);
    

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;
        
        Log.Initialize("p5rpc.SkillTableExpanded", _logger, Color.Aquamarine);
        
        _modLoader.OnModLoaderInitialized += OnModLoaderInitialised;
        
        _modLoader.GetController<IStartupScanner>().TryGetTarget(out var scanner);
        _scanner = new ScannerWrapper(scanner!, _hooks!);
        
        _hooker = new Hooker(_scanner, _hooks!);
        
        unsafe
        {
            _scanner.GetFunctionHook<LoadSkillTbl>("LoadSkillTbl", "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B 19",
                LoadSkillTbl_Custom, hook => _LoadSkillTbl_Hook = hook);
            
            _modConfig.PluginData["ExpandedTableAddresses"] = new[]
            {
                (ulong)_pinnedSkillElements.Pointer,
                (ulong)_pinnedActiveSkillData.Pointer,
                (ulong)_pinnedTechnicalComboMaps.Pointer
            };
        }
    }
    
    private void OnModLoaderInitialised()
    {
        foreach (var activeMod in _modLoader.GetActiveMods())
        {
            // SKILL.TBL
            if (activeMod.Generic.ModDependencies.Contains("p5rpc.modloader"))
            {
                var cpkDir = $"{_modLoader.GetDirectoryForModId(activeMod.Generic.ModId)}{Path.DirectorySeparatorChar}P5REssentials{Path.DirectorySeparatorChar}CPK";
                if (Directory.Exists(cpkDir))
                {
                    foreach (var dir in Directory.GetDirectories(cpkDir))
                    {
                        var tableDir = $"{dir}{Path.DirectorySeparatorChar}BATTLE{Path.DirectorySeparatorChar}TABLE";
                        if (Directory.Exists(tableDir))
                        {
                            var filePath = $"{tableDir}{Path.DirectorySeparatorChar}SKILL.TBL";
                            if (File.Exists(filePath))
                            {
                                _skillTableTuples.Add((false, filePath));
                            }
                        }
                    }
                }
            }
            
            // SKILL_EXPANDED.TBL
            if (activeMod.Generic.ModDependencies.Contains(_modConfig.ModId))
            {
                var estDir = $"{_modLoader.GetDirectoryForModId(activeMod.Generic.ModId)}{Path.DirectorySeparatorChar}ExpandedSkillTable";
                if (Directory.Exists(estDir))
                {
                    var filePath = $"{estDir}{Path.DirectorySeparatorChar}SKILL_EXPANDED.TBL";
                    if (File.Exists(filePath))
                    {
                        _skillTableTuples.Add((true, filePath));
                    }
                }
            }
        }
    }

    private unsafe uint LoadAndParseExpandedTableData(bool isExpandedTable, string filePath, byte* skillElementsPointer, byte* activeSkillDataPointer, byte* technicalComboMapsPointer)
    {
        using var readStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        
        using var binaryReader = new BigEndianBinaryReader(readStream);

        var version = 0U;
        if (isExpandedTable)
        {
            // Version
            version = binaryReader.ReadUInt32();
        
            // Alignment
            if (version >= 3)
            {
                binaryReader.ReadBytes(12);
            }
        }
        
        // Size of skill elements segment
        var size = binaryReader.ReadUInt32();
        
        for (var i = 0U; i < size; i += SkillElementLength)
        {
            // Element
            *skillElementsPointer = binaryReader.ReadByte();
            skillElementsPointer++;
            
            // Active or passive
            *skillElementsPointer = binaryReader.ReadByte();
            skillElementsPointer++;
            
            // Inheritability
            *skillElementsPointer = binaryReader.ReadByte();
            skillElementsPointer++;
            
            // Unused
            *skillElementsPointer = binaryReader.ReadByte();
            skillElementsPointer++;

            // Unused
            WriteBytesToPointer(skillElementsPointer, binaryReader.ReadBytesAndReverse(4));
            skillElementsPointer += 4;
        }
        
        // Alignment
        if (version >= 3 || !isExpandedTable)
        {
            binaryReader.ReadBytes(12);
        }
        else
        {
            binaryReader.ReadBytes(8);
        }
        
        // Size of active skill data segment
        size = binaryReader.ReadUInt32();
        
        for (var i = 0U; i < size; i += ActiveSkillDataLength)
        {
            // Flags
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(4));
            activeSkillDataPointer += 4;
            
            // Area type. Don't think second byte is used
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(2));
            activeSkillDataPointer += 2;
            
            // Damage stat
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Cost type
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Skill cost
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(2));
            activeSkillDataPointer += 2;
            
            // Physical or magic. Don't think second byte is used
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(2));
            activeSkillDataPointer += 2;
            
            // Number of targets
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Valid targets
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Additional target restrictions
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Unused
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Used but I've no idea what this is
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(4));
            activeSkillDataPointer += 4;
            
            // Accuracy
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Minimum number of hits
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Maximum number of hits
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Damage/Healing type for HP
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Base damage to HP
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(2));
            activeSkillDataPointer += 2;
            
            // Damage/Healing type for SP
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Unused
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Base damage to SP
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(2));
            activeSkillDataPointer += 2;
            
            // Apply or cure effect
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Effect chance
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Ailment list 1. Last byte is unused
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(4));
            activeSkillDataPointer += 4;
            
            // Ailment list 2 + buffs/debuffs
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(4));
            activeSkillDataPointer += 4;
            
            // Other buffs. Only first byte is used
            WriteBytesToPointer(activeSkillDataPointer, binaryReader.ReadBytesAndReverse(4));
            activeSkillDataPointer += 4;
            
            // Extra effects
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Crit chance
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Unused
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
            
            // Unused
            *activeSkillDataPointer = binaryReader.ReadByte();
            activeSkillDataPointer++;
        }
        
        // Alignment
        binaryReader.ReadBytes(12);

        if (version >= 2 || !isExpandedTable)
        {
            // Size of technical combo maps segment
            size = binaryReader.ReadUInt32();
            
            for (var i = 0U; i < size; i += TechnicalComboMapLength)
            {
                // Applicable ailments
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // All affinities are technical
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Technical affinity 1
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Technical affinity 2
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Technical affinity 3
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Technical affinity 4
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Technical affinity 5
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Damage multiplier
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Unused
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
                
                // Requires Knowing the Heart
                WriteBytesToPointer(technicalComboMapsPointer, binaryReader.ReadBytesAndReverse(4));
                technicalComboMapsPointer += 4;
            }
            
            // Alignment
            binaryReader.ReadBytes(12);
        }

        return version;
    }

    private unsafe void WriteBytesToPointer(byte* pByte, byte[] bytes)
    {
        // I could use Buffer.MemoryCopy but the longest array shouldn't exceed 4 bytes, so whatever
        
        for (var i = 0; i < bytes.Length; i++)
        {
            pByte[i] = bytes[i];
        }
    }
    
    private async Task LoadExpandedTables()
    {
        var watch = Stopwatch.StartNew();
        
        var originalDataPath = $"{_modLoader.GetDirectoryForModId(_modConfig.ModId)}{Path.DirectorySeparatorChar}originaldata";

        if (!File.Exists(originalDataPath))
        {
            throw new Exception("[p5rpc.SkillTableExpanded] Original data not found!");
        }
        
        var pinnedOriginalSkillElements = new Pinnable<byte>(new byte[ExpandedSkillSlotCount * SkillElementLength]);
        var pinnedOriginalActiveSkillData = new Pinnable<byte>(new byte[ExpandedSkillSlotCount * ActiveSkillDataLength]);
        var pinnedOriginalTechnicalComboMaps = new Pinnable<byte>(new byte[ExpandedTechnicalComboMapsCount * TechnicalComboMapLength]);

        unsafe
        {
            // Load the original data
            LoadAndParseExpandedTableData(true, originalDataPath, pinnedOriginalSkillElements.Pointer, pinnedOriginalActiveSkillData.Pointer, pinnedOriginalTechnicalComboMaps.Pointer);
        
            // Copy the original data as it's the baseline
            Buffer.MemoryCopy(pinnedOriginalSkillElements.Pointer, _pinnedSkillElements.Pointer,
                ExpandedSkillSlotCount * SkillElementLength, ExpandedSkillSlotCount * SkillElementLength);
            Buffer.MemoryCopy(pinnedOriginalActiveSkillData.Pointer, _pinnedActiveSkillData.Pointer,
                ExpandedSkillSlotCount * ActiveSkillDataLength, ExpandedSkillSlotCount * ActiveSkillDataLength);
            Buffer.MemoryCopy(pinnedOriginalTechnicalComboMaps.Pointer, _pinnedTechnicalComboMaps.Pointer,
                ExpandedTechnicalComboMapsCount * TechnicalComboMapLength, ExpandedTechnicalComboMapsCount * TechnicalComboMapLength);
        }
        
        var pinnedTempSkillElements = new Pinnable<byte>(new byte[ExpandedSkillSlotCount * SkillElementLength]);
        var pinnedTempActiveSkillData = new Pinnable<byte>(new byte[ExpandedSkillSlotCount * ActiveSkillDataLength]);
        var pinnedTempTechnicalComboMaps = new Pinnable<byte>(new byte[ExpandedTechnicalComboMapsCount * TechnicalComboMapLength]);
        
        // Check that all the SKILL.TBLs aren't open in something else. If not, delay.
        var allReadable = true;
        for (var i = 0; i < 10; i++)
        {
            allReadable = true;
            
            foreach (var tablePath in _skillTableTuples)
            {
                if (!tablePath.Item1)
                {
                    try
                    {
                        using var fStream = File.Open(tablePath.Item2, FileMode.Open, FileAccess.Read, FileShare.None);
                    }
                    catch (Exception)
                    {
                        allReadable = false;
                        await Task.Delay(500);
                    
                        break;
                    }
                }
            }

            if (allReadable)
            {
                break;
            }
        }

        if (!allReadable)
        {
            Log.Error("Cannot read one of the SKILL.TBls, they will not be merged.");
        }

        unsafe
        {
            foreach (var tuple in _skillTableTuples)
            {
                if (!tuple.Item1 && !allReadable)
                {
                    continue;
                }
                
                Log.Information($"Merging {(tuple.Item1 ? "SKILL_EXPANDED.TBL" : "SKILL.TBL")} at {tuple.Item2}");

                // Load the expanded table data
                var version = LoadAndParseExpandedTableData(tuple.Item1, tuple.Item2, pinnedTempSkillElements.Pointer, pinnedTempActiveSkillData.Pointer,
                    pinnedTempTechnicalComboMaps.Pointer);

                // Merge skill elements
                var pOriginal = pinnedOriginalSkillElements.Pointer;
                var pDestination = _pinnedSkillElements.Pointer;
                var pSource = pinnedTempSkillElements.Pointer;

                var count = tuple.Item1 ? ExpandedTableSkillSlotCount : VanillaSkillElementsCount;
                for (var j = 0; j < count; j++)
                {
                    // Element
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Active or passive
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Inheritability
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Unused
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Unused
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;
                }

                // Merge active skill data
                pOriginal = pinnedOriginalActiveSkillData.Pointer;
                pDestination = _pinnedActiveSkillData.Pointer;
                pSource = pinnedTempActiveSkillData.Pointer;

                count = tuple.Item1 ? ExpandedTableSkillSlotCount : VanillaActiveSkillDataCount;
                for (var j = 0; j < count; j++)
                {
                    // Flags
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Area type
                    if (*(ushort*)pSource != *(ushort*)pOriginal)
                    {
                        *(ushort*)pDestination = *(ushort*)pSource;
                    }

                    pOriginal += 2;
                    pDestination += 2;
                    pSource += 2;

                    // Damage stat
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Cost type
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Skill cost
                    if (*(ushort*)pSource != *(ushort*)pOriginal)
                    {
                        *(ushort*)pDestination = *(ushort*)pSource;
                    }

                    pOriginal += 2;
                    pDestination += 2;
                    pSource += 2;

                    // Physical or magic
                    if (*(ushort*)pSource != *(ushort*)pOriginal)
                    {
                        *(ushort*)pDestination = *(ushort*)pSource;
                    }

                    pOriginal += 2;
                    pDestination += 2;
                    pSource += 2;

                    // Number of targets
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Valid targets
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Additional target restrictions
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Unused
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Used but unknown
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Accuracy
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Minimum number of hits
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Maximum number of hits
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Damage/Healing type for HP
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Base damage to HP
                    if (*(ushort*)pSource != *(ushort*)pOriginal)
                    {
                        *(ushort*)pDestination = *(ushort*)pSource;
                    }

                    pOriginal += 2;
                    pDestination += 2;
                    pSource += 2;

                    // Damage/Healing type for SP
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Unused
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Base damage to SP
                    if (*(ushort*)pSource != *(ushort*)pOriginal)
                    {
                        *(ushort*)pDestination = *(ushort*)pSource;
                    }

                    pOriginal += 2;
                    pDestination += 2;
                    pSource += 2;

                    // Apply or cure effect
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Effect chance
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Effect list 1
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Effect list 2
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    // Other buffs. Merged by byte despite being a uint
                    for (var k = 0; k < 4; k++)
                    {
                        if (*pSource != *pOriginal)
                        {
                            *pDestination = *pSource;
                        }

                        pOriginal += 1;
                        pDestination += 1;
                        pSource += 1;
                    }

                    // Extra effects
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Crit chance
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Unused
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;

                    // Unused
                    if (*pSource != *pOriginal)
                    {
                        *pDestination = *pSource;
                    }

                    pOriginal += 1;
                    pDestination += 1;
                    pSource += 1;
                }

                if (version < 2 && tuple.Item1)
                {
                    continue;
                }

                // Merge technical combo maps
                pOriginal = pinnedOriginalTechnicalComboMaps.Pointer;
                pDestination = _pinnedTechnicalComboMaps.Pointer;
                pSource = pinnedTempTechnicalComboMaps.Pointer;

                count = tuple.Item1 ? ExpandedTechnicalComboMapsCount : VanillaTechnicalComboMapsCount;
                for (var j = 0; j < count; j++)
                {
                    // Applicable ailments
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // All affinities are technical
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Technical affinity 1
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Technical affinity 2
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Technical affinity 3
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Technical affinity 4
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Technical affinity 5
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Damage multiplier
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Unused
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;

                    // Requires Knowing the Heart
                    if (*(uint*)pSource != *(uint*)pOriginal)
                    {
                        *(uint*)pDestination = *(uint*)pSource;
                    }

                    pOriginal += 4;
                    pDestination += 4;
                    pSource += 4;
                }
            }
            
            // Fill the vanilla skill tbl data in game's memory with merged values, just in case
            Buffer.MemoryCopy(_pinnedSkillElements.Pointer, (byte*)_hooker.VanillaSkillElementsAddress,
                VanillaSkillElementsCount * SkillElementLength, VanillaSkillElementsCount * SkillElementLength);
            Buffer.MemoryCopy(_pinnedActiveSkillData.Pointer, (byte*)_hooker.VanillaActiveSkillDataAddress,
                VanillaActiveSkillDataCount * ActiveSkillDataLength, VanillaActiveSkillDataCount * ActiveSkillDataLength);
            Buffer.MemoryCopy(_pinnedTechnicalComboMaps.Pointer, (byte*)_hooker.VanillaTechnicalComboMapsAddress,
                VanillaTechnicalComboMapsCount * TechnicalComboMapLength, VanillaTechnicalComboMapsCount * TechnicalComboMapLength);
        }
        
        watch.Stop();
        Log.Information($"Merging completed in {watch.ElapsedMilliseconds}ms");
    }

    private unsafe void LoadSkillTbl_Custom(uint* param1)
    {
        _LoadSkillTbl_Hook!.OriginalFunction(param1);

        if (!_skillTableTuples.Any(x => x.Item1))
        {
            Log.Warning("No expanded skill tables found.");

            return;
        }
                
        _hooker.CreateHooks(_pinnedSkillElements.Pointer, _pinnedActiveSkillData.Pointer, _pinnedTechnicalComboMaps.Pointer, ExpandedSkillSlotCount, ExpandedTechnicalComboMapsCount);
                
        Task.Run(LoadExpandedTables);
    }
    
    #region Standard Overrides

    public override void ConfigurationUpdated(Config configuration)
    {
        // Apply settings from configuration.
        // ... your code here.
        _configuration = configuration;
        _logger.WriteLine($"[{_modConfig.ModId}] Config Updated: Applying");
    }

    #endregion

    #region For Exports, Serialization etc.

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Mod()
    {
    }
#pragma warning restore CS8618

    #endregion
}
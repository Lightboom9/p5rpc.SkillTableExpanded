using System.Drawing;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using SkillTableExpanded.Template;
using SkillTableExpanded.Configuration;
using Project.Utils;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Memory.Utilities;
using IReloadedHooks = Reloaded.Hooks.Definitions.IReloadedHooks;

namespace SkillTableExpanded;

/// <summary>
/// Your mod logic goes here.
/// </summary>
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
    
    
    [Function(CallingConventions.Microsoft)]
    private unsafe delegate long FUN_140e39b70(long param1);
    private IHook<FUN_140e39b70>? FUN_140e39b70_Hook;
    
    private nint _DAT_142226bf0_Address; // Starting address of SKILL.TBL in memory

    private nint _LEA_140105ab5_Address;
    
    
    private readonly ScannerWrapper _scanner;

    private readonly List<string> _customInstructionSetList = new();
    private readonly List<IAsmHook> _asmHooks = new();
    private readonly Pinnable<byte> _pinnedSkillTblBytes = new(new byte[800 * 48]);

    public Mod(ModContext context)
    {
        _modLoader = context.ModLoader;
        _hooks = context.Hooks;
        _logger = context.Logger;
        _owner = context.Owner;
        _configuration = context.Configuration;
        _modConfig = context.ModConfig;

#if DEBUG
        //Debugger.Launch();
#endif
        
        Log.Initialize("p5rpc.SkillTableExpanded", _logger, Color.Orange);

        _modLoader.GetController<IStartupScanner>().TryGetTarget(out var scanner);

        _scanner = new ScannerWrapper(scanner!, _hooks!);
        
        unsafe
        {
            // ======================================================
            //                      FUNCTIONS
            // ======================================================
            // Loads TBL files
            _scanner.GetFunctionHook<FUN_140e39b70>("FUN_140e39b70", "48 89 5C 24 ?? 56 48 83 EC 20 48 8B 59 ?? 8B 03",
                FUN_140e39b70_Custom, hook => FUN_140e39b70_Hook = hook);
            
            
            // ======================================================
            //                        DATA
            // ======================================================
            _scanner.ScanForData("DAT_142226bf0", "48 8D 05 ?? ?? ?? ?? 4C 03 CD", 7, 3, 0,
                address => _DAT_142226bf0_Address = address);
            
            _scanner.ScanForData("LEA_140105ab5", "4C 8D 15 ?? ?? ?? ?? 4C 8D 0C ?? 4D 03 C9", 7, 3, 0,
                address => _LEA_140105ab5_Address = address);
        }
    }
    
    private unsafe long FUN_140e39b70_Custom(long param1)
    {
        var result = FUN_140e39b70_Hook!.OriginalFunction(param1);
        
        var plVar5 = *(long**)(param1 + 72);
        var switcher = *(uint*)plVar5;
        if (switcher == 5)
        {
            if (plVar5[1] != 0)
            {
                Log.Information("SKILL.TBL loaded and ready!");

                var mySkillBytes = _pinnedSkillTblBytes.Pointer;
                var japaneseSkillBytes = (byte*)0x142226BF0;
                
                Buffer.MemoryCopy(japaneseSkillBytes, mySkillBytes, 800 * 48, 800 * 48);
                
                Log.Information($"My pointer address: {(ulong)_pinnedSkillTblBytes.Pointer:X}, original pointer address: {(ulong)_DAT_142226bf0_Address:X}, difference: {(ulong)_pinnedSkillTblBytes.Pointer - (ulong)_DAT_142226bf0_Address}");
                
                AboardExclamationPoint();
            }
        }

        return result;
    }
    
    private unsafe void AboardExclamationPoint()
    {
        ConstructHook(_LEA_140105ab5_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        /* Examples (some outdated)
        Log.Information($"We do a little tomfoolery.");
        *(short*)((ulong)_pinnedSkillTblBytes.Pointer + 12 * 48 + 24) = 14400;

        ConstructHook(0x140e1b33c, [
            "push rcx",
            "add rcx, 0x18",
            "add rcx, r15",
            "movsx eax, word [rcx]",

            "movd xmm0, eax",
            "pop rcx"
        ], "r15");

        string[] functionForCalcBaseDamage =
        {
            "use64",
            "push r10",
            "push r11",

            //"movsx eax, word [rcx + 0x18]", // old
            "mov r10, rcx",
            "add r10, 0x18",
            "mov r11, 0x142226BF0",
            "sub r10, r11",
            $"mov r11, {(ulong)_pinnedSkillTblBytes.Pointer}",
            "add r10, r11",
            "movsx eax, word [r10]",

            "movd xmm0, eax",
            "pop r11",
            "pop r10"
        };
        _asmHooks.Add(_hooks!.CreateAsmHook(functionForCalcBaseDamage, 0x140e1b33c, AsmHookBehaviour.DoNotExecuteOriginal).Activate());
        */
    }
    
    private void ConstructHook(long address, string[] function)
    {
        _asmHooks.Add(_hooks!.CreateAsmHook(function, address, AsmHookBehaviour.DoNotExecuteOriginal).Activate());
    }

    private unsafe string[] CreateCustomInstructionSet(string[] customInstructions, string pointerAddressRegister)
    {
        _customInstructionSetList.Clear();

        var tempRegister = "r14";
        if (tempRegister == pointerAddressRegister)
        {
            tempRegister = "r15";
        }
            
        _customInstructionSetList.AddRange([
            "use64",
            $"push {pointerAddressRegister}",
            $"mov {pointerAddressRegister}, 0x{(ulong)_pinnedSkillTblBytes.Pointer:X}",
            $"push {tempRegister}",
            $"mov {tempRegister}, 0x{(ulong)_DAT_142226bf0_Address:X}",
            $"sub {pointerAddressRegister}, {tempRegister}",
            $"pop {tempRegister}"
        ]);
        
        _customInstructionSetList.AddRange(customInstructions);
        
        _customInstructionSetList.Add($"pop {pointerAddressRegister}");

        return _customInstructionSetList.ToArray();
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
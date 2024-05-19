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
    
    private readonly ScannerWrapper _scanner;

    private IAsmHook _calcDamageAsmHook;
    private IAsmHook _calcBaseDamageAsmHook;
    private readonly Pinnable<byte> _pinnedSkillTblBytes = new Pinnable<byte>(new byte[800 * 48]);

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
            
            
            // 142226bf0 - start position in memory for SKILL.TBL
            // 0x{(ulong)_pinDamage.Pointer:X}
            
            string[] functionForCalcDamage =
            {
                "use64",
                "movzx edi, word [r9 + 0x18]",
                "mov rcx, r12"
            };
            _calcDamageAsmHook = _hooks!.CreateAsmHook(functionForCalcDamage, 0x140e14e40, AsmHookBehaviour.DoNotExecuteOriginal).Activate();
            
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
                $"mov r11, 0x{(ulong)_pinnedSkillTblBytes.Pointer:X}",
                "add r10, r11",
                "movsx eax, word [r10]",
                
                "movd xmm0, eax",
                "pop r11",
                "pop r10"
            };
            _calcBaseDamageAsmHook = _hooks!.CreateAsmHook(functionForCalcBaseDamage, 0x140e1b33c, AsmHookBehaviour.DoNotExecuteOriginal).Activate();
            
            
            // ======================================================
            //                        DATA
            // ======================================================
        }
    }
    
    private unsafe long FUN_140e39b70_Custom(long param1)
    {
        var plVar5 = *(long**)(param1 + 72);
        var switcher = *(uint*)plVar5;
        if (switcher == 5)
        {
            if (plVar5[1] != 0)
            {
                Log.Information("SKILL.TBL loaded!");

                var mySkillBytes = _pinnedSkillTblBytes.Pointer;
                var japaneseSkillBytes = (byte*)0x142226BF0;
                
                Buffer.MemoryCopy(japaneseSkillBytes, mySkillBytes, 800 * 48, 800 * 48);
            }
        }
        
        var result = FUN_140e39b70_Hook!.OriginalFunction(param1);

        return result;
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
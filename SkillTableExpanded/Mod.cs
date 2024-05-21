using System.Drawing;
using System.Text;
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
    
    [Function(CallingConventions.Microsoft)]
    private unsafe delegate void FUN_140e3dc90(uint* param1);
    private IHook<FUN_140e3dc90>? FUN_140e3dc90_Hook;
    
    private nint _DAT_142226bf0_Address; // Starting address of SKILL.TBL in memory

    // For DAT_142226bf0
    // 140e3dced, 140e3dde0 and 140e3dde5 are skipped as they seem to be related to loading the table and writing data
    private nint _LEA_140105ab5_Address;
    private nint _TEST_140719e12_Address;
    private nint _LEA_14071bd02_Address;
    private nint _MOV_14073a0ae_Address;
    private nint _MOV_14073c18b_Address;
    private nint _TEST_14073c1df_Address;
    private nint _MOV_14073d50e_Address;
    private nint _LEA_1407521d7_Address;
    private nint _LEA_140752496_Address;
    private nint _LEA_14075277d_Address;
    private nint _LEA_140752df5_Address;
    private nint _LEA_140752f6b_Address;
    private nint _LEA_140753697_Address;
    private nint _LEA_1407536ca_Address;
    private nint _LEA_14075384d_Address;
    private nint _LEA_1407538ad_Address;
    private nint _LEA_14075396b_Address;
    private nint _LEA_140753af8_Address;
    private nint _LEA_140753c6a_Address;
    private nint _LEA_140753dfd_Address;
    private nint _LEA_140753fe4_Address;
    private nint _LEA_1407541de_Address;
    private nint _LEA_140754479_Address;
    private nint _LEA_14075e087_Address;
    private nint _LEA_140771dfc_Address;
    private nint _LEA_140771f54_Address;
    private nint _LEA_1407724e7_Address;
    private nint _TEST_14077b9eb_Address;
    private nint _MOV_14077c383_Address;
    private nint _LEA_1407afc81_Address;
    private nint _LEA_1407aff57_Address;
    private nint _LEA_1407b076e_Address;
    private nint _LEA_1407b40a0_Address;
    private nint _LEA_1407b81fb_Address;
    private nint _LEA_1407b9128_Address;
    private nint _LEA_140801ee2_Address;
    private nint _LEA_140810877_Address;
    private nint _LEA_140811470_Address;
    private nint _LEA_14081162d_Address;
    private nint _LEA_14085e961_Address;
    private nint _LEA_140868c5e_Address;
    private nint _LEA_140868f94_Address;
    private nint _LEA_14086fbc4_Address;
    private nint _LEA_140870ad8_Address;
    private nint _LEA_140871e93_Address;
    private nint _MOV_1408ec3c4_Address;
    private nint _LEA_1409f0136_Address;
    private nint _LEA_140acc910_Address;
    private nint _LEA_140accb16_Address;
    private nint _LEA_140accf19_Address;
    private nint _LEA_140acd266_Address;
    private nint _LEA_140acd6f0_Address;
    private nint _LEA_140ad431e_Address;
    private nint _LEA_140ad4414_Address;
    private nint _LEA_140ad4431_Address;
    private nint _LEA_140ad44d5_Address;
    private nint _LEA_140ad4615_Address;
    private nint _LEA_140ad474b_Address;
    private nint _LEA_140ad47c4_Address;
    private nint _LEA_140ad48e6_Address;
    private nint _LEA_140ad55fa_Address;
    private nint _LEA_140ad5f65_Address;
    private nint _LEA_140ad653a_Address;
    private nint _LEA_140ad6671_Address;
    private nint _LEA_140ad707f_Address;
    private nint _LEA_140ad74cf_Address;
    private nint _LEA_140ad98cb_Address;
    private nint _LEA_140ad9b75_Address;
    private nint _LEA_140aedd10_Address;
    private nint _LEA_140aee4f6_Address;
    private nint _LEA_140aee8d0_Address;
    private nint _LEA_140aeec6a_Address;
    private nint _LEA_140b0fd01_Address;
    private nint _LEA_140b10242_Address;
    private nint _LEA_140b6650b_Address;
    private nint _LEA_140b76766_Address;
    private nint _LEA_140b768bd_Address;
    private nint _LEA_140b768fa_Address;
    private nint _LEA_140b7691c_Address;
    private nint _LEA_140b769ca_Address;
    private nint _LEA_140b76b0e_Address;
    private nint _LEA_140b76c62_Address;
    private nint _LEA_140b76df8_Address;
    private nint _LEA_140d23a05_Address;
    private nint _LEA_140d6b75a_Address;
    private nint _LEA_140d6b845_Address;
    private nint _LEA_140d6b8f5_Address;
    private nint _LEA_140d6b925_Address;
    private nint _LEA_140d6bd7d_Address;
    private nint _LEA_140d73e5e_Address;
    private nint _LEA_140dbaaa7_Address;
    private nint _LEA_140dbac38_Address;
    private nint _LEA_140e0fb52_Address;
    private nint _LEA_140e10232_Address;
    private nint _MOV_140e1072a_Address;
    private nint _LEA_140e1223f_Address;
    private nint _LEA_140e12590_Address;
    private nint _TEST_140e12933_Address;
    private nint _TEST_140e13177_Address;
    private nint _MOV_140e1344d_Address;
    private nint _LEA_140e139f1_Address;
    private nint _LEA_140e14347_Address;
    private nint _LEA_140e14502_Address;
    private nint _LEA_140e1460a_Address;
    private nint _LEA_140e1492a_Address;
    private nint _LEA_140e1498c_Address;
    private nint _LEA_140e14b56_Address;
    private nint _LEA_140e14dd5_Address;
    private nint _MOV_140e16e9e_Address;
    private nint _LEA_140e1b1ef_Address;
    private nint _LEA_140e1b2bd_Address;
    private nint _LEA_140e1b2f3_Address;
    private nint _LEA_140e1b31b_Address;
    private nint _LEA_140e1b3b1_Address;
    private nint _LEA_140e1c435_Address;
    private nint _LEA_140e32ef3_Address;
    private nint _LEA_140e32f0a_Address;
    private nint _LEA_140e32f47_Address;
    private nint _LEA_140e3318a_Address;
    private nint _LEA_140e3330a_Address;
    private nint _LEA_140e3342a_Address;
    private nint _LEA_140e3344a_Address;
    private nint _MOV_140e3358a_Address;
    private nint _MOV_140e33933_Address;
    private nint _MOV_140e34230_Address;
    private nint _MOV_140e34438_Address;
    private nint _MOV_140e34732_Address;
    private nint _MOV_140e34750_Address;
    private nint _MOV_140e34769_Address;
    private nint _MOV_140e34782_Address;
    private nint _LEA_140e3eb53_Address;
    private nint _LEA_1417b4694_Address;
    private nint _LEA_14a8ab88f_Address;
    private nint _LEA_14bd93fea_Address;
    
    
    private readonly ScannerWrapper _scanner;

    private readonly List<string> _customInstructionSetList = new();
    private readonly List<IAsmHook> _asmHooks = new();
    private readonly Pinnable<byte> _pinnedSkillTblBytes = new(new byte[800 * 48]);
    private long _customSkillDataOffset;

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
            _scanner.GetFunctionHook<FUN_140e3dc90>("FUN_140e3dc90", "48 89 5C 24 ?? 48 89 6C 24 ?? 48 89 74 24 ?? 57 48 83 EC 20 8B 19",
                FUN_140e3dc90_Custom, hook => FUN_140e3dc90_Hook = hook);
            
            
            // ======================================================
            //                        DATA
            // ======================================================
            _scanner.ScanForData("DAT_142226bf0", "48 8D 05 ?? ?? ?? ?? 4C 03 CD", 7, 3, 0,
                address => _DAT_142226bf0_Address = address);
            
            _scanner.Scan("LEA_140105ab5", "4C 8D 15 ?? ?? ?? ?? 4C 8D 0C ?? 4D 03 C9",
                address => _LEA_140105ab5_Address = address);
            _scanner.Scan("TEST_140719e12", "41 F7 84 ?? ?? ?? ?? ?? 00 00 01 00 0F 84 ?? ?? ?? ?? 48 8B 9B ?? ?? ?? ??",
                address => _TEST_140719e12_Address = address);
            _scanner.Scan("LEA_14071bd02", "48 8D 05 ?? ?? ?? ?? F7 04 ?? 00 00 01 00 74 ??",
                address => _LEA_14071bd02_Address = address);
            _scanner.Scan("MOV_14073a0ae", "8B 84 ?? ?? ?? ?? ?? D1 E8 A8 01 0F 84 ?? ?? ?? ?? 49 8B 06",
                address => _MOV_14073a0ae_Address = address);
            _scanner.Scan("MOV_14073c18b", "8B 84 ?? ?? ?? ?? ?? C1 E8 11",
                address => _MOV_14073c18b_Address = address);
            _scanner.Scan("TEST_14073c1df", "F7 84 ?? ?? ?? ?? ?? 00 00 01 00 0F 45 CE",
                address => _TEST_14073c1df_Address = address);
            _scanner.Scan("MOV_14073c18b", "8B 84 ?? ?? ?? ?? ?? C1 E8 11",
                address => _MOV_14073c18b_Address = address);
            _scanner.Scan("MOV_14073d50e", "8B 84 ?? ?? ?? ?? ?? D1 E8 A8 01 0F 84 ?? ?? ?? ?? 49 8D 96 ?? ?? ?? ??",
                address => _MOV_14073d50e_Address = address);
            _scanner.Scan("LEA_1407521d7", "48 8D 0D ?? ?? ?? ?? 74 ?? 41 0F B7 16",
                address => _LEA_1407521d7_Address = address);
            _scanner.Scan("LEA_140752496", "4C 8D 15 ?? ?? ?? ?? 42 80 7C ?? ?? 64",
                address => _LEA_140752496_Address = address);
            _scanner.Scan("LEA_14075277d", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? C1 E8 11 A8 01 0F 84 ?? ?? ?? ?? 49 8B 4D ??",
                address => _LEA_14075277d_Address = address);
            _scanner.Scan("LEA_140752df5", "48 8D 05 ?? ?? ?? ?? 0F B6 44 ?? ?? FF C8",
                address => _LEA_140752df5_Address = address);
            _scanner.Scan("LEA_140752f6b", "48 8D 05 ?? ?? ?? ?? 0F B6 44 ?? ?? 3C 0D",
                address => _LEA_140752f6b_Address = address);
            _scanner.Scan("LEA_140753697", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8 A8 01 74 ?? 0F BA EE 08",
                address => _LEA_140753697_Address = address);
            _scanner.Scan("LEA_1407536ca", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? C1 E8 11 A8 01 0F 85 ?? ?? ?? ?? 0F 57 C0",
                address => _LEA_1407536ca_Address = address);
            _scanner.Scan("LEA_14075384d", "4C 8D 15 ?? ?? ?? ?? 4B 8D 04 ?? 48 03 C0",
                address => _LEA_14075384d_Address = address);
            _scanner.Scan("LEA_1407538ad", "48 8D 05 ?? ?? ?? ?? 48 8D 14 ?? 48 03 D2 0F B6 44 ?? ?? 3C 0D 77 ?? B9 10 25 00 00 0F A3 C1 72 ??",
                address => _LEA_1407538ad_Address = address);
            _scanner.Scan("LEA_14075396b", "48 8D 05 ?? ?? ?? ?? 48 8D 14 ?? 48 03 D2 0F B6 44 ?? ?? 3C 0D 77 ?? B9 10 25 00 00 0F A3 C1 0F 82 ?? ?? ?? ??",
                address => _LEA_14075396b_Address = address);
            _scanner.Scan("LEA_140753af8", "4C 8D 3D ?? ?? ?? ?? 48 8B F0",
                address => _LEA_140753af8_Address = address);
            _scanner.Scan("LEA_140753c6a", "4C 8D 15 ?? ?? ?? ?? 41 0F B7 06",
                address => _LEA_140753c6a_Address = address);
            _scanner.Scan("LEA_140753dfd", "4C 8D 2D ?? ?? ?? ?? 48 85 DB 0F 84 ?? ?? ?? ?? 41 0F B7 0F",
                address => _LEA_140753dfd_Address = address);
            _scanner.Scan("LEA_140753fe4", "4C 8D 3D ?? ?? ?? ?? 48 85 C0 74 ?? 41 0F B7 06",
                address => _LEA_140753fe4_Address = address);
            _scanner.Scan("LEA_1407541de", "48 8D 05 ?? ?? ?? ?? 80 7C ?? ?? 64",
                address => _LEA_1407541de_Address = address);
            _scanner.Scan("LEA_140754479", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? C1 E8 11 A8 01 0F 85 ?? ?? ?? ?? 44 8B 5C 24 ??",
                address => _LEA_140754479_Address = address);
            _scanner.Scan("LEA_14075e087", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8 A8 01 74 ?? 41 8B 8E ?? ?? ?? ??",
                address => _LEA_14075e087_Address = address);
            _scanner.Scan("LEA_140771dfc", "4C 8D 05 ?? ?? ?? ?? 85 DB 0F 85 ?? ?? ?? ?? 0F 57 C0",
                address => _LEA_140771dfc_Address = address);
            _scanner.Scan("LEA_140771f54", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 04 75 ??",
                address => _LEA_140771f54_Address = address);
            _scanner.Scan("LEA_1407724e7", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 04 0F 85 ?? ?? ?? ??",
                address => _LEA_1407724e7_Address = address);
            _scanner.Scan("TEST_14077b9eb", "41 F7 84 ?? ?? ?? ?? ?? 00 00 01 00 0F 84 ?? ?? ?? ?? 48 8B 9E ?? ?? ?? ??",
                address => _TEST_14077b9eb_Address = address);
            _scanner.Scan("MOV_14077c383", "41 8B 84 ?? ?? ?? ?? ?? D1 E8 A8 01 0F 85 ?? ?? ?? ?? F7 86 ?? ?? ?? ?? 00 08 00 00",
                address => _MOV_14077c383_Address = address);
            _scanner.Scan("LEA_1407afc81", "4C 8D 05 ?? ?? ?? ?? 8B 4A ??",
                address => _LEA_1407afc81_Address = address);
            _scanner.Scan("LEA_1407aff57", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8 A8 01 75 ?? 49 8B 06",
                address => _LEA_1407aff57_Address = address);
            _scanner.Scan("LEA_1407b076e", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8 A8 01 0F 84 ?? ?? ?? ??",
                address => _LEA_1407b076e_Address = address);
            _scanner.Scan("LEA_1407b40a0", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? C1 E8 11 A8 01 0F 85 ?? ?? ?? ?? 49 8D 56 ??",
                address => _LEA_1407b40a0_Address = address);
            _scanner.Scan("LEA_1407b81fb", "48 8D 05 ?? ?? ?? ?? F7 04 ?? 00 00 01 00 75 ??",
                address => _LEA_1407b81fb_Address = address);
            _scanner.Scan("LEA_1407b9128", "48 8D 1D ?? ?? ?? ?? 48 8D 0C ?? 48 03 C9 F7 C2 FF FF 07 00",
                address => _LEA_1407b9128_Address = address);
            _scanner.Scan("LEA_140801ee2", "4C 8D 05 ?? ?? ?? ?? 48 8D 3C ?? 48 C1 E7 04",
                address => _LEA_140801ee2_Address = address);
            _scanner.Scan("LEA_140810877", "4C 8D 0D ?? ?? ?? ?? 8B C8 0F B7 94 ?? ?? ?? ?? ??",
                address => _LEA_140810877_Address = address);
            _scanner.Scan("LEA_140811470", "4C 8D 0D ?? ?? ?? ?? 41 B8 1E 03 00 00",
                address => _LEA_140811470_Address = address);
            _scanner.Scan("LEA_14081162d", "48 8D 05 ?? ?? ?? ?? F6 44 ?? ?? 03",
                address => _LEA_14081162d_Address = address);
            _scanner.Scan("LEA_14085e961", "4C 8D 3D ?? ?? ?? ?? 48 8D 1C ?? 48 03 DB",
                address => _LEA_14085e961_Address = address);
            _scanner.Scan("LEA_140868c5e", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8 A8 01 75 ?? 44 0F B7 CB B2 01 41 8B C9 E8 ?? ?? ?? ?? 84 C0 75 ??",
                address => _LEA_140868c5e_Address = address);
            _scanner.Scan("LEA_140868f94", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8 A8 01 74 ?? 41 80 7F ?? 00",
                address => _LEA_140868f94_Address = address);
            _scanner.Scan("LEA_14086fbc4", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8 A8 01 75 ?? 44 0F B7 CB B2 01 41 8B C9 E8 ?? ?? ?? ?? 84 C0 74 ??",
                address => _LEA_14086fbc4_Address = address);
            _scanner.Scan("LEA_140870ad8", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? C1 E8 11 A8 01 0F 84 ?? ?? ?? ?? 49 8B 45 ??",
                address => _LEA_140870ad8_Address = address);
            _scanner.Scan("LEA_140871e93", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? C1 E8 11 A8 01 74 ??",
                address => _LEA_140871e93_Address = address);
            _scanner.Scan("MOV_1408ec3c4", "8B 84 ?? ?? ?? ?? ?? D1 E8 A8 01 0F 84 ?? ?? ?? ?? 49 8B 4E ??",
                address => _MOV_1408ec3c4_Address = address);
            _scanner.Scan("LEA_1409f0136", "4C 8D 0D ?? ?? ?? ?? 8D 42 ?? 66 41 3B C4",
                address => _LEA_1409f0136_Address = address);
            _scanner.Scan("LEA_140acc910", "48 8D 1D ?? ?? ?? ?? 42 F6 44 ?? ?? 03",
                address => _LEA_140acc910_Address = address);
            _scanner.Scan("LEA_140accb16", "48 8D 1D ?? ?? ?? ?? 48 8B 54 24 ?? 48 8B 44 24 ??",
                address => _LEA_140accb16_Address = address);
            _scanner.Scan("LEA_140accf19", "4C 8D 3D ?? ?? ?? ?? 48 8D 4F ?? 0F 57 C0 48 8B 3F F3 0F 7F 45 ?? 48 89 5D ??",
                address => _LEA_140accf19_Address = address);
            _scanner.Scan("LEA_140acd266", "48 8D 0D ?? ?? ?? ?? 42 0F B6 44 ?? ??",
                address => _LEA_140acd266_Address = address);
            _scanner.Scan("LEA_140acd6f0", "4C 8D 35 ?? ?? ?? ?? 66 0F 1F 84 ?? 00 00 00 00 48 8B D7",
                address => _LEA_140acd6f0_Address = address);
            _scanner.Scan("LEA_140ad431e", "48 8D 0D ?? ?? ?? ?? 45 33 ED 48 89 7D ??",
                address => _LEA_140ad431e_Address = address);
            _scanner.Scan("LEA_140ad4414", "48 8D 15 ?? ?? ?? ?? 42 F6 44 ?? ?? 40",
                address => _LEA_140ad4414_Address = address);
            _scanner.Scan("LEA_140ad4431", "48 8D 15 ?? ?? ?? ?? 42 8B 44 ?? ??",
                address => _LEA_140ad4431_Address = address);
            _scanner.Scan("LEA_140ad44d5", "48 8D 05 ?? ?? ?? ?? 42 23 5C ?? ?? 48 8B 45 ?? 0F BA F3 14 4C 89 4D ??",
                address => _LEA_140ad44d5_Address = address);
            _scanner.Scan("LEA_140ad4615", "48 8D 15 ?? ?? ?? ?? 80 7C ?? ?? 02 0F 85 ?? ?? ?? ??",
                address => _LEA_140ad4615_Address = address);
            _scanner.Scan("LEA_140ad474b", "48 8D 15 ?? ?? ?? ?? 80 7C ?? ?? 02 75 ??",
                address => _LEA_140ad474b_Address = address);
            _scanner.Scan("LEA_140ad47c4", "48 8D 0D ?? ?? ?? ?? 42 F6 44 ?? ?? 10",
                address => _LEA_140ad47c4_Address = address);
            _scanner.Scan("LEA_140ad48e6", "48 8D 0D ?? ?? ?? ?? 41 83 FD 01",
                address => _LEA_140ad48e6_Address = address);
            _scanner.Scan("LEA_140ad55fa", "48 8D 05 ?? ?? ?? ?? 83 7C ?? ?? 00 4C 8D 34 ??",
                address => _LEA_140ad55fa_Address = address);
            _scanner.Scan("LEA_140ad5f65", "48 8D 05 ?? ?? ?? ?? 41 BA 01 00 00 00",
                address => _LEA_140ad5f65_Address = address);
            _scanner.Scan("LEA_140ad653a", "48 8D 1D ?? ?? ?? ?? 41 0F B7 FD",
                address => _LEA_140ad653a_Address = address);
            _scanner.Scan("LEA_140ad6671", "48 8D 15 ?? ?? ?? ?? 48 8B 4D ?? 85 C0",
                address => _LEA_140ad6671_Address = address);
            _scanner.Scan("LEA_140ad707f", "4C 8D 0D ?? ?? ?? ?? 42 80 7C ?? ?? 02",
                address => _LEA_140ad707f_Address = address);
            _scanner.Scan("LEA_140ad74cf", "4C 8D 05 ?? ?? ?? ?? 42 0F B6 4C ?? ??",
                address => _LEA_140ad74cf_Address = address);
            _scanner.Scan("LEA_140ad98cb", "4C 8D 3D ?? ?? ?? ?? 4C 89 9B ?? ?? ?? ?? 85 FF",
                address => _LEA_140ad98cb_Address = address);
            _scanner.Scan("LEA_140ad9b75", "4C 8D 3D ?? ?? ?? ?? 4C 89 B3 ?? ?? ?? ??",
                address => _LEA_140ad9b75_Address = address);
            _scanner.Scan("LEA_140aedd10", "4C 8D 15 ?? ?? ?? ?? 66 0F 1F 84 ?? 00 00 00 00 8B C2",
                address => _LEA_140aedd10_Address = address);
            _scanner.Scan("LEA_140aee4f6", "4C 8D 15 ?? ?? ?? ?? 48 8D 0C ?? 48 03 C9",
                address => _LEA_140aee4f6_Address = address);
            _scanner.Scan("LEA_140aee8d0", "4C 8D 15 ?? ?? ?? ?? 0F B7 83 ?? ?? ?? ??",
                address => _LEA_140aee8d0_Address = address);
            _scanner.Scan("LEA_140aeec6a", "4C 8D 15 ?? ?? ?? ?? 48 8B 49 ?? 48 89 4D ?? 48 89 7D ??",
                address => _LEA_140aeec6a_Address = address);
            _scanner.Scan("LEA_140b0fd01", "48 8D 05 ?? ?? ?? ?? F3 0F 7F 45 ?? 0F B6 44 ?? ??",
                address => _LEA_140b0fd01_Address = address);
            _scanner.Scan("LEA_140b10242", "48 8D 05 ?? ?? ?? ?? 0F B6 44 ?? ?? B9 18 01 00 00",
                address => _LEA_140b10242_Address = address);
            _scanner.Scan("LEA_140b6650b", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 80 0F 85 ?? ?? ?? ?? 41 83 7E ?? 02",
                address => _LEA_140b6650b_Address = address);
            _scanner.Scan("LEA_140b76766", "48 8D 0D ?? ?? ?? ?? BE 01 00 00 00",
                address => _LEA_140b76766_Address = address);
            _scanner.Scan("LEA_140b768bd", "48 8D 0D ?? ?? ?? ?? 4C 8B 75 ??",
                address => _LEA_140b768bd_Address = address);
            _scanner.Scan("LEA_140b768fa", "4C 8D 05 ?? ?? ?? ?? 43 F6 44 ?? ?? 40",
                address => _LEA_140b768fa_Address = address);
            _scanner.Scan("LEA_140b7691c", "4C 8D 05 ?? ?? ?? ?? 43 8B 44 ?? ??",
                address => _LEA_140b7691c_Address = address);
            _scanner.Scan("LEA_140b769ca", "48 8D 05 ?? ?? ?? ?? 42 23 5C ?? ?? 48 8B 45 ?? 0F BA F3 14 4C 89 44 24 ??",
                address => _LEA_140b769ca_Address = address);
            _scanner.Scan("LEA_140b76b0e", "4C 8D 05 ?? ?? ?? ?? 41 80 7C ?? ?? 02 0F 85 ?? ?? ?? ??",
                address => _LEA_140b76b0e_Address = address);
            _scanner.Scan("LEA_140b76c62", "4C 8D 05 ?? ?? ?? ?? 41 80 7C ?? ?? 02 75 ??",
                address => _LEA_140b76c62_Address = address);
            _scanner.Scan("LEA_140b76df8", "48 8D 0D ?? ?? ?? ?? 3B D6",
                address => _LEA_140b76df8_Address = address);
            _scanner.Scan("LEA_140d23a05", "4C 8D 3D ?? ?? ?? ?? 66 C1 ED 0C",
                address => _LEA_140d23a05_Address = address);
            _scanner.Scan("LEA_140d6b75a", "4C 8D 25 ?? ?? ?? ?? 66 3B F0",
                address => _LEA_140d6b75a_Address = address);
            _scanner.Scan("LEA_140d6b845", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 02 75 ?? F6 44 ?? ?? 01 74 ?? 66 45 89 3E",
                address => _LEA_140d6b845_Address = address);
            _scanner.Scan("LEA_140d6b8f5", "48 8D 05 ?? ?? ?? ?? 48 03 C8 F6 01 ??",
                address => _LEA_140d6b8f5_Address = address);
            _scanner.Scan("LEA_140d6b925", "4C 8D 25 ?? ?? ?? ?? 0F B7 75 ??",
                address => _LEA_140d6b925_Address = address);
            _scanner.Scan("LEA_140d6bd7d", "4C 8D 3D ?? ?? ?? ?? 48 8D 50 ??",
                address => _LEA_140d6bd7d_Address = address);
            _scanner.Scan("LEA_140d73e5e", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 02 0F 85 ?? ?? ?? ??",
                address => _LEA_140d73e5e_Address = address);
            _scanner.Scan("LEA_140dbaaa7", "48 8D 15 ?? ?? ?? ?? 0F B7 C5",
                address => _LEA_140dbaaa7_Address = address);
            _scanner.Scan("LEA_140dbac38", "4C 8D 05 ?? ?? ?? ?? 41 0F B7 CC",
                address => _LEA_140dbac38_Address = address);
            _scanner.Scan("LEA_140e0fb52", "48 8D 05 ?? ?? ?? ?? 48 03 E8 0F B6 4D ?? 83 E9 01 0F 84 ?? ?? ?? ?? 83 F9 01 0F 85 ?? ?? ?? ?? 66 41 83 F9 02",
                address => _LEA_140e0fb52_Address = address);
            _scanner.Scan("LEA_140e10232", "48 8D 05 ?? ?? ?? ?? 48 03 D0 F6 02 ??",
                address => _LEA_140e10232_Address = address);
            _scanner.Scan("MOV_140e1072a", "41 8B BC ?? ?? ?? ?? ?? 40 F6 C7 02",
                address => _MOV_140e1072a_Address = address);
            _scanner.Scan("LEA_140e1223f", "48 8D 05 ?? ?? ?? ?? 42 F6 04 ?? 02",
                address => _LEA_140e1223f_Address = address);
            _scanner.Scan("LEA_140e12590", "48 8D 05 ?? ?? ?? ?? 42 80 7C ?? ?? 00",
                address => _LEA_140e12590_Address = address);
            _scanner.Scan("TEST_140e12933", "43 F6 84 ?? ?? ?? ?? ?? 40",
                address => _TEST_140e12933_Address = address);
            _scanner.Scan("TEST_140e13177", "45 85 B4 ?? ?? ?? ?? ?? B8 0D 00 00 00",
                address => _TEST_140e13177_Address = address);
            _scanner.Scan("MOV_140e1344d", "41 8B 84 ?? ?? ?? ?? ?? D1 E8 40 84 C6",
                address => _MOV_140e1344d_Address = address);
            _scanner.Scan("LEA_140e139f1", "48 8D 05 ?? ?? ?? ?? 48 03 E8 0F B6 4D ?? 83 E9 01 0F 84 ?? ?? ?? ?? 83 F9 01 0F 85 ?? ?? ?? ?? 66 41 83 F8 02",
                address => _LEA_140e139f1_Address = address);
            _scanner.Scan("LEA_140e14347", "4C 8D 0D ?? ?? ?? ?? 43 80 7C ?? ?? 15",
                address => _LEA_140e14347_Address = address);
            _scanner.Scan("LEA_140e14502", "48 8D 05 ?? ?? ?? ?? 48 03 F8 8B 07",
                address => _LEA_140e14502_Address = address);
            _scanner.Scan("LEA_140e1460a", "48 8D 05 ?? ?? ?? ?? 48 03 D8 80 7B ?? ??",
                address => _LEA_140e1460a_Address = address);
            _scanner.Scan("LEA_140e1492a", "4C 8D 15 ?? ?? ?? ?? 43 80 7C ?? ?? 00",
                address => _LEA_140e1492a_Address = address);
            _scanner.Scan("LEA_140e1498c", "4C 8D 15 ?? ?? ?? ?? 44 8B 8C 24 ?? ?? ?? ??",
                address => _LEA_140e1498c_Address = address);
            _scanner.Scan("LEA_140e14b56", "4C 8D 2D ?? ?? ?? ?? 43 8B 44 ?? 00",
                address => _LEA_140e14b56_Address = address);
            _scanner.Scan("LEA_140e14dd5", "48 8D 05 ?? ?? ?? ?? 4C 03 CD",
                address => _LEA_140e14dd5_Address = address);
            _scanner.Scan("MOV_140e16e9e", "41 8B 8C ?? ?? ?? ?? ?? F6 C1 02",
                address => _MOV_140e16e9e_Address = address);
            _scanner.Scan("LEA_140e1b1ef", "48 8D 05 ?? ?? ?? ?? 80 7C ?? ?? 12",
                address => _LEA_140e1b1ef_Address = address);
            _scanner.Scan("LEA_140e1b2bd", "48 8D 05 ?? ?? ?? ?? 48 03 C8 8B 01 A8 02 74 ?? B2 01",
                address => _LEA_140e1b2bd_Address = address);
            _scanner.Scan("LEA_140e1b2f3", "48 8D 05 ?? ?? ?? ?? 0F B6 44 ?? ?? 66 0F 6E C0",
                address => _LEA_140e1b2f3_Address = address);
            _scanner.Scan("LEA_140e1b31b", "48 8D 05 ?? ?? ?? ?? 48 03 C8 8B 01 A8 02 75 ??",
                address => _LEA_140e1b31b_Address = address);
            _scanner.Scan("LEA_140e1b3b1", "48 8D 05 ?? ?? ?? ?? 48 03 C8 8B 01 A8 02 74 ?? 0F BA E0 09",
                address => _LEA_140e1b3b1_Address = address);
            _scanner.Scan("LEA_140e1c435", "48 8D 05 ?? ?? ?? ?? 48 03 C9 F6 04 ?? 02",
                address => _LEA_140e1c435_Address = address);
            _scanner.Scan("LEA_140e32ef3 and LEA_140e32f0a", "48 8D 05 ?? ?? ?? ?? 42 0F B6 44 ?? ?? 83 C0 F4 83 F8 01 0F 96 C0 C3 48 8D 05 ?? ?? ?? ??",
                address =>
                {
                    _LEA_140e32ef3_Address = address;
                    _LEA_140e32f0a_Address = address + 23;
                });
            _scanner.Scan("LEA_140e32f47", "48 8D 05 ?? ?? ?? ?? 33 DB 49 8B F8",
                address => _LEA_140e32f47_Address = address);
            _scanner.Scan("FUN_140e33180 for LEA_140e32f47", "0F B7 C1 48 8D 0C ?? 48 03 C9 48 8D 05 ?? ?? ?? ?? 0F B6 04 ?? 24 01",
                address => _LEA_140e3318a_Address = address + 10);
            _scanner.Scan("LEA_140e3330a", "48 8D 05 ?? ?? ?? ?? 80 7C ?? ?? 01",
                address => _LEA_140e3330a_Address = address);
            _scanner.Scan("FUN_140e33420 for LEA_140e3342a", "0F B7 C1 48 8D 0C ?? 48 03 C9 48 8D 05 ?? ?? ?? ?? 8B 04 ?? D1 E8",
                address => _LEA_140e3342a_Address = address + 10);
            _scanner.Scan("FUN_140e33440 for LEA_140e3344a", "0F B7 C1 48 8D 0C ?? 48 03 C9 48 8D 05 ?? ?? ?? ?? 8B 04 ?? C1 E8 11",
                address => _LEA_140e3344a_Address = address + 10);
            _scanner.Scan("MOV_140e3358a", "41 8B 84 ?? ?? ?? ?? ?? D1 E8 A8 01 0F 85 ?? ?? ?? ?? 8B D3",
                address => _MOV_140e3358a_Address = address);
            _scanner.Scan("MOV_140e33933", "8B 84 ?? ?? ?? ?? ?? D1 E8 A8 01 74 ??",
                address => _MOV_140e33933_Address = address);
            _scanner.Scan("MOV_140e34230", "8B 84 ?? ?? ?? ?? ?? D1 E8 84 C2",
                address => _MOV_140e34230_Address = address);
            _scanner.Scan("MOV_140e34438", "42 8B 84 ?? ?? ?? ?? ?? A8 01",
                address => _MOV_140e34438_Address = address);
            _scanner.Scan("MOV_140e34732", "41 8B 84 ?? ?? ?? ?? ?? A9 00 F0 00 00",
                address => _MOV_140e34732_Address = address);
            _scanner.Scan("MOV_140e34750", "41 8B 84 ?? ?? ?? ?? ?? 0F BA E0 0D",
                address => _MOV_140e34750_Address = address);
            _scanner.Scan("MOV_140e34769", "41 8B 84 ?? ?? ?? ?? ?? 0F BA E0 0E",
                address => _MOV_140e34769_Address = address);
            _scanner.Scan("MOV_140e34782", "41 8B 84 ?? ?? ?? ?? ?? 0F BA E0 0F",
                address => _MOV_140e34782_Address = address);
            _scanner.ScanForData("CALL_14071e0d8 for FUN_140e3eb50 for LEA_140e3eb53", "E8 ?? ?? ?? ?? F6 00 01 74 ?? 48 8D 54 24 ??", 5, 1, 0,
                address => _LEA_140e3eb53_Address = address + 3);
            _scanner.Scan("LEA_1417b4694", "48 8D 0D ?? ?? ?? ?? 80 7C ?? ?? 02",
                address => _LEA_1417b4694_Address = address);
            _scanner.Scan("LEA_14a8ab88f", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 80 0F 85 ?? ?? ?? ?? 48 8B 46 ??",
                address => _LEA_14a8ab88f_Address = address);
            _scanner.Scan("LEA_14bd93fea", "48 8D 05 ?? ?? ?? ?? 83 7C ?? ?? 00 75 ??",
                address => _LEA_14bd93fea_Address = address);


            /* _LEA_14bd93fea_Address
            _scanner.Scan("NAMEGE", "PATTERNGE",
                address => ADRESSGE = address);
                */
            // fsadfjsdjflksjfdokjsfdj
        }
    }

    private unsafe void FUN_140e3dc90_Custom(uint* param1)
    {
        Log.Information($"FUN_140e3dc90 called. Agidyne skill cost: {*(short*)((long)0x142226bf0 + 48 * 12 + 8)}");
        
        FUN_140e3dc90_Hook!.OriginalFunction(param1);
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
                var japaneseSkillBytes = (byte*)_DAT_142226bf0_Address;
                
                Buffer.MemoryCopy(japaneseSkillBytes, mySkillBytes, 800 * 48, 800 * 48);
                
                _customSkillDataOffset = (long)_pinnedSkillTblBytes.Pointer - (long)japaneseSkillBytes;
                
                Log.Information($"My pointer address: {(ulong)_pinnedSkillTblBytes.Pointer:X}, original pointer address: {(ulong)_DAT_142226bf0_Address:X}, difference: {_customSkillDataOffset}");
                
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
        
        ConstructHook(_TEST_140719e12_Address, CreateCustomInstructionSet([
            "test dword [r12 + rcx*0x8], 0x10000"
        ], "r12", false));
        
        ConstructHook(_LEA_14071bd02_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_MOV_14073a0ae_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        ConstructHook(_MOV_14073c18b_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        ConstructHook(_TEST_14073c1df_Address, CreateCustomInstructionSet([
            "test dword [r12 + rax*0x8], 0x10000"
        ], "r12", false));
        
        ConstructHook(_MOV_14073d50e_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        ConstructHook(_LEA_1407521d7_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140752496_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14075277d_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140752df5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140752f6b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140753697_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407536ca_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14075384d_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407538ad_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14075396b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140753af8_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140753c6a_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140753dfd_Address, [
            "use64",
            $"mov r13, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140753fe4_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407541de_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140754479_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14075e087_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140771dfc_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140771f54_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407724e7_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_TEST_14077b9eb_Address, CreateCustomInstructionSet([
            "test dword [r12 + rcx*0x8], 0x10000"
        ], "r12", false));
        
        ConstructHook(_MOV_14077c383_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        ConstructHook(_LEA_1407afc81_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407aff57_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407b076e_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407b40a0_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407b81fb_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1407b9128_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140801ee2_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140810877_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140811470_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14081162d_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14085e961_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140868c5e_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140868f94_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14086fbc4_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140870ad8_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140871e93_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_MOV_1408ec3c4_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        ConstructHook(_LEA_1409f0136_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140acc910_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140accb16_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140accf19_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140acd266_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140acd6f0_Address, [
            "use64",
            $"mov r14, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad431e_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad4414_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad4431_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad44d5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad4615_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad474b_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad47c4_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad48e6_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad55fa_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad5f65_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad653a_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad6671_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad707f_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad74cf_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad98cb_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140ad9b75_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140aedd10_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140aee4f6_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140aee8d0_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140aeec6a_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b0fd01_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b10242_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b6650b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b76766_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b768bd_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b768fa_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b7691c_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b769ca_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b76b0e_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b76c62_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140b76df8_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140d23a05_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140d6b75a_Address, [
            "use64",
            $"mov r12, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140d6b845_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140d6b8f5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140d6b925_Address, [
            "use64",
            $"mov r12, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140d6bd7d_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140d73e5e_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140dbaaa7_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140dbac38_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e0fb52_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e10232_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_MOV_140e1072a_Address, CreateCustomInstructionSet([
            "mov edi, dword [r12 + rdx*0x8]"
        ], "r12", false));
        
        ConstructHook(_LEA_140e1223f_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e12590_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_TEST_140e12933_Address, CreateCustomInstructionSet([
            "test byte [r12 + r9*0x1], 0x40"
        ], "r12", false));
        
        
        ConstructHook(_TEST_140e13177_Address, CreateCustomInstructionSet([
            "test dword [r15 + r12*0x1], r14d"
        ], "r12", false));
        
        ConstructHook(_MOV_140e1344d_Address, CreateCustomInstructionSet([
            "mov eax, dword [r15 + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_LEA_140e139f1_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e14347_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e14502_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1460a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1492a_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1498c_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e14b56_Address, [
            "use64",
            $"mov r13, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e14dd5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_MOV_140e16e9e_Address, CreateCustomInstructionSet([
            "mov ecx, dword [r12 + rax*0x8]"
        ], "r12", false));
        
        ConstructHook(_LEA_140e1b1ef_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1b2bd_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1b2f3_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1b31b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1b3b1_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e1c435_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e32ef3_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e32f0a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e32f47_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e3318a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e3330a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e3342a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_140e3344a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_MOV_140e3358a_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_MOV_140e33933_Address, CreateCustomInstructionSet([
            "mov eax, dword [rax + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_MOV_140e34230_Address, CreateCustomInstructionSet([
            "mov eax, dword [rax + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_MOV_140e34438_Address, CreateCustomInstructionSet([
            "mov eax, dword [rax + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_MOV_140e34732_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_MOV_140e34750_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_MOV_140e34769_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_MOV_140e34782_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        ConstructHook(_LEA_140e3eb53_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_1417b4694_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14a8ab88f_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        ConstructHook(_LEA_14bd93fea_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        // _LEA_14bd93fea_Address
        
        // fsadfjsdjflksjfdokjsfdj
    }
    
    private void ConstructHook(long address, string[] function)
    {
        _asmHooks.Add(_hooks!.CreateAsmHook(function, address, AsmHookBehaviour.DoNotExecuteOriginal).Activate());
    }

    private unsafe string[] CreateCustomInstructionSet(string[] customInstructions, string dataOffsetRegister, bool registerIsOffset = true)
    {
        _customInstructionSetList.Clear();
        
        _customInstructionSetList.AddRange([
            "use64",
            $"push {dataOffsetRegister}",
            registerIsOffset ? $"mov {dataOffsetRegister}, {_customSkillDataOffset}" : $"mov {dataOffsetRegister}, {(long)_pinnedSkillTblBytes.Pointer}"
        ]);
        
        _customInstructionSetList.AddRange(customInstructions);
        
        _customInstructionSetList.Add($"pop {dataOffsetRegister}");

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
using System.Diagnostics;
using System.Drawing;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Mod.Interfaces;
using SkillTableExpanded.Template;
using SkillTableExpanded.Configuration;
using Project.Utils;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Memory;
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


    private const uint SkillElementLength = 8;
    private const uint ActiveSkillDataLength = 48;
    
    private const uint VanillaSkillElementsLength = 1056;
    private const uint VanillaActiveSkillDataLength = 800;

    private const uint ExpandedSkillElementsLength = 2500;
    private const uint ExpandedActiveSkillDataLength = 2500;
    
    
    [Function(CallingConventions.Microsoft)]
    private unsafe delegate long FUN_140e39b70(long param1);
    private IHook<FUN_140e39b70>? FUN_140e39b70_Hook;
    
    // Segment 1
    private nint _DAT_142226bf0_Address; // Starting address of Segment 1 of SKILL.TBL in memory
    
    // For DAT_142226bf0
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
    
    // For DAT_142226bf4
    private nint _TEST_140764760_Address;
    private nint _TEST_140ad2e25_Address;
    private nint _LEA_140b110f8_Address;
    private nint _TEST_140b31f96_Address;
    private nint _TEST_140b32a54_Address;
    private nint _TEST_140b74509_Address;
    private nint _LEA_140e2bc7f_Address;
    private nint _LEA_14bc3c1d9_Address;
    
    // For DAT_142226bf6
    private nint _LEA_140101809_Address;
    private nint _LEA_1401018a9_Address;
    private nint _CMP_14077bd39_Address;
    private nint _CMP_14077f324_Address;
    private nint _CMP_14077ffe1_Address;
    private nint _CMP_140911e33_Address;
    private nint _CMP_1409257c3_Address;
    private nint _CMP_140911f4c_Address;
    private nint _CMP_1409258dc_Address;
    private nint _CMP_140950c7e_Address;
    private nint _LEA_140e3335c_Address;
    
    // For DAT_142226bf7
    private nint _MOVZX_14073aa19_Address;
    private nint _MOVZX_140ac6527_Address;
    private nint _MOVZX_140d07c3b_Address;
    private nint _MOVZX_140d0ac7c_Address;
    private nint _MOVZX_140e335c8_Address;
    private nint _LEA_14171e63b_Address;
    private nint _MOVZX_14177b5ae_Address;
    private nint _LEA_141780be8_Address;
    
    // For DAT_142226bfc
    private nint _CMP_14074211f_Address;
    private nint _CMP_140742760_Address;
    private nint _MOVZX_140770766_Address;
    private nint _MOVZX_140770911_Address;
    private nint _LEA_1407b8b36_Address;
    private nint _CMP_1407ba966_Address;
    private nint _MOVZX_1407bde19_Address;
    private nint _MOVZX_1407be616_Address;
    private nint _LEA_1407bf421_Address;
    private nint _LEA_1407c84a2_Address;
    private nint _LEA_1407e5a4e_Address;
    private nint _LEA_1407ecfc0_Address;
    private nint _LEA_1407ed288_Address;
    private nint _LEA_1407ed9e6_Address;
    private nint _LEA_1407edc4a_Address;
    private nint _LEA_1407ee1dd_Address;
    private nint _LEA_1407ee483_Address;
    private nint _LEA_1407efdb8_Address;
    private nint _LEA_1407f1a15_Address;
    private nint _LEA_1407f2964_Address;
    private nint _LEA_1407f32f4_Address;
    private nint _LEA_1407f5514_Address;
    private nint _LEA_1407f5a63_Address;
    private nint _LEA_1407f6223_Address;
    private nint _LEA_1407f6d53_Address;
    private nint _LEA_1407f83d3_Address;
    private nint _LEA_1407f9327_Address;
    private nint _LEA_1407f9ac3_Address;
    private nint _CMP_1407fa38f_Address;
    private nint _MOVZX_140800dbc_Address;
    private nint _MOVZX_140808efa_Address;
    private nint _MOVZX_140809ad7_Address;
    private nint _MOVZX_1409ec814_Address;
    private nint _CMP_140ac4cb6_Address;
    private nint _LEA_140acc3d0_Address;
    private nint _MOVZX_140ad3048_Address;
    private nint _MOVZX_140ad37ee_Address;
    private nint _LEA_140ada874_Address;
    private nint _LEA_140b5115b_Address;
    private nint _CMP_140b53c15_Address;
    private nint _LEA_140b7daa1_Address;
    private nint _LEA_140b7ec53_Address;
    private nint _LEA_140b82003_Address;
    private nint _MOVZX_140d23838_Address;
    private nint _LEA_140d73d5f_Address;
    private nint _LEA_140e3519a_Address;
    private nint _MOVZX_14123d8c3_Address;
    
    // For DAT_142226bfd
    private nint _MOVZX_1407ba194_Address;
    private nint _LEA_140ad5841_Address;
    private nint _LEA_140ad5bb6_Address;
    private nint _LEA_140b764d7_Address;
    private nint _LEA_14bd9429a_Address;
    
    // For DAT_142226bfe
    private nint _MOVZX_1407ba19d_Address;
    
    // For DAT_142226c00
    private nint _LEA_1407ba233_Address;
    
    // For DAT_142226c04
    private nint _CMP_140e12a8d_Address;
    private nint _CMP_140e12b6a_Address;
    
    // For DAT_142226c06
    private nint _MOVZX_1409f1a37_Address;
    
    // For DAT_142226c07
    private nint _MOVZX_1400fa0d5_Address;
    private nint _MOVZX_1407fac4b_Address;
    private nint _MOVZX_140883961_Address;
    private nint _MOVZX_14088476c_Address;
    private nint _CMP_1408a8c77_Address;
    private nint _MOVZX_1408eb238_Address;
    private nint _MOVZX_140911e42_Address;
    private nint _MOVZX_140911f5b_Address;
    private nint _MOVZX_1409257d2_Address;
    private nint _MOVZX_1409258eb_Address;
    private nint _MOVZX_1409ec823_Address;
    private nint _CMP_140e12342_Address;
    private nint _MOVZX_140e12850_Address;
    private nint _MOVZX_140e332ad_Address;
    private nint _MOVZX_140e3348d_Address;
    
    // For DAT_142226c0a
    private nint _MOVZX_1408eb24d_Address;
    private nint _MOVZX_140e332a2_Address;
    private nint _MOVZX_140e33482_Address;
    private nint _MOVZX_140e347d8_Address;
    
    // For DAT_142226c0e
    private nint _CMP_1400f9485_Address;
    private nint _CMP_1400fa0fb_Address;
    private nint _MOVZX_140e10715_Address;
    private nint _CMP_140e110c9_Address;
    private nint _MOVZX_140e11193_Address;
    private nint _CMP_140e11dee_Address;
    private nint _CMP_140e1234c_Address;
    private nint _CMP_140e1238f_Address;
    private nint _CMP_140e13023_Address;
    private nint _CMP_140e33a92_Address;
    private nint _CMP_140e33aee_Address;
    private nint _CMP_140e347c3_Address;
    
    // For DAT_142226c0f
    private nint _CMP_1400f948f_Address;
    private nint _CMP_1400fa10a_Address;
    private nint _CMP_140e11de0_Address;
    private nint _CMP_140e12399_Address;
    private nint _CMP_140e1302d_Address;
    private nint _CMP_140e347ce_Address;
    
    // For DAT_142226c10
    private nint _TEST_1400f9499_Address;
    private nint _TEST_1400fa119_Address;
    private nint _TEST_1407703a6_Address;
    private nint _LEA_14080e0c4_Address;
    private nint _TEST_1408a8c39_Address;
    private nint _MOV_140e110a9_Address;
    private nint _TEST_140e1235a_Address;
    private nint _TEST_140e123a3_Address;
    private nint _TEST_140e12a9b_Address;
    private nint _TEST_140e13037_Address;
    private nint _TEST_140e33af8_Address;
    
    // For DAT_142226c14
    private nint _TEST_1400fa0a0_Address;
    private nint _TEST_1400fa1da_Address;
    private nint _LEA_140ad7871_Address;
    private nint _MOV_140e33b47_Address;
    
    // For DAT_142226c18
    private nint _MOV_140e33b4e_Address;
    
    // For DAT_142226c1c
    private nint _MOVZX_1400fa087_Address;
    private nint _MOVZX_1400fa1c7_Address;
    private nint _CMP_1407ba25f_Address;
    private nint _TEST_1408a8c2f_Address;
    private nint _CMP_140ac516e_Address;
    private nint _CMP_140ac59fa_Address;
    private nint _CMP_140e104af_Address;
    private nint _CMP_140e106ea_Address;
    private nint _CMP_140e11083_Address;
    private nint _MOVZX_140e111c8_Address;
    private nint _MOVZX_140e12668_Address;
    private nint _CMP_140e12a2c_Address;
    private nint _CMP_140e33a71_Address;
    private nint _MOVZX_140e33cd4_Address;
    private nint _MOVZX_140e34851_Address;
    
    // For DAT_142226c1d
    private nint _MOVZX_140e4035c_Address;
    private nint _MOVZX_140e40427_Address;
    
    // Segment 0
    private nint _DAT_142260b80_Address; // Starting address of Segment 0 of SKILL.TBL in memory
    
    // For DATx142260b80
    private nint _LEA_14071a1ce_Address;
    private nint _LEA_14071a445_Address;
    private nint _LEA_14073acb7_Address;
    private nint _CMP_14073ad65_Address;
    private nint _LEA_14073ae82_Address;
    private nint _CMP_14073b083_Address;
    private nint _LEA_14073b110_Address;
    private nint _CMP_14076476f_Address;
    private nint _MOVSX_14076487d_Address;
    private nint _LEA_1407b9f92_Address;
    private nint _MOVSX_1407fb6f3_Address;
    private nint _MOVSX_1407fb805_Address;
    private nint _MOVSX_1407fdee4_Address;
    private nint _MOVSX_1407fdf5a_Address;
    private nint _LEA_14080095e_Address;
    private nint _LEA_140801ef4_Address;
    private nint _MOVSX_140807419_Address;
    private nint _MOVSX_140808e2a_Address;
    private nint _LEA_140815aab_Address;
    private nint _MOVSX_1408eb262_Address;
    private nint _CMP_140911f79_Address;
    private nint _CMP_140925909_Address;
    private nint _LEA_140911fb8_Address;
    private nint _LEA_140925948_Address;
    private nint _MOVSX_140ac50f3_Address;
    private nint _MOVSX_140ac5983_Address;
    private nint _LEA_140ad202a_Address;
    private nint _LEA_140ad241e_Address;
    private nint _LEA_140ad2745_Address;
    private nint _LEA_140b31af6_Address;
    private nint _LEA_140b31fa5_Address;
    private nint _LEA_140b325d8_Address;
    private nint _LEA_140b32a62_Address;
    private nint _CMP_140b7469f_Address;
    private nint _MOVSX_140b748c0_Address;
    private nint _MOVSX_140b74973_Address;
    private nint _CMP_140d71d6a_Address;
    private nint _CMP_140d72d0c_Address;
    private nint _CMP_140d73094_Address;
    private nint _CMP_140e16eab_Address;
    private nint _MOVSX_140e16f21_Address;
    private nint _LEA_140e16f74_Address;
    private nint _LEA_140e32879_Address;
    private nint _LEA_140e32cd6_Address;
    private nint _LEA_140e33076_Address;
    private nint _LEA_140e331d6_Address;
    private nint _LEA_140e3eb73_Address;
    private nint _LEA_140e4051a_Address;
    private nint _LEA_140fae067_Address;
    private nint _MOVSX_140fb00f7_Address;
    private nint _MOVSX_140fb0317_Address;
    private nint _MOVSX_140fb0c79_Address;
    private nint _MOVSX_140fb0e20_Address;
    private nint _LEA_140fb3c8f_Address;
    private nint _LEA_140fb8094_Address;
    private nint _LEA_141780647_Address;
    
    // For DATx142260b81
    private nint _CMP_14077033f_Address;
    private nint _CMP_140883791_Address;
    private nint _CMP_14088459c_Address;
    private nint _CMP_1409ec802_Address;
    private nint _CMP_140d07c26_Address;
    private nint _CMP_140d0ac66_Address;
    private nint _LEA_140e35393_Address;
    private nint _CMP_14177b599_Address;
    
    // For DATx142260b82
    private nint _MOVZX_140fa4eba_Address;
    private nint _CMP_140fa4ec6_Address;
    private nint _LEA_140faf2c4_Address;
    private nint _LEA_140faf7db_Address;
    private nint _MOVZX_140fb0100_Address;
    private nint _MOVZX_140fb031f_Address;
    private nint _MOVZX_140fb0c82_Address;
    private nint _MOVZX_140fb0e30_Address;
    private nint _LEA_140fb3803_Address;
    
    // Funny literal replacements
    private nint _LiteralReplacement_140fa4eda_Address;
    private nint _LiteralReplacement_140faf7ed_Address;
    private nint _LiteralReplacement_140fb3814_Address;
    private nint _LiteralReplacement_140fb3d43_Address;
    private nint _LiteralReplacement_14076ff34_Address;
    private nint _LiteralReplacement_14073b876_Address;
    private nint _LiteralReplacement_14073b458_Address;
    private nint _LiteralReplacement_14073c1c6_Address;
    private nint _LiteralReplacement_14076472e_Address;
    private nint _LiteralReplacement_1407b9f6c_Address;
    private nint _LiteralReplacement_14080093d_Address;
    private nint _LiteralReplacement_140801ec9_Address;
    private nint _LiteralReplacement_140802097_Address;
    private nint _LiteralReplacement_140800b29_Address;
    private nint _LiteralReplacement_1407ba0c1_Address;
    private nint _LiteralReplacement_140764905_Address;
    private nint _LiteralReplacement_1408073f8_Address;
    private nint _LiteralReplacement_140808dab_Address;
    private nint _LiteralReplacement_14080988e_Address;
    private nint _LiteralReplacement_140b31f61_Address;
    private nint _LiteralReplacement_140b3241f_Address;
    private nint _LiteralReplacement_140b32a23_Address;
    private nint _LiteralReplacement_140b32ec1_Address;
    private nint _LiteralReplacement_140b744d1_Address;
    private nint _LiteralReplacement_140b74c2e_Address;
    private nint _LiteralReplacement_140e16e8b_Address;
    private nint _LiteralReplacement_140fb3d45_Address;
    private nint _LiteralReplacement_14073e903_Address;
    private nint _LiteralReplacement_14073d3f4_Address;
    private nint _LiteralReplacement_1409f00b6_Address;
    private nint _LiteralReplacement_140ad2dee_Address;
    private nint _LiteralReplacement_140ad3b24_Address;
    private nint _LiteralReplacement_140ad98bf_Address;
    private nint _LiteralReplacement_140ad9b6b_Address;
    private nint _LiteralReplacement_140aedd0a_Address;
    private nint _LiteralReplacement_140b110f2_Address;
    private nint _LiteralReplacement_140d23a21_Address;
    private nint _LiteralReplacement_140d23a57_Address;
    private nint _LiteralReplacement_140d23af6_Address;
    private nint _LiteralReplacement_140d6b831_Address;
    private nint _LiteralReplacement_140d6b8e0_Address;
    private nint _LiteralReplacement_140d6b9be_Address;
    private nint _LiteralReplacement_140d6baaa_Address;
    private nint _LiteralReplacement_140d6bd94_Address;
    private nint _LiteralReplacement_140d73e36_Address;
    private nint _LiteralReplacement_140e2bc3b_Address;
    private nint _LiteralReplacement_140e2bc68_Address;
    private nint _LiteralReplacement_14bc3c1c2_Address;


    private ExternalMemory _memory;
    
    private readonly ScannerWrapper _scanner;

    private readonly List<string> _customInstructionSetList = new();
    private readonly List<IAsmHook> _asmHooks = new();
    
    private readonly Pinnable<byte> _pinnedSkillElements = new(new byte[ExpandedSkillElementsLength * SkillElementLength]);
    private long _skillElementsOffset;
    
    private readonly Pinnable<byte> _pinnedActiveSkillData = new(new byte[ExpandedActiveSkillDataLength * ActiveSkillDataLength]);
    private long _activeSkillDataOffset;

    private long _customInstructionSetPointer;
    private long _customInstructionSetOffset;

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
            // Segment 1
            _scanner.ScanForData("DAT_142226bf0", "48 8D 05 ?? ?? ?? ?? 4C 03 CD", 7, 3, 0,
                address => _DAT_142226bf0_Address = address);
            
            // For DAT_142226bf0
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
            
            // For DAT_142226bf4
            _scanner.Scan("TEST_140764760", "41 F6 84 ?? ?? ?? ?? ?? 02 0F 84 ?? ?? ?? ?? 41 80 BC ?? ?? ?? ?? ?? 0A",
                address => _TEST_140764760_Address = address);
            _scanner.Scan("TEST_140ad2e25", "42 F6 84 ?? ?? ?? ?? ?? 02 0F 84 ?? ?? ?? ?? 0F 57 C0",
                address => _TEST_140ad2e25_Address = address);
            _scanner.Scan("LEA_140b110f8", "4C 8D 1D ?? ?? ?? ?? 90 8B CA",
                address => _LEA_140b110f8_Address = address);
            _scanner.Scan("TEST_140b31f96", "42 F6 84 ?? ?? ?? ?? ?? 02 0F 84 ?? ?? ?? ?? 4D 8D B9 ?? ?? ?? ??",
                address => _TEST_140b31f96_Address = address);
            _scanner.Scan("TEST_140b32a54", "F6 84 ?? ?? ?? ?? ?? 02 0F 84 ?? ?? ?? ?? 4C 8D B2 ?? ?? ?? ??",
                address => _TEST_140b32a54_Address = address);
            _scanner.Scan("TEST_140b74509", "42 F6 84 ?? ?? ?? ?? ?? 02 0F 84 ?? ?? ?? ?? 48 8B 05 ?? ?? ?? ??",
                address => _TEST_140b74509_Address = address);
            _scanner.Scan("LEA_140e2bc7f", "48 8D 0D ?? ?? ?? ?? F6 04 ?? 01 75 ??",
                address => _LEA_140e2bc7f_Address = address);
            _scanner.Scan("LEA_14bc3c1d9", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 02 74 ??",
                address => _LEA_14bc3c1d9_Address = address);
            
            // For DAT_142226bf6
            _scanner.ScanMany("LEA_140101809 and LEA_1401018a9", "48 8D 05 ?? ?? ?? ?? 80 3C ?? 01 74 ?? 41 B8 03 00 00 00 EB ?? 41 B8 02 00 00 00 EB ?? 41 B8 04 00 00 00 45 33 C9 48 8B D6 48 8B CF 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 83 C4 20 5F E9 ?? ?? ?? ??",
                2, addresses =>
                {
                    _LEA_140101809_Address = addresses[0];
                    _LEA_1401018a9_Address = addresses[1];
                });
            _scanner.Scan("CMP_14077bd39", "80 BC ?? ?? ?? ?? ?? 01 0F 95 C3",
                address => _CMP_14077bd39_Address = address);
            _scanner.Scan("CMP_14077f324", "80 BC ?? ?? ?? ?? ?? 01 40 0F 94 C7 48 8B 4C 24 ?? 48 8B 45 ??",
                address => _CMP_14077f324_Address = address);
            _scanner.Scan("CMP_14077ffe1", "80 BC ?? ?? ?? ?? ?? 01 40 0F 94 C7 48 8D 4C 24 ??",
                address => _CMP_14077ffe1_Address = address);
            _scanner.ScanMany("CMP_140911e33 and CMP_1409257c3", "43 80 BC ?? ?? ?? ?? ?? 01 0F 85 ?? ?? ?? ?? 43 0F B6 84 ?? ?? ?? ?? ?? 3C 0A 0F 87 ?? ?? ?? ?? B9 52 05 00 00 0F A3 C1 0F 83 ?? ?? ?? ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 0F 85 ?? ?? ?? ?? 48 85 C0 0F 85 ?? ?? ?? ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 55 ?? 48 89 75 ?? E9 ?? ?? ?? ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 55 ?? 48 89 75 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CE 48 8B D6 48 89 4D ?? 48 89 55 ?? 48 8D 05 ?? ?? ?? ?? 48 89 45 ?? 48 85 D2 75 ?? 48 85 C9 0F 84 ?? ?? ?? ?? 48 89 51 ?? 48 89 75 ?? 48 89 75 ?? E9 ?? ?? ?? ?? 48 89 4A ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? 48 89 75 ?? 48 89 75 ?? E9 ?? ?? ?? ?? 41 83 78 ?? 01 0F 85 ?? ?? ?? ?? 41 0F B7 40 ?? 48 8D 0C ?? 48 03 C9 41 80 BC ?? ?? ?? ?? ?? 02 0F 85 ?? ?? ?? ?? 41 0F B6 84 ?? ?? ?? ?? ?? E9 ?? ?? ?? ?? 41 83 78 ?? 01 0F 85 ?? ?? ?? ?? 41 0F B7 40 ?? 41 80 BC ?? ?? ?? ?? ?? 15 E9 ?? ?? ?? ?? 45 8B 48 ?? 41 8D 41 ?? 83 F8 01 0F 87 ?? ?? ?? ?? 41 8B 40 ?? 41 83 F9 02 75 ?? 0F B7 C8 E8 ?? ?? ?? ?? 4C 8B 45 ?? 48 8B 55 ?? 0F B7 C0 0F B7 C0 49 8D 8E ?? ?? ?? ?? 48 8D 0C ?? 48 85 C9 0F 84 ?? ?? ?? ?? 80 39 ?? 0F 85 ?? ?? ?? ?? 48 89 5D ?? 48 85 D2 75 ?? 48 8B 4D ?? 48 85 C9 75 ?? 4D 85 C0 0F 84 ?? ?? ?? ?? EB ?? 48 8B 45 ?? 48 89 42 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? EB ?? 41 83 78 ?? 04 EB ?? 41 83 78 ?? 07 EB ?? 41 83 78 ?? 02 0F 85 ?? ?? ?? ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 55 ?? 48 89 75 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CE 48 8B D6 48 89 4D ?? 48 89 55 ?? 48 8D 05 ?? ?? ?? ?? 48 89 45 ?? 48 85 D2 75 ?? 48 85 C9 74 ?? EB ?? 48 89 4A ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? 48 89 75 ?? 48 89 75 ?? 48 8B CF E8 ?? ?? ?? ?? B0 01 EB ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 4D 85 C0 74 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? 48 89 48 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4D ?? 48 89 48 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CF E8 ?? ?? ?? ?? 32 C0 4C 8B 74 24 ?? 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 40 5D C3",
                2, addresses =>
                {
                    _CMP_140911e33_Address = addresses[0];
                    _CMP_1409257c3_Address = addresses[1];
                });
            _scanner.ScanMany("CMP_140911f4c and CMP_1409258dc", "41 80 BC ?? ?? ?? ?? ?? 02 0F 85 ?? ?? ?? ?? 41 0F B6 84 ?? ?? ?? ?? ?? E9 ?? ?? ?? ?? 41 83 78 ?? 01 0F 85 ?? ?? ?? ?? 41 0F B7 40 ?? 41 80 BC ?? ?? ?? ?? ?? 15 E9 ?? ?? ?? ?? 45 8B 48 ?? 41 8D 41 ?? 83 F8 01 0F 87 ?? ?? ?? ?? 41 8B 40 ?? 41 83 F9 02 75 ?? 0F B7 C8 E8 ?? ?? ?? ?? 4C 8B 45 ?? 48 8B 55 ?? 0F B7 C0 0F B7 C0 49 8D 8E ?? ?? ?? ?? 48 8D 0C ?? 48 85 C9 0F 84 ?? ?? ?? ?? 80 39 ?? 0F 85 ?? ?? ?? ?? 48 89 5D ?? 48 85 D2 75 ?? 48 8B 4D ?? 48 85 C9 75 ?? 4D 85 C0 0F 84 ?? ?? ?? ?? EB ?? 48 8B 45 ?? 48 89 42 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? EB ?? 41 83 78 ?? 04 EB ?? 41 83 78 ?? 07 EB ?? 41 83 78 ?? 02 0F 85 ?? ?? ?? ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 55 ?? 48 89 75 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CE 48 8B D6 48 89 4D ?? 48 89 55 ?? 48 8D 05 ?? ?? ?? ?? 48 89 45 ?? 48 85 D2 75 ?? 48 85 C9 74 ?? EB ?? 48 89 4A ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? 48 89 75 ?? 48 89 75 ?? 48 8B CF E8 ?? ?? ?? ?? B0 01 EB ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 4D 85 C0 74 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? 48 89 48 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4D ?? 48 89 48 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CF E8 ?? ?? ?? ?? 32 C0 4C 8B 74 24 ?? 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 40 5D C3",
                2, addresses =>
                {
                    _CMP_140911f4c_Address = addresses[0];
                    _CMP_1409258dc_Address = addresses[1];
                });
            _scanner.Scan("CMP_140950c7e", "41 80 BC ?? ?? ?? ?? ?? 01 40 0F 94 C7",
                address => _CMP_140950c7e_Address = address);
            _scanner.Scan("LEA_140e3335c", "48 8D 05 ?? ?? ?? ?? 80 3C ?? 01 74 ?? 32 C0",
                address => _LEA_140e3335c_Address = address);
            
            // For DAT_142226bf7
            _scanner.Scan("MOVZX_14073aa19", "0F B6 9C ?? ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 8D 4D ??",
                address => _MOVZX_14073aa19_Address = address);
            _scanner.Scan("MOVZX_140ac6527", "0F B6 BC ?? ?? ?? ?? ?? 48 8D 8D ?? ?? ?? ??",
                address => _MOVZX_140ac6527_Address = address);
            _scanner.Scan("MOVZX_140d07c3b", "0F B6 8C ?? ?? ?? ?? ?? 83 E9 01 74 ?? 83 F9 01 0F 85 ?? ?? ?? ?? 41 BE 1C 01 00 00",
                address => _MOVZX_140d07c3b_Address = address);
            _scanner.Scan("MOVZX_140d0ac7c", "41 0F B6 8C ?? ?? ?? ?? ?? 83 E9 01",
                address => _MOVZX_140d0ac7c_Address = address);
            _scanner.Scan("MOVZX_140e335c8", "41 0F B6 84 ?? ?? ?? ?? ?? 3C 01",
                address => _MOVZX_140e335c8_Address = address);
            _scanner.Scan("LEA_14171e63b", "48 8D 05 ?? ?? ?? ?? 44 0F B6 04 ?? 41 83 E8 01",
                address => _LEA_14171e63b_Address = address);
            _scanner.Scan("MOVZX_14177b5ae", "0F B6 8C ?? ?? ?? ?? ?? 83 E9 01 74 ?? 83 F9 01 0F 85 ?? ?? ?? ?? BF 1C 01 00 00 41 0F B7 C6",
                address => _MOVZX_14177b5ae_Address = address);
            _scanner.Scan("LEA_141780be8", "48 8D 05 ?? ?? ?? ?? 0F B6 14 ?? 83 EA 01",
                address => _LEA_141780be8_Address = address);
            
            // For DAT_142226bfc
            _scanner.Scan("CMP_14074211f", "41 80 BC ?? ?? ?? ?? ?? 00 0F 84 ?? ?? ?? ?? 48 8B 5C 24 ??",
                address => _CMP_14074211f_Address = address);
            _scanner.Scan("CMP_140742760", "41 80 BC ?? ?? ?? ?? ?? 00 0F 84 ?? ?? ?? ?? 48 8B 5D ??",
                address => _CMP_140742760_Address = address);
            _scanner.Scan("MOVZX_140770766", "0F B6 84 ?? ?? ?? ?? ?? 48 8D 4D 00",
                address => _MOVZX_140770766_Address = address);
            _scanner.Scan("MOVZX_140770911", "44 0F B6 A4 ?? ?? ?? ?? ?? 48 8D 4D ??",
                address => _MOVZX_140770911_Address = address);
            _scanner.Scan("LEA_1407b8b36", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 03 0F 84 ?? ?? ?? ?? 48 8B 5D ?? 48 85 DB 0F 84 ?? ?? ?? ?? 0F 1F 40 00 0F 1F 84 ?? 00 00 00 00",
                address => _LEA_1407b8b36_Address = address);
            _scanner.Scan("CMP_1407ba966", "40 38 BC ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 4C 8B 65 ??",
                address => _CMP_1407ba966_Address = address);
            _scanner.Scan("MOVZX_1407bde19", "41 0F B6 84 ?? ?? ?? ?? ?? 84 C0 0F 85 ?? ?? ?? ?? 48 8B 45 ??",
                address => _MOVZX_1407bde19_Address = address);
            _scanner.Scan("MOVZX_1407be616", "41 0F B6 84 ?? ?? ?? ?? ?? 84 C0 0F 85 ?? ?? ?? ?? 0F 57 C0",
                address => _MOVZX_1407be616_Address = address);
            _scanner.Scan("LEA_1407bf421", "41 0F B6 84 ?? ?? ?? ?? ?? 84 C0 0F 85 ?? ?? ?? ?? 0F 57 C0",
                address => _LEA_1407bf421_Address = address);
            _scanner.Scan("LEA_1407c84a2", "48 8D 05 ?? ?? ?? ?? 80 3C ?? 00 0F 84 ?? ?? ?? ??",
                address => _LEA_1407c84a2_Address = address);
            _scanner.Scan("LEA_1407e5a4e", "48 8D 05 ?? ?? ?? ?? 0F B6 1C ?? 48 8B CF E8 ?? ?? ?? ?? 48 8B 74 24 ??",
                address => _LEA_1407e5a4e_Address = address);
            _scanner.Scan("LEA_1407ecfc0", "48 8D 1D ?? ?? ?? ?? 41 B8 FF 0F 00 00",
                address => _LEA_1407ecfc0_Address = address);
            _scanner.Scan("LEA_1407ed288", "48 8D 1D ?? ?? ?? ?? 2B 45 ??",
                address => _LEA_1407ed288_Address = address);
            _scanner.Scan("LEA_1407ed9e6", "4C 8D 05 ?? ?? ?? ?? 0F B7 F3",
                address => _LEA_1407ed9e6_Address = address);
            _scanner.Scan("LEA_1407edc4a", "4C 8D 05 ?? ?? ?? ?? 2B C3",
                address => _LEA_1407edc4a_Address = address);
            _scanner.Scan("LEA_1407ee1dd", "48 8D 15 ?? ?? ?? ?? 44 0F B7 FF",
                address => _LEA_1407ee1dd_Address = address);
            _scanner.Scan("LEA_1407ee483", "48 8D 15 ?? ?? ?? ?? 48 83 C6 02 49 83 EF 01 0F 85 ?? ?? ?? ?? 4C 8B 75 ??",
                address => _LEA_1407ee483_Address = address);
            _scanner.Scan("LEA_1407efdb8", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 03 0F 84 ?? ?? ?? ?? 48 8B CB",
                address => _LEA_1407efdb8_Address = address);
            _scanner.Scan("LEA_1407f1a15", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 03 0F 84 ?? ?? ?? ?? 49 8D 4D ??",
                address => _LEA_1407f1a15_Address = address);
            _scanner.ScanMany("LEA_1407f2964 and LEA_1407f32f4", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 03 0F 84 ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 4D 85 E4 0F 84 ?? ?? ?? ?? 45 33 ED 48 8D 35 ?? ?? ?? ?? 49 8D 4C 24 ?? 0F 57 C0 4D 8B 24 24 F3 0F 7F 44 24 ?? 48 89 74 24 ?? 4C 89 6C 24 ?? 48 8B 41 ?? 48 85 C0 74 ?? 48 89 44 24 ?? 48 8D 54 24 ?? 48 89 4C 24 ?? 48 8B 41 ?? 48 89 50 ?? 48 8D 44 24 ?? 48 89 41 ?? EB ?? 48 8D 44 24 ?? 48 89 41 ?? 48 89 4C 24 ?? 48 8B 44 24 ?? 48 8B 51 ?? 48 89 54 24 ?? 4C 89 6D ?? 48 89 75 ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 8B 54 24 ?? 48 89 44 24 ?? EB ?? 48 8D 45 ?? 48 89 44 24 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 89 55 ?? 48 8B CF 48 8D 55 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8B 4C 24 ?? 48 8B 44 24 ?? 48 89 74 24 ?? 48 85 C9 75 ?? 48 85 C0 75 ?? 48 8B 4C 24 ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 48 8B 4C 24 ?? 48 8B 44 24 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 44 24 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4D 85 E4 0F 85 ?? ?? ?? ?? 48 8D 3D ?? ?? ?? ?? 48 8D 4D ?? 48 89 7D ?? E8 ?? ?? ?? ?? 48 8D 4D ?? 48 89 7D ?? E8 ?? ?? ?? ?? 49 8B CE E8 ?? ?? ?? ?? 85 DB 0F 95 C0 E9 ?? ?? ?? ?? 48 8B 55 ?? 4C 8D 25 ?? ?? ?? ?? 0F 57 C0 4C 89 65 ?? 45 33 ED 48 8D 4D ?? F3 0F 7F 45 ?? 4C 89 6D ?? E8 ?? ?? ?? ?? 48 8D 55 ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 84 C0 75 ?? 85 DB 0F 84 ?? ?? ?? ?? 8B 15 ?? ?? ?? ?? 81 FA 70 02 00 00 7C ?? E8 ?? ?? ?? ?? 41 8B D5 48 8B 05 ?? ?? ?? ?? 48 63 CA FF C2 89 15 ?? ?? ?? ?? 33 D2 8B 04 ?? F7 F3 85 D2 74 ?? 8B C2 0F 1F 44 ?? 00 4D 8B 3F 48 83 E8 01 75 ?? 0F 57 C0 4C 89 65 ?? 49 8D 57 ?? 4C 89 6D ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8D 55 ?? 48 8B CF E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8D 3D ?? ?? ?? ?? 48 8D 4D ?? 48 89 7D ?? E8 ?? ?? ?? ?? 48 8D 4D ?? 48 89 7D ?? E8 ?? ?? ?? ?? 49 8B CE E8 ?? ?? ?? ?? B0 01 EB ?? 0F 57 C0 4C 89 65 ?? 49 8B D6 4C 89 6D ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 8B D6 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8D 3D ?? ?? ?? ?? 48 8D 4D ?? 48 89 7D ?? E8 ?? ?? ?? ?? 48 8D 4D ?? 48 89 7D ?? E8 ?? ?? ?? ?? 49 8B CE E8 ?? ?? ?? ?? 32 C0 48 81 C4 18 01 00 00 41 5F 41 5E 41 5D 41 5C 5F 5E 5B 5D C3",
                2, addresses =>
                {
                    _LEA_1407f2964_Address = addresses[0];
                    _LEA_1407f32f4_Address = addresses[1];
                });
            _scanner.Scan("LEA_1407f5514", "48 8D 05 ?? ?? ?? ?? F6 04 ?? 03 0F 84 ?? ?? ?? ?? 48 8B 5D ?? 48 85 DB 0F 84 ?? ?? ?? ?? 0F 1F 40 00 66 66 0F 1F 84 ?? 00 00 00 00",
                address => _LEA_1407f5514_Address = address);
            _scanner.ScanMany("LEA_1407f5a63 and LEA_1407f6d53", "48 8D 05 ?? ?? ?? ?? 44 38 24 ?? 0F 84 ?? ?? ?? ?? 48 8B 5D ?? 48 85 DB 0F 84 ?? ?? ?? ?? 48 8D 7B ?? 48 8B 1B 48 8D 44 24 ?? 48 3B F8 74 ?? 48 8B 45 ?? 48 39 47 ?? 74 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? EB ?? 48 8B 4C 24 ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 64 24 ?? 48 8B D7 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 45 ?? 4C 89 65 ?? 4C 89 6D ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 89 45 ?? EB ?? 48 8D 45 ?? 48 89 45 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 8B 45 ?? 48 8D 55 ?? 49 8B CE 48 89 45 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 85 DB 0F 85 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 4C 8B 44 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 4D 85 C0 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 41 8D 50 ?? FF 10 48 8B 45 ?? 4C 8B 44 24 ?? 4C 89 65 ?? EB ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 74 ?? 4C 8B 44 24 ?? 4C 89 40 ?? 49 8B C4 4C 89 64 24 ?? 48 89 45 ?? 4D 8B C4 48 89 74 24 ?? 4D 85 C0 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 8B 44 24 ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 0F 57 C0 4C 89 6D ?? 48 8D 53 ?? 4C 89 65 ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8D 55 ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 54 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 48 8B 45 ?? 48 8B 54 24 ?? 4C 89 65 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 49 8B C4 49 8B D4 48 89 45 ?? 48 89 54 24 ?? 48 89 74 24 ?? 48 85 D2 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 8B 7D ?? BE 00 00 00 80 48 85 FF 0F 84 ?? ?? ?? ?? 4C 8D 35 ?? ?? ?? ?? 48 8D 5F ?? 48 8B 3F 48 8D 44 24 ?? 48 3B D8 74 ?? 48 8B 45 ?? 48 39 43 ?? 74 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? EB ?? 48 8B 4C 24 ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 64 24 ?? 48 8B D3 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 45 ?? 4C 89 65 ?? 4C 89 6D ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 89 45 ?? EB ?? 48 8D 45 ?? 48 89 45 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 8B 45 ?? 48 8D 4D ?? 0F 57 C0 48 89 45 ?? 49 8B D7 4C 89 6D ?? F3 0F 7F 45 ?? 4C 89 65 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 49 8B 57 ?? 48 8D 4D ?? 0F 57 C0 4C 89 75 ?? 48 83 C2 40 4C 89 65 ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8B 55 ?? 4C 8D 0D ?? ?? ?? ?? 0F 57 C0 4C 89 4D ?? 48 83 C2 18 4C 89 65 ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8B 4D ?? 48 8B 55 ?? 48 8B 41 ?? 8B 58 ?? 48 8B 45 ?? 4C 89 4D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 01 BA 01 00 00 00 FF 10 48 8B 45 ?? 48 8B 4D ?? 4C 89 65 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 49 8B C4 49 8B CC 48 89 45 ?? 48 89 4D ?? 48 8D 15 ?? ?? ?? ?? 48 89 55 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4D ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 65 ?? 48 8D 4D ?? 4C 89 75 ?? E8 ?? ?? ?? ?? 48 8B 4D ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4D ?? 48 89 48 ?? 3B DE 7E ?? 48 8B 45 ?? 48 8D 4D ?? 48 39 45 ?? 74 ?? E8 ?? ?? ?? ?? EB ?? E8 ?? ?? ?? ?? 48 8D 54 24 ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 8B F3 48 85 FF 0F 85 ?? ?? ?? ?? 4C 8B 75 ?? 0F 57 C0 4C 89 6D ?? 48 8D 55 ?? 4C 89 65 ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8D 55 ?? 49 8B CE E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8D 05 ?? ?? ?? ?? 48 8D 4D ?? 48 89 45 ?? E8 ?? ?? ?? ?? 49 8B CF E8 ?? ?? ?? ?? 4C 8D 9C 24 ?? ?? ?? ?? B0 01 49 8B 5B ?? 49 8B 73 ?? 49 8B 7B ?? 49 8B E3 41 5F 41 5E 41 5D 41 5C 5D C3",
                2, addresses =>
                {
                    _LEA_1407f5a63_Address = addresses[0];
                    _LEA_1407f6d53_Address = addresses[1];
                });
            _scanner.Scan("LEA_1407f6223", "48 8D 05 ?? ?? ?? ?? 44 38 24 ?? 0F 84 ?? ?? ?? ?? 48 8B 5D ?? 48 85 DB 0F 84 ?? ?? ?? ?? 48 8D 7B ?? 48 8B 1B 48 8D 44 24 ?? 48 3B F8 74 ?? 48 8B 45 ?? 48 39 47 ?? 74 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? EB ?? 48 8B 4C 24 ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 64 24 ?? 48 8B D7 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 45 ?? 4C 89 65 ?? 4C 89 6D ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 89 45 ?? EB ?? 48 8D 45 ?? 48 89 45 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 8B 45 ?? 48 8D 55 ?? 49 8B CE 48 89 45 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 85 DB 0F 85 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 4C 8B 44 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 4D 85 C0 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 41 8D 50 ?? FF 10 48 8B 45 ?? 4C 8B 44 24 ?? 4C 89 65 ?? EB ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 74 ?? 4C 8B 44 24 ?? 4C 89 40 ?? 49 8B C4 4C 89 64 24 ?? 48 89 45 ?? 4D 8B C4 48 89 74 24 ?? 4D 85 C0 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 8B 44 24 ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 0F 57 C0 4C 89 6D ?? 48 8D 53 ?? 4C 89 65 ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8D 55 ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 54 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 48 8B 45 ?? 48 8B 54 24 ?? 4C 89 65 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 49 8B C4 49 8B D4 48 89 45 ?? 48 89 54 24 ?? 48 89 74 24 ?? 48 85 D2 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 8B 7D ?? BE FF FF FF 7F 48 85 FF 0F 84 ?? ?? ?? ?? 4C 8D 35 ?? ?? ?? ?? 48 8D 5F ?? 48 8B 3F 48 8D 44 24 ?? 48 3B D8 74 ?? 48 8B 45 ?? 48 39 43 ?? 74 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? EB ?? 48 8B 4C 24 ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 64 24 ?? 48 8B D3 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 45 ?? 4C 89 65 ?? 4C 89 6D ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 89 45 ?? EB ?? 48 8D 45 ?? 48 89 45 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 8B 45 ?? 48 8D 4D ?? 0F 57 C0 48 89 45 ?? 49 8B D7 4C 89 6D ?? F3 0F 7F 45 ?? 4C 89 65 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 49 8B 57 ?? 48 8D 4D ?? 0F 57 C0 4C 89 75 ?? 48 83 C2 40 4C 89 65 ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8B 55 ?? 4C 8D 15 ?? ?? ?? ??",
                address => _LEA_1407f6223_Address = address);
            _scanner.Scan("LEA_1407f83d3", "48 8D 05 ?? ?? ?? ?? 44 38 24 ?? 0F 84 ?? ?? ?? ?? 48 8B 5D ?? 48 85 DB 0F 84 ?? ?? ?? ?? 48 8D 7B ?? 48 8B 1B 48 8D 44 24 ?? 48 3B F8 74 ?? 48 8B 45 ?? 48 39 47 ?? 74 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? EB ?? 48 8B 4C 24 ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 64 24 ?? 48 8B D7 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 45 ?? 4C 89 65 ?? 4C 89 6D ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 89 45 ?? EB ?? 48 8D 45 ?? 48 89 45 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 8B 45 ?? 48 8D 55 ?? 49 8B CE 48 89 45 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 85 DB 0F 85 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 4C 8B 44 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 4D 85 C0 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 41 8D 50 ?? FF 10 48 8B 45 ?? 4C 8B 44 24 ?? 4C 89 65 ?? EB ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 74 ?? 4C 8B 44 24 ?? 4C 89 40 ?? 49 8B C4 4C 89 64 24 ?? 48 89 45 ?? 4D 8B C4 48 89 74 24 ?? 4D 85 C0 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 8B 44 24 ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 0F 57 C0 4C 89 6D ?? 48 8D 53 ?? 4C 89 65 ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8D 55 ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 54 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 48 8B 45 ?? 48 8B 54 24 ?? 4C 89 65 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 49 8B C4 49 8B D4 48 89 45 ?? 48 89 54 24 ?? 48 89 74 24 ?? 48 85 D2 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 8B 7D ?? 41 8B F4",
                address => _LEA_1407f83d3_Address = address);
            _scanner.Scan("LEA_1407f9327", "48 8D 0D ?? ?? ?? ?? F6 04 ?? 03",
                address => _LEA_1407f9327_Address = address);
            _scanner.Scan("LEA_1407f9ac3", "48 8D 05 ?? ?? ?? ?? 44 38 24 ?? 0F 84 ?? ?? ?? ?? 48 8B 5D ?? 48 85 DB 0F 84 ?? ?? ?? ?? 48 8D 7B ?? 48 8B 1B 48 8D 44 24 ?? 48 3B F8 74 ?? 48 8B 45 ?? 48 39 47 ?? 74 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? EB ?? 48 8B 4C 24 ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 64 24 ?? 48 8B D7 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 45 ?? 4C 89 65 ?? 4C 89 6D ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 89 45 ?? EB ?? 48 8D 45 ?? 48 89 45 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 8B 45 ?? 48 8D 55 ?? 49 8B CE 48 89 45 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 85 DB 0F 85 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 4C 8B 44 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 4D 85 C0 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 41 8D 50 ?? FF 10 48 8B 45 ?? 4C 8B 44 24 ?? 4C 89 65 ?? EB ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 74 ?? 4C 8B 44 24 ?? 4C 89 40 ?? 49 8B C4 4C 89 64 24 ?? 48 89 45 ?? 4D 8B C4 48 89 74 24 ?? 4D 85 C0 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 49 89 40 ?? 48 8B 45 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 4C 8B 44 24 ?? 4C 89 40 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 0F 57 C0 4C 89 6D ?? 48 8D 53 ?? 4C 89 65 ?? 48 8D 4D ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8D 55 ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? 48 8B 55 ?? 48 8B 45 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 45 ?? 48 8B 55 ?? 48 89 75 ?? 48 85 D2 75 ?? 48 85 C0 74 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B 54 24 ?? 48 8B 45 ?? 4C 89 6C 24 ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 01 BA 01 00 00 00 FF 10 48 8B 45 ?? 48 8B 54 24 ?? 4C 89 65 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 49 8B C4 49 8B D4 48 89 45 ?? 48 89 54 24 ?? 48 89 74 24 ?? 48 85 D2 75 ?? 48 85 C0 0F 84 ?? ?? ?? ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 54 24 ?? 48 89 50 ?? 4C 89 64 24 ?? 4C 89 65 ?? E9 ?? ?? ?? ?? 48 8B 7D ?? BE FF FF FF 7F 48 85 FF 0F 84 ?? ?? ?? ?? 4C 8D 35 ?? ?? ?? ?? 48 8D 5F ?? 48 8B 3F 48 8D 44 24 ?? 48 3B D8 74 ?? 48 8B 45 ?? 48 39 43 ?? 74 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? EB ?? 48 8B 4C 24 ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4C 24 ?? 48 89 48 ?? 4C 89 65 ?? 4C 89 64 24 ?? 48 8B D3 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 45 ?? 4C 89 65 ?? 4C 89 6D ?? 48 85 C0 74 ?? 48 8D 4C 24 ?? 48 89 45 ?? 48 89 4D ?? 48 8D 4D ?? 48 89 48 ?? 48 8D 45 ?? 48 89 45 ?? EB ?? 48 8D 45 ?? 48 89 45 ?? 48 8D 44 24 ?? 48 89 45 ?? 48 8B 45 ?? 48 8D 4D ?? 0F 57 C0 48 89 45 ?? 49 8B D7 4C 89 6D ?? F3 0F 7F 45 ?? 4C 89 65 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 48 8D 4D ?? E8 ?? ?? ?? ?? 49 8B 57 ?? 48 8D 4D ?? 0F 57 C0 4C 89 75 ?? 48 83 C2 40 4C 89 65 ?? F3 0F 7F 45 ?? E8 ?? ?? ?? ?? 48 8B 55 ?? 4C 8D 0D ?? ?? ?? ??",
                address => _LEA_1407f9ac3_Address = address);
            _scanner.Scan("CMP_1407fa38f", "46 38 B4 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ??",
                address => _CMP_1407fa38f_Address = address);
            _scanner.Scan("MOVZX_140800dbc", "0F B6 84 ?? ?? ?? ?? ?? FE C8 3C 01 0F 86 ?? ?? ?? ??",
                address => _MOVZX_140800dbc_Address = address);
            _scanner.Scan("MOVZX_140808efa", "44 0F B6 AC ?? ?? ?? ?? ?? 48 8B 45 ??",
                address => _MOVZX_140808efa_Address = address);
            _scanner.Scan("MOVZX_140809ad7", "0F B6 84 ?? ?? ?? ?? ?? FE C8 3C 01 48 8D 05 ?? ?? ?? ??",
                address => _MOVZX_140809ad7_Address = address);
            _scanner.Scan("MOVZX_1409ec814", "41 0F B6 84 ?? ?? ?? ?? ?? FE C8",
                address => _MOVZX_1409ec814_Address = address);
            _scanner.Scan("CMP_140ac4cb6", "44 38 B4 ?? ?? ?? ?? ?? 0F 85 ?? ?? ?? ??",
                address => _CMP_140ac4cb6_Address = address);
            _scanner.Scan("LEA_140acc3d0", "48 8D 05 ?? ?? ?? ?? 40 38 34 ?? 0F 85 ?? ?? ?? ?? 49 8D 8E ?? ?? ?? ??",
                address => _LEA_140acc3d0_Address = address);
            _scanner.Scan("MOVZX_140ad3048", "42 0F B6 B4 ?? ?? ?? ?? ?? 40 32 FF",
                address => _MOVZX_140ad3048_Address = address);
            _scanner.Scan("MOVZX_140ad37ee", "42 0F B6 B4 ?? ?? ?? ?? ?? 48 85 C0",
                address => _MOVZX_140ad37ee_Address = address);
            _scanner.Scan("LEA_140ada874", "48 8D 0D ?? ?? ?? ?? 48 8D 35 ?? ?? ?? ?? 48 8D 04 ??",
                address => _LEA_140ada874_Address = address);
            _scanner.Scan("LEA_140b5115b", "48 8D 05 ?? ?? ?? ?? 40 38 34 ?? 0F 85 ?? ?? ?? ?? 49 8D 8D ?? ?? ?? ??",
                address => _LEA_140b5115b_Address = address);
            _scanner.Scan("CMP_140b53c15", "45 38 A4 ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ??",
                address => _CMP_140b53c15_Address = address);
            _scanner.Scan("LEA_140b7daa1", "48 8D 05 ?? ?? ?? ?? 80 3C ?? 00 0F 85 ?? ?? ?? ?? 4C 8B B5 ?? ?? ?? ??",
                address => _LEA_140b7daa1_Address = address);
            _scanner.Scan("LEA_140b7ec53", "48 8D 05 ?? ?? ?? ?? 42 80 3C ?? 00 0F 85 ?? ?? ?? ?? 4C 8B 05 ?? ?? ?? ?? 4C 8D 15 ?? ?? ?? ?? 32 C0 45 32 E4",
                address => _LEA_140b7ec53_Address = address);
            _scanner.Scan("LEA_140b82003", "48 8D 05 ?? ?? ?? ?? 42 80 3C ?? 00 0F 85 ?? ?? ?? ?? 4C 8B 05 ?? ?? ?? ?? 4C 8D 15 ?? ?? ?? ?? 32 C0 45 33 F6",
                address => _LEA_140b82003_Address = address);
            _scanner.Scan("MOVZX_140d23838", "0F B6 84 ?? ?? ?? ?? ?? 0F B7 4F ?? 66 85 C9",
                address => _MOVZX_140d23838_Address = address);
            _scanner.Scan("LEA_140d73d5f", "48 8D 05 ?? ?? ?? ?? 0F B6 04 ?? 84 C0",
                address => _LEA_140d73d5f_Address = address);
            _scanner.ScanForData("CALL_14073e193 for FUN_140e35190 for LEA_140e3519a", "E8 ?? ?? ?? ?? 84 C0 74 ?? FE C8", 5, 1, 10,
                address => _LEA_140e3519a_Address = address);
            _scanner.Scan("MOVZX_14123d8c3", "0F B6 8C ?? ?? ?? ?? ?? 84 C9 0F 84 ?? ?? ?? ??",
                address => _MOVZX_14123d8c3_Address = address);
            
            // For DAT_142226bfd
            _scanner.Scan("MOVZX_1407ba194", "44 0F B6 BC ?? ?? ?? ?? ?? 0F B6 84 ?? ?? ?? ?? ??",
                address => _MOVZX_1407ba194_Address = address);
            _scanner.Scan("LEA_140ad5841", "48 8D 0D ?? ?? ?? ?? 33 F6 0F 57 C0",
                address => _LEA_140ad5841_Address = address);
            _scanner.Scan("LEA_140ad5bb6", "48 8D 0D ?? ?? ?? ?? 0F 57 C0 C7 45 ?? 00 00 00 00",
                address => _LEA_140ad5bb6_Address = address);
            _scanner.Scan("LEA_140b764d7", "48 8D 05 ?? ?? ?? ?? 80 3C ?? 01 0F 95 C0",
                address => _LEA_140b764d7_Address = address);
            _scanner.Scan("FUN_14bd94290 for LEA_14bd9429a", "0F B7 C1 48 8D 0C ?? 48 01 C9 48 8D 05 ?? ?? ?? ?? 0F B6 04 ??",
                address => _LEA_14bd9429a_Address = address + 10);
            
            // For DAT_142226bfe
            _scanner.Scan("MOVZX_1407ba19d", "0F B6 84 ?? ?? ?? ?? ?? 38 4D ??",
                address => _MOVZX_1407ba19d_Address = address);
            
            // For DAT_142226c00
            _scanner.Scan("LEA_1407ba233", "48 8D 86 ?? ?? ?? ?? 48 03 C3",
                address => _LEA_1407ba233_Address = address);
            
            // For DAT_142226c04
            _scanner.Scan("CMP_140e12a8d", "46 38 AC ?? ?? ?? ?? ?? 0F 84 ?? ?? ?? ?? 42 F7 84 ?? ?? ?? ?? ?? 00 00 08 00",
                address => _CMP_140e12a8d_Address = address);
            _scanner.Scan("CMP_140e12b6a", "80 BC ?? ?? ?? ?? ?? 64 0F 83 ?? ?? ?? ?? F6 C1 40",
                address => _CMP_140e12b6a_Address = address);
            
            // For DAT_142226c06
            _scanner.Scan("MOVZX_1409f1a37", "0F B6 84 ?? ?? ?? ?? ?? 48 8B CB 66 0F 6E C0",
                address => _MOVZX_1409f1a37_Address = address);
            
            // For DAT_142226c07
            _scanner.Scan("MOVZX_1400fa0d5", "42 0F B6 84 ?? ?? ?? ?? ?? FF C8 83 F8 0F 77 ?? 48 98 0F B6 84 ?? ?? ?? ?? ?? 8B 8C ?? ?? ?? ?? ?? 48 03 CA FF E1 42 80 BC ?? ?? ?? ?? ?? 01",
                address => _MOVZX_1400fa0d5_Address = address);
            _scanner.Scan("MOVZX_1407fac4b", "42 0F B6 84 ?? ?? ?? ?? ?? FF C8 83 F8 0F 0F 87 ?? ?? ?? ??",
                address => _MOVZX_1407fac4b_Address = address);
            _scanner.Scan("MOVZX_140883961", "0F B6 84 ?? ?? ?? ?? ?? 3C 0B 74 ?? 3C 02 0F 85 ?? ?? ?? ??",
                address => _MOVZX_140883961_Address = address);
            _scanner.Scan("MOVZX_14088476c", "0F B6 84 ?? ?? ?? ?? ?? 3C 0B 74 ?? 3C 02 75 ??",
                address => _MOVZX_14088476c_Address = address);
            _scanner.Scan("CMP_1408a8c77", "41 80 BC ?? ?? ?? ?? ?? 04",
                address => _CMP_1408a8c77_Address = address);
            _scanner.Scan("MOVZX_1408eb238", "41 0F B6 84 ?? ?? ?? ?? ?? 83 E8 0C 83 F8 01 0F 86 ?? ?? ?? ?? 41 0F B6 84 ?? ?? ?? ?? ??",
                address => _MOVZX_1408eb238_Address = address);
            _scanner.ScanForData("CALL_14091f24b for FUN_140911c80 for MOVZX_140911e42 and MOVZX_140911f5b", "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? B9 D6 00 00 00 E8 ?? ?? ?? ?? 83 F8 05 48 8B 45 ??",
                5, 1, 0, address =>
                {
                    _MOVZX_140911e42_Address = address + 450;
                    _MOVZX_140911f5b_Address = address + 731;
                });
            _scanner.ScanForData("CALL_140932186 for FUN_140925610 for MOVZX_1409257d2 and MOVZX_1409258eb", "E8 ?? ?? ?? ?? 84 C0 0F 84 ?? ?? ?? ?? B9 D6 00 00 00 E8 ?? ?? ?? ?? 83 F8 05 48 8B 85 ?? ?? ?? ??",
                5, 1, 0, address =>
                {
                    _MOVZX_1409257d2_Address = address + 450;
                    _MOVZX_1409258eb_Address = address + 731;
                });
            _scanner.Scan("MOVZX_1409ec823", "41 0F B6 84 ?? ?? ?? ?? ?? 3C 0E",
                address => _MOVZX_1409ec823_Address = address);
            _scanner.Scan("CMP_140e12342", "80 BC ?? ?? ?? ?? ?? 00 75 ?? 80 BC ?? ?? ?? ?? ?? 01",
                address => _CMP_140e12342_Address = address);
            _scanner.Scan("MOVZX_140e12850", "43 0F B6 84 ?? ?? ?? ?? ?? 83 F8 07",
                address => _MOVZX_140e12850_Address = address);
            _scanner.Scan("MOVZX_140e332ad", "42 0F B6 84 ?? ?? ?? ?? ?? FF C8 83 F8 0F 77 ?? 48 98 0F B6 84 ?? ?? ?? ?? ?? 8B 8C ?? ?? ?? ?? ?? 48 03 CA FF E1 B0 01",
                address => _MOVZX_140e332ad_Address = address);
            _scanner.Scan("MOVZX_140e3348d", "42 0F B6 84 ?? ?? ?? ?? ?? 83 C0 FE",
                address => _MOVZX_140e3348d_Address = address);
            
            // For DAT_142226c0a
            _scanner.Scan("MOVZX_1408eb24d", "41 0F B6 84 ?? ?? ?? ?? ?? 83 E8 0C 83 F8 01 0F 86 ?? ?? ?? ?? 41 0F BE 84 ?? ?? ?? ?? ??",
                address => _MOVZX_1408eb24d_Address = address);
            _scanner.Scan("MOVZX_140e332a2", "42 0F B6 84 ?? ?? ?? ?? ?? EB ?? 42 0F B6 84 ?? ?? ?? ?? ?? FF C8",
                address => _MOVZX_140e332a2_Address = address);
            _scanner.Scan("MOVZX_140e33482", "42 0F B6 84 ?? ?? ?? ?? ?? EB ?? 42 0F B6 84 ?? ?? ?? ?? ?? 83 C0 FE",
                address => _MOVZX_140e33482_Address = address);
            _scanner.Scan("MOVZX_140e347d8", "41 0F B6 84 ?? ?? ?? ?? ?? 2C 0C",
                address => _MOVZX_140e347d8_Address = address);
            
            // For DAT_142226c0e
            _scanner.Scan("CMP_1400f9485", "80 BC ?? ?? ?? ?? ?? 01 75 ?? 80 BC ?? ?? ?? ?? ?? 00",
                address => _CMP_1400f9485_Address = address);
            _scanner.Scan("CMP_1400fa0fb", "42 80 BC ?? ?? ?? ?? ?? 01 0F 85 ?? ?? ?? ??",
                address => _CMP_1400fa0fb_Address = address);
            _scanner.Scan("MOVZX_140e10715", "45 0F B6 BC ?? ?? ?? ?? ?? 41 8D 47 ??",
                address => _MOVZX_140e10715_Address = address);
            _scanner.Scan("CMP_140e110c9", "80 BC ?? ?? ?? ?? ?? 03 44 0F 29 54 24 ??",
                address => _CMP_140e110c9_Address = address);
            _scanner.Scan("MOVZX_140e11193", "0F B6 84 ?? ?? ?? ?? ?? FE C8 A8 FD",
                address => _MOVZX_140e11193_Address = address);
            _scanner.Scan("CMP_140e11dee", "0F B6 84 ?? ?? ?? ?? ?? FE C8 A8 FD",
                address => _CMP_140e11dee_Address = address);
            _scanner.Scan("CMP_140e1234c", "80 BC ?? ?? ?? ?? ?? 01 0F 85 ?? ?? ?? ?? F7 84 ?? ?? ?? ?? ?? 00 00 08 00",
                address => _CMP_140e1234c_Address = address);
            _scanner.Scan("CMP_140e1238f", "80 BC ?? ?? ?? ?? ?? 01 75 ?? 80 BC ?? ?? ?? ?? ?? 64",
                address => _CMP_140e1238f_Address = address);
            _scanner.Scan("CMP_140e13023", "40 38 B4 ?? ?? ?? ?? ?? 75 ?? 40 38 BC ?? ?? ?? ?? ??",
                address => _CMP_140e13023_Address = address);
            _scanner.Scan("CMP_140e33a92", "80 BC ?? ?? ?? ?? ?? 02 44 89 44 24 ??",
                address => _CMP_140e33a92_Address = address);
            _scanner.Scan("CMP_140e33aee", "80 BC ?? ?? ?? ?? ?? 01 75 ?? F7 84 ?? ?? ?? ?? ?? 00 00 08 00",
                address => _CMP_140e33aee_Address = address);
            _scanner.Scan("CMP_140e347c3", "41 80 BC ?? ?? ?? ?? ?? 01 75 ??",
                address => _CMP_140e347c3_Address = address);
            
            // For DAT_142226c0f
            _scanner.Scan("CMP_1400f948f", "80 BC ?? ?? ?? ?? ?? 00 76 ?? F7 84 ?? ?? ?? ?? ?? 00 00 08 00",
                address => _CMP_1400f948f_Address = address);
            _scanner.Scan("CMP_1400fa10a", "42 80 BC ?? ?? ?? ?? ?? 00 0F 86 ?? ?? ?? ??",
                address => _CMP_1400fa10a_Address = address);
            _scanner.Scan("CMP_140e11de0", "80 BC ?? ?? ?? ?? ?? 64 0F 83 ?? ?? ?? ?? 80 BC ?? ?? ?? ?? ?? 02",
                address => _CMP_140e11de0_Address = address);
            _scanner.Scan("CMP_140e12399", "80 BC ?? ?? ?? ?? ?? 64 72 ??",
                address => _CMP_140e12399_Address = address);
            _scanner.Scan("CMP_140e1302d", "40 38 BC ?? ?? ?? ?? ?? 76 ?? F7 84 ?? ?? ?? ?? ?? 00 00 08 00",
                address => _CMP_140e1302d_Address = address);
            _scanner.Scan("CMP_140e347ce", "41 38 84 ?? ?? ?? ?? ?? 77 ??",
                address => _CMP_140e347ce_Address = address);
            
            // For DAT_142226c10
            _scanner.Scan("TEST_1400f9499", "F7 84 ?? ?? ?? ?? ?? 00 00 08 00 75 ?? 41 BA DF 00 00 00",
                address => _TEST_1400f9499_Address = address);
            _scanner.Scan("TEST_1400fa119", "42 F7 84 ?? ?? ?? ?? ?? 00 00 08 00 0F 84 ?? ?? ?? ??",
                address => _TEST_1400fa119_Address = address);
            _scanner.Scan("TEST_1407703a6", "42 85 84 ?? ?? ?? ?? ?? 74 ?? 83 39 00",
                address => _TEST_1407703a6_Address = address);
            _scanner.Scan("LEA_14080e0c4", "48 8D 05 ?? ?? ?? ?? 8B 04 ?? 41 23 C5",
                address => _LEA_14080e0c4_Address = address);
            _scanner.Scan("TEST_1408a8c39", "41 F7 84 ?? ?? ?? ?? ?? 00 00 08 00",
                address => _TEST_1408a8c39_Address = address);
            _scanner.Scan("MOV_140e110a9", "8B AC ?? ?? ?? ?? ?? 4C 89 A4 24 ?? ?? ?? ?? 81 E5 FF FF FF 00",
                address => _MOV_140e110a9_Address = address);
            _scanner.Scan("TEST_140e1235a", "F7 84 ?? ?? ?? ?? ?? 00 00 08 00 0F 84 ?? ?? ?? ?? 45 0F B7 E0",
                address => _TEST_140e1235a_Address = address);
            _scanner.Scan("TEST_140e123a3", "F7 84 ?? ?? ?? ?? ?? 00 00 10 00 74 ?? 8B 06",
                address => _TEST_140e123a3_Address = address);
            _scanner.Scan("TEST_140e12a9b", "42 F7 84 ?? ?? ?? ?? ?? 00 00 08 00 74 ??",
                address => _TEST_140e12a9b_Address = address);
            _scanner.Scan("TEST_140e13037", "F7 84 ?? ?? ?? ?? ?? 00 00 08 00 75 ?? BA B2 03 00 00",
                address => _TEST_140e13037_Address = address);
            _scanner.Scan("TEST_140e33af8", "F7 84 ?? ?? ?? ?? ?? 00 00 08 00 74 ?? 0F BA E7 13",
                address => _TEST_140e33af8_Address = address);
            
            // For DAT_142226c14
            _scanner.Scan("TEST_1400fa0a0", "F7 84 ?? ?? ?? ?? ?? 00 03 00 00 E9 ?? ?? ?? ??",
                address => _TEST_1400fa0a0_Address = address);
            _scanner.Scan("TEST_1400fa1da", "42 F7 84 ?? ?? ?? ?? ?? 00 03 00 00",
                address => _TEST_1400fa1da_Address = address);
            _scanner.Scan("LEA_140ad7871", "48 8D 05 ?? ?? ?? ?? 8B 3C ?? 4C 89 7D ??",
                address => _LEA_140ad7871_Address = address);
            _scanner.Scan("MOV_140e33b47", "8B 9C ?? ?? ?? ?? ?? 8B 84 ?? ?? ?? ?? ?? 49 8B 8D ?? ?? ?? ??",
                address => _MOV_140e33b47_Address = address);
            
            // For DAT_142226c18
            _scanner.Scan("MOV_140e33b4e", "8B 84 ?? ?? ?? ?? ?? 49 8B 8D ?? ?? ?? ?? 89 9C 24 ?? ?? ?? ??",
                address => _MOV_140e33b4e_Address = address);
            
            // For DAT_142226c1c
            _scanner.Scan("MOVZX_1400fa087", "0F B6 84 ?? ?? ?? ?? ?? 2C 13 3C 01 77 ??",
                address => _MOVZX_1400fa087_Address = address);
            _scanner.Scan("MOVZX_1400fa1c7", "42 0F B6 84 ?? ?? ?? ?? ?? 2C 13",
                address => _MOVZX_1400fa1c7_Address = address);
            _scanner.Scan("CMP_1407ba25f", "80 BC ?? ?? ?? ?? ?? 0F 0F 85 ?? ?? ?? ??",
                address => _CMP_1407ba25f_Address = address);
            _scanner.Scan("TEST_1408a8c2f", "45 84 BC ?? ?? ?? ?? ?? 74 ??",
                address => _TEST_1408a8c2f_Address = address);
            _scanner.Scan("CMP_140ac516e", "80 BC ?? ?? ?? ?? ?? 12 41 0F 44 DF F7 C3 00 00 00 07 0F 85 ?? ?? ?? ??",
                address => _CMP_140ac516e_Address = address);
            _scanner.Scan("CMP_140ac59fa", "80 BC ?? ?? ?? ?? ?? 12 41 0F 44 DF F7 C3 00 00 00 07 75 ??",
                address => _CMP_140ac59fa_Address = address);
            _scanner.Scan("CMP_140e104af", "80 BC ?? ?? ?? ?? ?? 06 74 ??",
                address => _CMP_140e104af_Address = address);
            _scanner.Scan("CMP_140e106ea", "41 80 BC ?? ?? ?? ?? ?? 0D",
                address => _CMP_140e106ea_Address = address);
            _scanner.Scan("CMP_140e11083", "80 BC ?? ?? ?? ?? ?? 0D 75 ?? B8 00 00 08 00 48 81 C4 D0 00 00 00",
                address => _CMP_140e11083_Address = address);
            _scanner.Scan("MOVZX_140e111c8", "0F B6 94 ?? ?? ?? ?? ?? 80 FA 0E",
                address => _MOVZX_140e111c8_Address = address);
            _scanner.Scan("MOVZX_140e12668", "41 0F B6 84 ?? ?? ?? ?? ?? FF C8",
                address => _MOVZX_140e12668_Address = address);
            _scanner.Scan("CMP_140e12a2c", "43 80 BC ?? ?? ?? ?? ?? 12",
                address => _CMP_140e12a2c_Address = address);
            _scanner.Scan("CMP_140e33a71", "80 BC ?? ?? ?? ?? ?? 15 75 ?? 8B 43 ??",
                address => _CMP_140e33a71_Address = address);
            _scanner.Scan("MOVZX_140e33cd4", "0F B6 84 ?? ?? ?? ?? ?? 88 85 ?? ?? ?? ?? 44 89 44 24 ??",
                address => _MOVZX_140e33cd4_Address = address);
            _scanner.Scan("MOVZX_140e34851", "41 0F B6 8C ?? ?? ?? ?? ?? 41 88 8D ?? ?? ?? ??",
                address => _MOVZX_140e34851_Address = address);
            
            // For DAT_142226c1d
            _scanner.Scan("MOVZX_140e4035c", "0F B6 05 ?? ?? ?? ?? C3 48 8D 05 ?? ?? ?? ??",
                address => _MOVZX_140e4035c_Address = address);
            _scanner.Scan("MOVZX_140e40427", "0F B6 05 ?? ?? ?? ?? C3 66 85 C9",
                address => _MOVZX_140e40427_Address = address);
            
            // Segment 0
            _scanner.ScanForData("DAT_142260b80", "4C 8D 05 ?? ?? ?? ?? 41 80 3C ?? 0A", 7, 3, 0,
                address => _DAT_142260b80_Address = address);
            
            // For DATx142260b80
            _scanner.Scan("LEA_14071a1ce", "4D 8D B4 24 ?? ?? ?? ?? 4F 8D 34 ??",
                address => _LEA_14071a1ce_Address = address);
            _scanner.Scan("LEA_14071a445", "4D 8D BC 24 ?? ?? ?? ?? 0F B7 41 ??",
                address => _LEA_14071a445_Address = address);
            _scanner.Scan("LEA_14073acb7", "48 8D 89 ?? ?? ?? ?? 48 85 C9 74 ??",
                address => _LEA_14073acb7_Address = address);
            _scanner.Scan("CMP_14073ad65", "80 BC ?? ?? ?? ?? ?? 15 75 ?? E8 ?? ?? ?? ?? 44 8B C0",
                address => _CMP_14073ad65_Address = address);
            _scanner.Scan("LEA_14073ae82", "48 8D 89 ?? ?? ?? ?? 48 85 C9 0F 84 ?? ?? ?? ?? 80 39 ??",
                address => _LEA_14073ae82_Address = address);
            _scanner.Scan("CMP_14073b083", "80 BC ?? ?? ?? ?? ?? 15 75 ?? E8 ?? ?? ?? ?? 8B D8",
                address => _CMP_14073b083_Address = address);
            _scanner.Scan("LEA_14073b110", "48 8D 0C C5 ?? ?? ?? ?? 48 03 CB",
                address => _LEA_14073b110_Address = address);
            _scanner.Scan("CMP_14076476f", "41 80 BC ?? ?? ?? ?? ?? 0A",
                address => _CMP_14076476f_Address = address);
            _scanner.Scan("MOVSX_14076487d", "45 0F BE 84 ?? ?? ?? ?? ??",
                address => _MOVSX_14076487d_Address = address);
            _scanner.Scan("LEA_1407b9f92", "4C 8D 04 C5 ?? ?? ?? ?? 43 0F BE 0C ??",
                address => _LEA_1407b9f92_Address = address);
            _scanner.Scan("MOVSX_1407fb6f3", "42 0F BE 94 ?? ?? ?? ?? ?? 48 8B 49 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? 8B D8 E8 ?? ?? ?? ?? 0F BA E3 1B 73 ?? F3 0F 10 07 41 BD 0F 00 00 00",
                address => _MOVSX_1407fb6f3_Address = address);
            _scanner.Scan("MOVSX_1407fb805", "42 0F BE 94 ?? ?? ?? ?? ?? 48 8B 49 ?? E8 ?? ?? ?? ?? 48 8D 4D ?? 8B D8 E8 ?? ?? ?? ?? 0F BA E3 1B 73 ?? F3 0F 10 07 41 BD 1E 00 00 00",
                address => _MOVSX_1407fb805_Address = address);
            _scanner.Scan("MOVSX_1407fdee4", "0F BE 05 ?? ?? ?? ?? 03 D0",
                address => _MOVSX_1407fdee4_Address = address);
            _scanner.Scan("MOVSX_1407fdf5a", "0F BE 15 ?? ?? ?? ?? 48 89 45 ??",
                address => _MOVSX_1407fdf5a_Address = address);
            _scanner.Scan("LEA_14080095e", "48 8D B1 ?? ?? ?? ?? BB 01 00 00 00",
                address => _LEA_14080095e_Address = address);
            _scanner.Scan("LEA_140801ef4", "4C 8D 05 ?? ?? ?? ?? 41 80 3C ?? 0A",
                address => _LEA_140801ef4_Address = address);
            _scanner.Scan("MOVSX_140807419", "0F BE 94 ?? ?? ?? ?? ?? 0F B7 CF",
                address => _MOVSX_140807419_Address = address);
            _scanner.Scan("MOVSX_140808e2a", "41 0F BE 8C ?? ?? ?? ?? ?? FF C1",
                address => _MOVSX_140808e2a_Address = address);
            _scanner.Scan("LEA_140815aab", "4C 8D 3C C5 ?? ?? ?? ?? 43 80 3C ?? ??",
                address => _LEA_140815aab_Address = address);
            _scanner.Scan("MOVSX_1408eb262", "41 0F BE 84 ?? ?? ?? ?? ?? FF C0",
                address => _MOVSX_1408eb262_Address = address);
            _scanner.ScanMany("CMP_140911f79 and CMP_140925909", "41 80 BC ?? ?? ?? ?? ?? 15 E9 ?? ?? ?? ?? 45 8B 48 ?? 41 8D 41 ?? 83 F8 01 0F 87 ?? ?? ?? ?? 41 8B 40 ?? 41 83 F9 02 75 ?? 0F B7 C8 E8 ?? ?? ?? ?? 4C 8B 45 ?? 48 8B 55 ?? 0F B7 C0 0F B7 C0 49 8D 8E ?? ?? ?? ?? 48 8D 0C ?? 48 85 C9 0F 84 ?? ?? ?? ?? 80 39 ?? 0F 85 ?? ?? ?? ?? 48 89 5D ?? 48 85 D2 75 ?? 48 8B 4D ?? 48 85 C9 75 ?? 4D 85 C0 0F 84 ?? ?? ?? ?? EB ?? 48 8B 45 ?? 48 89 42 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? EB ?? 41 83 78 ?? 04 EB ?? 41 83 78 ?? 07 EB ?? 41 83 78 ?? 02 0F 85 ?? ?? ?? ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 55 ?? 48 89 75 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CE 48 8B D6 48 89 4D ?? 48 89 55 ?? 48 8D 05 ?? ?? ?? ?? 48 89 45 ?? 48 85 D2 75 ?? 48 85 C9 74 ?? EB ?? 48 89 4A ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? 48 89 75 ?? 48 89 75 ?? 48 8B CF E8 ?? ?? ?? ?? B0 01 EB ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 4D 85 C0 74 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? 48 89 48 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4D ?? 48 89 48 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CF E8 ?? ?? ?? ?? 32 C0 4C 8B 74 24 ?? 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 40 5D C3",
                2, addresses =>
                {
                    _CMP_140911f79_Address = addresses[0];
                    _CMP_140925909_Address = addresses[1];
                });
            _scanner.ScanMany("LEA_140911fb8 and LEA_140925948", "49 8D 8E ?? ?? ?? ?? 48 8D 0C ?? 48 85 C9 0F 84 ?? ?? ?? ?? 80 39 ?? 0F 85 ?? ?? ?? ?? 48 89 5D ?? 48 85 D2 75 ?? 48 8B 4D ?? 48 85 C9 75 ?? 4D 85 C0 0F 84 ?? ?? ?? ?? EB ?? 48 8B 45 ?? 48 89 42 ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? EB ?? 41 83 78 ?? 04 EB ?? 41 83 78 ?? 07 EB ?? 41 83 78 ?? 02 0F 85 ?? ?? ?? ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 55 ?? 48 89 75 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CE 48 8B D6 48 89 4D ?? 48 89 55 ?? 48 8D 05 ?? ?? ?? ?? 48 89 45 ?? 48 85 D2 75 ?? 48 85 C9 74 ?? EB ?? 48 89 4A ?? 48 8B 4D ?? 48 85 C9 74 ?? 48 8B 55 ?? 48 89 51 ?? 48 89 75 ?? 48 89 75 ?? 48 8B CF E8 ?? ?? ?? ?? B0 01 EB ?? 48 8B 45 ?? 48 89 5D ?? 48 85 D2 75 ?? 48 85 C0 75 ?? 4D 85 C0 74 ?? 49 8B 00 BA 01 00 00 00 49 8B C8 FF 10 48 8B 4D ?? 48 8B 45 ?? 48 85 C9 75 ?? 48 85 C0 74 ?? 48 89 48 ?? EB ?? 48 89 41 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 4D ?? 48 89 48 ?? EB ?? 48 89 42 ?? 48 8B 45 ?? 48 85 C0 74 ?? 48 8B 55 ?? 48 89 50 ?? 48 8B CF E8 ?? ?? ?? ?? 32 C0 4C 8B 74 24 ?? 48 8B 5C 24 ?? 48 8B 74 24 ?? 48 8B 7C 24 ?? 48 83 C4 40 5D C3",
                2, addresses =>
                {
                    _LEA_140911fb8_Address = addresses[0];
                    _LEA_140925948_Address = addresses[1];
                });
            _scanner.Scan("MOVSX_140ac50f3", "0F BE 94 ?? ?? ?? ?? ?? 48 89 45 ?? 48 8B 48 ?? E8 ?? ?? ?? ?? 48 8B 55 ?? 4C 8D 05 ?? ?? ?? ??",
                address => _MOVSX_140ac50f3_Address = address);
            _scanner.Scan("MOVSX_140ac5983", "0F BE 94 ?? ?? ?? ?? ?? 48 8B 49 ?? E8 ?? ?? ?? ?? 8B D8",
                address => _MOVSX_140ac5983_Address = address);
            _scanner.Scan("LEA_140ad202a", "49 8D B5 ?? ?? ?? ?? 48 8B DA",
                address => _LEA_140ad202a_Address = address);
            _scanner.Scan("LEA_140ad241e", "48 8D 34 C5 ?? ?? ?? ?? 42 80 3C ?? ??",
                address => _LEA_140ad241e_Address = address);
            _scanner.Scan("LEA_140ad2745", "4C 8D 34 C5 ?? ?? ?? ?? 43 80 3C ?? ??",
                address => _LEA_140ad2745_Address = address);
            _scanner.Scan("LEA_140b31af6", "48 8D 3C C5 ?? ?? ?? ?? 49 03 F8 80 3F ??",
                address => _LEA_140b31af6_Address = address);
            _scanner.Scan("LEA_140b31fa5", "4D 8D B9 ?? ?? ?? ?? 41 80 3C ?? 0A",
                address => _LEA_140b31fa5_Address = address);
            _scanner.Scan("LEA_140b325d8", "49 8D BD ?? ?? ?? ?? 80 3C ?? 0A",
                address => _LEA_140b325d8_Address = address);
            _scanner.Scan("LEA_140b32a62", "4C 8D B2 ?? ?? ?? ?? 41 80 3C ?? 0A",
                address => _LEA_140b32a62_Address = address);
            _scanner.Scan("CMP_140b7469f", "43 80 BC ?? ?? ?? ?? ?? 0A 76 ??",
                address => _CMP_140b7469f_Address = address);
            _scanner.Scan("MOVSX_140b748c0", "43 0F BE 84 ?? ?? ?? ?? ?? 03 D0",
                address => _MOVSX_140b748c0_Address = address);
            _scanner.Scan("MOVSX_140b74973", "43 0F BE 94 ?? ?? ?? ?? ?? 48 89 45 ??",
                address => _MOVSX_140b74973_Address = address);
            _scanner.Scan("CMP_140d71d6a", "41 38 84 ?? ?? ?? ?? ?? 75 ?? 0F B7 CB E8 ?? ?? ?? ?? 0F BA E0 1E 72 ?? 0F B7 CB 8B D1 81 C1 00 A0 FF FF 48 C1 E9 05 83 E2 1F 41 8B 84 ?? ?? ?? ?? ?? 0F A3 D0 72 ??",
                address => _CMP_140d71d6a_Address = address);
            _scanner.Scan("CMP_140d72d0c", "41 38 84 ?? ?? ?? ?? ?? 75 ?? 0F B7 CB E8 ?? ?? ?? ?? 0F BA E0 1E 72 ?? 0F B7 CB 8B D1 81 C1 00 A0 FF FF 48 C1 E9 05 83 E2 1F 41 8B 84 ?? ?? ?? ?? ?? 0F A3 D0 73 ??",
                address => _CMP_140d72d0c_Address = address);
            _scanner.Scan("CMP_140d73094", "38 84 ?? ?? ?? ?? ?? 75 ?? 0F B7 CB E8 ?? ?? ?? ?? 0F BA E0 1E 72 ?? 48 8B 8C 24 ?? ?? ?? ??",
                address => _CMP_140d73094_Address = address);
            _scanner.Scan("CMP_140e16eab", "43 80 BC ?? ?? ?? ?? ?? 0A 75 ??",
                address => _CMP_140e16eab_Address = address);
            _scanner.Scan("MOVSX_140e16f21", "43 0F BE 84 ?? ?? ?? ?? ?? 48 83 C4 28",
                address => _MOVSX_140e16f21_Address = address);
            _scanner.Scan("LEA_140e16f74", "48 8D 0D ?? ?? ?? ?? 0F BE 04 ?? 83 F8 08",
                address => _LEA_140e16f74_Address = address);
            _scanner.Scan("LEA_140e32879", "48 8D 15 ?? ?? ?? ?? 0F B7 C7",
                address => _LEA_140e32879_Address = address);
            _scanner.Scan("LEA_140e32cd6", "4C 8D 0C C5 ?? ?? ?? ?? 48 8D 43 ??",
                address => _LEA_140e32cd6_Address = address);
            _scanner.Scan("LEA_140e33076", "4C 8D 3D ?? ?? ?? ?? 0F 1F 00 8B 43 ??",
                address => _LEA_140e33076_Address = address);
            _scanner.Scan("LEA_140e331d6", "4C 8D 3D ?? ?? ?? ?? 0F 1F 00 44 3B 73 ??",
                address => _LEA_140e331d6_Address = address);
            _scanner.ScanForData("CALL_14078f65d for FUN_140e3eb70 for LEA_140e3eb73", "E8 ?? ?? ?? ?? 41 0F B7 8F ?? ?? ?? ?? 48 8B D8 E8 ?? ?? ?? ?? 0F B6 0B 80 E9 0B 80 F9 0A 0F 86 ?? ?? ?? ??",
                5, 1, 0, address => _LEA_140e3eb73_Address = address + 3);
            _scanner.Scan("LEA_140e4051a", "4C 8D 15 ?? ?? ?? ?? 41 0F B7 00",
                address => _LEA_140e4051a_Address = address);
            _scanner.Scan("LEA_140fae067", "48 8D 15 ?? ?? ?? ?? 0F BE 14 ??",
                address => _LEA_140fae067_Address = address);
            _scanner.Scan("MOVSX_140fb00f7", "41 0F BE 94 ?? ?? ?? ?? ?? 45 0F B6 94 ?? ?? ?? ?? ??",
                address => _MOVSX_140fb00f7_Address = address);
            _scanner.Scan("MOVSX_140fb0317", "0F BE 94 ?? ?? ?? ?? ?? 44 0F B6 9C ?? ?? ?? ?? ??",
                address => _MOVSX_140fb0317_Address = address);
            _scanner.Scan("MOVSX_140fb0c79", "41 0F BE 94 ?? ?? ?? ?? ?? 45 0F B6 84 ?? ?? ?? ?? ??",
                address => _MOVSX_140fb0c79_Address = address);
            _scanner.Scan("MOVSX_140fb0e20", "41 0F BE 94 ?? ?? ?? ?? ?? 83 FA FF",
                address => _MOVSX_140fb0e20_Address = address);
            _scanner.Scan("LEA_140fb3c8f", "4C 8D 2D ?? ?? ?? ?? 44 8B 25 ?? ?? ?? ?? 0F 57 C9",
                address => _LEA_140fb3c8f_Address = address);
            _scanner.Scan("LEA_140fb8094", "4C 8D 15 ?? ?? ?? ?? 4C 8B C9 41 0F B6 54 ?? ??",
                address => _LEA_140fb8094_Address = address);
            _scanner.Scan("LEA_141780647", "48 8D 0D ?? ?? ?? ?? 80 7C ?? ?? 01",
                address => _LEA_141780647_Address = address);
            
            // For DATx142260b81
            _scanner.Scan("CMP_14077033f", "80 BC ?? ?? ?? ?? ?? 00 0F 85 ?? ?? ?? ?? 0F B7 D1",
                address => _CMP_14077033f_Address = address);
            _scanner.Scan("CMP_140883791", "45 38 A4 ?? ?? ?? ?? ?? 75 ?? 40 B6 01",
                address => _CMP_140883791_Address = address);
            _scanner.Scan("CMP_14088459c", "45 38 A4 ?? ?? ?? ?? ?? 75 ?? 40 B7 01",
                address => _CMP_14088459c_Address = address);
            _scanner.Scan("CMP_1409ec802", "41 80 BC ?? ?? ?? ?? ?? 00 75 ?? 48 8D 0C ??",
                address => _CMP_1409ec802_Address = address);
            _scanner.Scan("CMP_140d07c26", "80 BC ?? ?? ?? ?? ?? 00 0F 85 ?? ?? ?? ?? 48 8D 04 ?? 48 03 C0 0F B6 8C ?? ?? ?? ?? ?? 83 E9 01 74 ?? 83 F9 01 0F 85 ?? ?? ?? ?? 41 BE 1C 01 00 00",
                address => _CMP_140d07c26_Address = address);
            _scanner.Scan("CMP_140d0ac66", "41 80 BC ?? ?? ?? ?? ?? 00 0F 85 ?? ?? ?? ?? 48 8D 04 ??",
                address => _CMP_140d0ac66_Address = address);
            _scanner.ScanForData("CALL_14073e946 for FUN_140e35390 for LEA_140e35393", "E8 ?? ?? ?? ?? 84 C0 75 ?? 0F B7 8E ?? ?? ?? ?? E8 ?? ?? ?? ?? 84 C0 0F 85 ?? ?? ?? ??",
                5, 1, 0, address => _LEA_140e35393_Address = address + 3);
            _scanner.Scan("CMP_14177b599", "80 BC ?? ?? ?? ?? ?? 00 0F 85 ?? ?? ?? ?? 48 8D 04 ?? 48 03 C0 0F B6 8C ?? ?? ?? ?? ?? 83 E9 01 74 ?? 83 F9 01 0F 85 ?? ?? ?? ?? BF 1C 01 00 00",
                address => _CMP_14177b599_Address = address);
            
            // For DATx142260b82
            _scanner.Scan("MOVZX_140fa4eba", "44 0F B6 94 ?? ?? ?? ?? ?? 0F B7 D0",
                address => _MOVZX_140fa4eba_Address = address);
            _scanner.Scan("CMP_140fa4ec6", "44 38 94 ?? ?? ?? ?? ?? 75 ?? 41 FF C0",
                address => _CMP_140fa4ec6_Address = address);
            _scanner.Scan("LEA_140faf2c4", "48 8D 0D ?? ?? ?? ?? F3 0F 10 25 ?? ?? ?? ??",
                address => _LEA_140faf2c4_Address = address);
            _scanner.Scan("LEA_140faf7db", "4C 8D 0D ?? ?? ?? ?? 41 0F B7 D3",
                address => _LEA_140faf7db_Address = address);
            _scanner.Scan("MOVZX_140fb0100", "45 0F B6 94 ?? ?? ?? ?? ?? 83 FA FF",
                address => _MOVZX_140fb0100_Address = address);
            _scanner.Scan("MOVZX_140fb031f", "44 0F B6 9C ?? ?? ?? ?? ?? 83 FA FF",
                address => _MOVZX_140fb031f_Address = address);
            _scanner.Scan("MOVZX_140fb0c82", "45 0F B6 84 ?? ?? ?? ?? ?? 83 FA FF",
                address => _MOVZX_140fb0c82_Address = address);
            _scanner.Scan("MOVZX_140fb0e30", "45 0F B6 84 ?? ?? ?? ?? ?? 45 85 C0",
                address => _MOVZX_140fb0e30_Address = address);
            _scanner.Scan("LEA_140fb3803", "4C 8D 0D ?? ?? ?? ?? 0F B7 D7",
                address => _LEA_140fb3803_Address = address);
            
            // Funny literal replacements
            _scanner.Scan("LiteralReplacement_140fa4eda", "FF C0 3D 20 04 00 00 7C ?? 42 8D 04 6D 00 00 00 00",
                address => _LiteralReplacement_140fa4eda_Address = address);
            _scanner.Scan("LiteralReplacement_140faf7ed", "BF 20 04 00 00 0F 1F 40 00 66 66 0F 1F 84 ?? 00 00 00 00",
                address => _LiteralReplacement_140faf7ed_Address = address);
            _scanner.Scan("LiteralReplacement_140fb3814", "41 BB 20 04 00 00",
                address => _LiteralReplacement_140fb3814_Address = address);
            _scanner.Scan("LiteralReplacement_140fb3d43", "FF C0 3D 20 04 00 00 7C ?? 43 8D 04 ??",
                address => _LiteralReplacement_140fb3d43_Address = address);
            _scanner.Scan("LiteralReplacement_14076ff34", "49 8D 8D ?? ?? ?? ?? 49 8B 47 ??",
                address => _LiteralReplacement_14076ff34_Address = address);
            _scanner.Scan("LiteralReplacement_14073b876", "49 8D 95 ?? ?? ?? ?? 48 8B 48 ?? 8B 79 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 5C 24 ?? 48 8B CB E8 ?? ?? ?? ?? 48 8D 4B ?? E8 ?? ?? ?? ?? F3 0F 10 05 ?? ?? ?? ?? BA 01 00 00 00 44 89 7C 24 ?? 41 B9 6D 01 00 00",
                address => _LiteralReplacement_14073b876_Address = address);
            _scanner.Scan("LiteralReplacement_14073b458", "49 8D 95 ?? ?? ?? ?? 48 8B 48 ?? 8B 79 ?? 48 8D 4C 24 ?? E8 ?? ?? ?? ?? 48 8B 5C 24 ?? 48 8B CB E8 ?? ?? ?? ?? 48 8D 4B ?? E8 ?? ?? ?? ?? F3 0F 10 05 ?? ?? ?? ?? BA 01 00 00 00 44 89 7C 24 ?? 41 B9 6E 01 00 00",
                address => _LiteralReplacement_14073b458_Address = address);
            _scanner.Scan("LiteralReplacement_14073c1c6", "32 C9 BA 20 03 00 00",
                address => _LiteralReplacement_14073c1c6_Address = address);
            _scanner.Scan("LiteralReplacement_14076472e", "BA 1E 03 00 00 0F 1F 40 00 66 0F 1F 84 ?? 00 00 00 00",
                address => _LiteralReplacement_14076472e_Address = address);
            _scanner.Scan("LiteralReplacement_1407b9f6c", "BA 1E 03 00 00 4C 8D 0D ?? ?? ?? ??",
                address => _LiteralReplacement_1407b9f6c_Address = address);
            _scanner.Scan("LiteralReplacement_14080093d", "B9 1E 03 00 00 42 0F B7 54 ?? ??",
                address => _LiteralReplacement_14080093d_Address = address);
            _scanner.Scan("LiteralReplacement_140801ec9", "B9 1E 03 00 00 66 90",
                address => _LiteralReplacement_140801ec9_Address = address);
            _scanner.Scan("LiteralReplacement_140802097", "B9 1E 03 00 00 41 FF C6 48 83 C6 02",
                address => _LiteralReplacement_140802097_Address = address);
            _scanner.Scan("LiteralReplacement_140800b29", "B9 1E 03 00 00 41 FF C6 49 FF C7",
                address => _LiteralReplacement_140800b29_Address = address);
            _scanner.Scan("LiteralReplacement_1407ba0c1", "BA 1E 03 00 00 FF C3",
                address => _LiteralReplacement_1407ba0c1_Address = address);
            _scanner.Scan("LiteralReplacement_140764905", "BA 1E 03 00 00 41 FF C6",
                address => _LiteralReplacement_140764905_Address = address);
            _scanner.Scan("LiteralReplacement_1408073f8", "48 8D 44 24 ?? 41 B9 1E 03 00 00",
                address => _LiteralReplacement_1408073f8_Address = address);
            _scanner.Scan("LiteralReplacement_140808dab", "4C 89 6D ?? B9 1E 03 00 00",
                address => _LiteralReplacement_140808dab_Address = address);
            _scanner.Scan("LiteralReplacement_14080988e", "B9 1E 03 00 00 41 FF C6 49 FF C5",
                address => _LiteralReplacement_14080988e_Address = address);
            _scanner.Scan("LiteralReplacement_140b31f61", "4C 8D 6D ?? B9 1E 03 00 00",
                address => _LiteralReplacement_140b31f61_Address = address);
            _scanner.Scan("LiteralReplacement_140b3241f", "B9 1E 03 00 00 48 8B 7D ??",
                address => _LiteralReplacement_140b3241f_Address = address);
            _scanner.Scan("LiteralReplacement_140b32a23", "BA 1E 03 00 00 0F 1F 84 ?? 00 00 00 00",
                address => _LiteralReplacement_140b32a23_Address = address);
            _scanner.Scan("LiteralReplacement_140b32ec1", "BA 1E 03 00 00 41 FF C7",
                address => _LiteralReplacement_140b32ec1_Address = address);
            _scanner.Scan("LiteralReplacement_140b744d1", "B9 20 03 00 00 F3 44 0F 10 0D ?? ?? ?? ??",
                address => _LiteralReplacement_140b744d1_Address = address);
            _scanner.Scan("LiteralReplacement_140b74c2e", "B9 20 03 00 00 41 FF C7",
                address => _LiteralReplacement_140b74c2e_Address = address);
            _scanner.Scan("LiteralReplacement_140e16e8b", "81 FA 20 03 00 00 0F 83 ?? ?? ?? ??",
                address => _LiteralReplacement_140e16e8b_Address = address);
            _scanner.Scan("LiteralReplacement_140fb3d45", "3D 20 04 00 00 7C ?? 43 8D 04 ??",
                address => _LiteralReplacement_140fb3d45_Address = address);
            _scanner.Scan("LiteralReplacement_14073e903", "BB 20 04 00 00 48 8D 4D ??",
                address => _LiteralReplacement_14073e903_Address = address);
            _scanner.Scan("LiteralReplacement_14073d3f4", "BB 20 04 00 00 4C 89 A6 ?? ?? ?? ??",
                address => _LiteralReplacement_14073d3f4_Address = address);
            _scanner.Scan("LiteralReplacement_1409f00b6", "41 8B DF 41 BC 1E 03 00 00",
                address => _LiteralReplacement_1409f00b6_Address = address);
            _scanner.Scan("LiteralReplacement_140ad2dee", "48 89 5D ?? BA 1E 03 00 00",
                address => _LiteralReplacement_140ad2dee_Address = address);
            _scanner.Scan("LiteralReplacement_140ad3b24", "8B 4D ?? BA 1E 03 00 00",
                address => _LiteralReplacement_140ad3b24_Address = address);
            _scanner.Scan("LiteralReplacement_140ad98bf", "BE 1E 03 00 00 4C 89 9B ?? ?? ?? ??",
                address => _LiteralReplacement_140ad98bf_Address = address);
            _scanner.Scan("LiteralReplacement_140ad9b6b", "BE 1E 03 00 00 66 45 89 1C 24",
                address => _LiteralReplacement_140ad9b6b_Address = address);
            _scanner.Scan("LiteralReplacement_140aedd0a", "41 BB 1E 03 00 00",
                address => _LiteralReplacement_140aedd0a_Address = address);
            _scanner.Scan("LiteralReplacement_140b110f2", "41 BA 1E 03 00 00",
                address => _LiteralReplacement_140b110f2_Address = address);
            _scanner.Scan("LiteralReplacement_140d23a21", "B8 1E 04 00 00 8D 4B ??",
                address => _LiteralReplacement_140d23a21_Address = address);
            _scanner.Scan("LiteralReplacement_140d23a57", "B9 1E 03 00 00 8D 43 ??",
                address => _LiteralReplacement_140d23a57_Address = address);
            _scanner.Scan("LiteralReplacement_140d23af6", "B8 20 03 00 00 66 3B D8",
                address => _LiteralReplacement_140d23af6_Address = address);
            _scanner.Scan("LiteralReplacement_140d6b831", "B9 20 03 00 00 66 3B C1 73 ?? 0F B7 C0 48 8D 0C ?? 48 03 C9",
                address => _LiteralReplacement_140d6b831_Address = address);
            _scanner.Scan("LiteralReplacement_140d6b8e0", "B9 20 03 00 00 66 3B C1 73 ?? 0F B7 C0 48 8D 0C ?? 48 C1 E1 04",
                address => _LiteralReplacement_140d6b8e0_Address = address);
            _scanner.Scan("LiteralReplacement_140d6b9be", "B8 20 03 00 00 44 3B C0",
                address => _LiteralReplacement_140d6b9be_Address = address);
            _scanner.Scan("LiteralReplacement_140d6baaa", "B8 20 03 00 00 66 3B D0",
                address => _LiteralReplacement_140d6baaa_Address = address);
            _scanner.Scan("LiteralReplacement_140d6bd94", "B9 20 03 00 00 44 8B 42 ??",
                address => _LiteralReplacement_140d6bd94_Address = address);
            _scanner.Scan("LiteralReplacement_140d73e36", "B8 20 03 00 00 45 0F B7 94 ?? ?? ?? ?? ??",
                address => _LiteralReplacement_140d73e36_Address = address);
            _scanner.Scan("LiteralReplacement_140e2bc3b", "BA 1E 04 00 00",
                address => _LiteralReplacement_140e2bc3b_Address = address);
            _scanner.Scan("LiteralReplacement_140e2bc68", "BA 1E 03 00 00 8D 48 ?? 66 3B CA",
                address => _LiteralReplacement_140e2bc68_Address = address);
            _scanner.Scan("LiteralReplacement_14bc3c1c2", "BA 1E 03 00 00 8D 48 ?? 66 39 D1",
                address => _LiteralReplacement_14bc3c1c2_Address = address);

            
            /* _LiteralReplacement_14bc3c1c2_Address
            _scanner.Scan("NAMEGE", "PATTERNGE",
                address => ADRESSGE = address);
                */
            // fsadfjsdjflksjfdokjsfdj
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
                
                Buffer.MemoryCopy((byte*)_DAT_142260b80_Address, _pinnedSkillElements.Pointer,
                    VanillaSkillElementsLength * SkillElementLength, VanillaSkillElementsLength * SkillElementLength);
                _skillElementsOffset = (long)_pinnedSkillElements.Pointer - _DAT_142260b80_Address;
                
                Buffer.MemoryCopy((byte*)_DAT_142226bf0_Address, _pinnedActiveSkillData.Pointer,
                    VanillaActiveSkillDataLength * ActiveSkillDataLength, VanillaActiveSkillDataLength * ActiveSkillDataLength);
                _activeSkillDataOffset = (long)_pinnedActiveSkillData.Pointer - _DAT_142226bf0_Address;

                Log.Information("We do a little tomfoolery. Replacing bytes for skill id 1800 with bytes of 22 (Bufudyne)");
                Buffer.MemoryCopy(_pinnedSkillElements.Pointer + 22 * SkillElementLength, _pinnedSkillElements.Pointer + 1800 * SkillElementLength,
                    SkillElementLength, SkillElementLength);
                Buffer.MemoryCopy(_pinnedActiveSkillData.Pointer + 22 * ActiveSkillDataLength, _pinnedActiveSkillData.Pointer + 1800 * ActiveSkillDataLength,
                    ActiveSkillDataLength, ActiveSkillDataLength);
                
                Log.Information($"1800 test info. Element: {_pinnedSkillElements.Pointer[1800 * SkillElementLength]}, activatibility: {_pinnedSkillElements.Pointer[1800 * SkillElementLength + 1]}, area type: {_pinnedActiveSkillData.Pointer[1800 * ActiveSkillDataLength + 4]}");

                _memory = new(Process.GetCurrentProcess());
                
                AboardExclamationPoint();
            }
        }

        return result;
    }
    
    private unsafe void AboardExclamationPoint()
    {
        // Segment 1
        _customInstructionSetPointer = (long)_pinnedActiveSkillData.Pointer;
        _customInstructionSetOffset = _activeSkillDataOffset;
        
        // For DAT_142226bf0
        CreateHook(_LEA_140105ab5_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_TEST_140719e12_Address, CreateCustomInstructionSet([
            "test dword [r12 + rcx*0x8], 0x10000"
        ], "r12", false));
        
        CreateHook(_LEA_14071bd02_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_MOV_14073a0ae_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_MOV_14073c18b_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_TEST_14073c1df_Address, CreateCustomInstructionSet([
            "test dword [r12 + rax*0x8], 0x10000"
        ], "r12", false));
        
        CreateHook(_MOV_14073d50e_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_1407521d7_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140752496_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14075277d_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140752df5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140752f6b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140753697_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407536ca_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14075384d_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407538ad_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14075396b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140753af8_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140753c6a_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140753dfd_Address, [
            "use64",
            $"mov r13, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140753fe4_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407541de_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140754479_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14075e087_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140771dfc_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140771f54_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407724e7_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_TEST_14077b9eb_Address, CreateCustomInstructionSet([
            "test dword [r12 + rcx*0x8], 0x10000"
        ], "r12", false));
        
        CreateHook(_MOV_14077c383_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_1407afc81_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407aff57_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407b076e_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407b40a0_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407b81fb_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1407b9128_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140801ee2_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140810877_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140811470_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14081162d_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14085e961_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140868c5e_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140868f94_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14086fbc4_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140870ad8_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140871e93_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_MOV_1408ec3c4_Address, CreateCustomInstructionSet([
            "mov eax, dword [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_1409f0136_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140acc910_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140accb16_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140accf19_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140acd266_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140acd6f0_Address, [
            "use64",
            $"mov r14, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad431e_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad4414_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad4431_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad44d5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad4615_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad474b_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad47c4_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad48e6_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad55fa_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad5f65_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad653a_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad6671_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad707f_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad74cf_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad98cb_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140ad9b75_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140aedd10_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140aee4f6_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140aee8d0_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140aeec6a_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b0fd01_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b10242_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b6650b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b76766_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b768bd_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b768fa_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b7691c_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b769ca_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b76b0e_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b76c62_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140b76df8_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140d23a05_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140d6b75a_Address, [
            "use64",
            $"mov r12, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140d6b845_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140d6b8f5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140d6b925_Address, [
            "use64",
            $"mov r12, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140d6bd7d_Address, [
            "use64",
            $"mov r15, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140d73e5e_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140dbaaa7_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140dbac38_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e0fb52_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e10232_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_MOV_140e1072a_Address, CreateCustomInstructionSet([
            "mov edi, dword [r12 + rdx*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_140e1223f_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e12590_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_TEST_140e12933_Address, CreateCustomInstructionSet([
            "test byte [r14 + r12*0x1], 0x40"
        ], "r12", false));
        
        CreateHook(_TEST_140e13177_Address, CreateCustomInstructionSet([
            "test dword [r15 + r12*0x1], r14d"
        ], "r12", false));
        
        CreateHook(_MOV_140e1344d_Address, CreateCustomInstructionSet([
            "mov eax, dword [r15 + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_LEA_140e139f1_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e14347_Address, [
            "use64",
            $"mov r9, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e14502_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e1460a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        // Hooking this normally crashes the game. I was unable to figure out why
        CreateHook(_LEA_140e1492a_Address + 7, CreateCustomInstructionSet([
            "mov r10, r12"
        ], "r12", false), AsmHookBehaviour.ExecuteFirst);
        
        CreateHook(_LEA_140e1498c_Address, [
            "use64",
            $"mov r10, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e14b56_Address, [
            "use64",
            $"mov r13, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e14dd5_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_MOV_140e16e9e_Address, CreateCustomInstructionSet([
            "mov ecx, dword [r12 + rax*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_140e1b1ef_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e1b2bd_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e1b2f3_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e1b31b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e1b3b1_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e1c435_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e32ef3_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e32f0a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e32f47_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e3318a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e3330a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e3342a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_140e3344a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_MOV_140e3358a_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_MOV_140e33933_Address, CreateCustomInstructionSet([
            "mov eax, dword [rax + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_MOV_140e34230_Address, CreateCustomInstructionSet([
            "mov eax, dword [rax + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_MOV_140e34438_Address, CreateCustomInstructionSet([
            "mov eax, dword [rax + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_MOV_140e34732_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_MOV_140e34750_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_MOV_140e34769_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_MOV_140e34782_Address, CreateCustomInstructionSet([
            "mov eax, dword [r14 + r12*0x1]"
        ], "r12", false));
        
        CreateHook(_LEA_140e3eb53_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_1417b4694_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14a8ab88f_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        CreateHook(_LEA_14bd93fea_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer}"
        ]);
        
        // For DAT_142226bf4
        CreateHook(_TEST_140764760_Address, CreateCustomInstructionSet([
            "test byte [r12 + rax*0x8 + 0x4], 0x2"
        ], "r12", false));
        
        CreateHook(_TEST_140ad2e25_Address, CreateCustomInstructionSet([
            "test byte [r13 + r12*0x8 + 0x4], 0x2"
        ], "r13", false));
        
        CreateHook(_LEA_140b110f8_Address, [
            "use64",
            $"mov r11, {(ulong)_pinnedActiveSkillData.Pointer + 4}"
        ]);
        
        CreateHook(_TEST_140b31f96_Address, CreateCustomInstructionSet([
            "test byte [rax + r12*0x1 + 0x4], 0x2"
        ], "r12", false));
        
        CreateHook(_TEST_140b32a54_Address, CreateCustomInstructionSet([
            "test byte [rax + r12*0x1 + 0x4], 0x2"
        ], "r12", false));
        
        CreateHook(_TEST_140b74509_Address, CreateCustomInstructionSet([
            "test byte [rax + r12*0x1 + 0x4], 0x2"
        ], "r12", false));
        
        CreateHook(_LEA_140e2bc7f_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer + 4}"
        ]);
        
        CreateHook(_LEA_14bc3c1d9_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 4}"
        ]);
        
        // For DAT_142226bf6
        CreateHook(_LEA_140101809_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 6}"
        ]);
        
        CreateHook(_LEA_1401018a9_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 6}"
        ]);
        
        CreateHook(_CMP_14077bd39_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8 + 0x6], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_14077f324_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8 + 0x6], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_14077ffe1_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8 + 0x6], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_140911e33_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + r9*0x8 + 0x6], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_1409257c3_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + r9*0x8 + 0x6], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_140911f4c_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8 + 0x6], 0x2"
        ], "r12", false));
        
        CreateHook(_CMP_1409258dc_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8 + 0x6], 0x2"
        ], "r12", false));
        
        CreateHook(_CMP_140950c7e_Address, CreateCustomInstructionSet([
            "cmp byte [r13 + rax*0x8 + 0x6], 0x1"
        ], "r13", false));
        
        CreateHook(_LEA_140e3335c_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 6}"
        ]);
        
        // For DAT_142226bf7
        CreateHook(_MOVZX_14073aa19_Address, CreateCustomInstructionSet([
            "movzx ebx, byte [r12 + rcx*0x8 + 0x7]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140ac6527_Address, CreateCustomInstructionSet([
            "movzx edi, byte [rax + r12*0x1 + 0x7]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140d07c3b_Address, CreateCustomInstructionSet([
            "movzx ecx, byte [r12 + rax*0x8 + 0x7]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140d0ac7c_Address, CreateCustomInstructionSet([
            "movzx ecx, byte [r12 + rax*0x8 + 0x7]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e335c8_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r14 + r12*0x1 + 0x7]"
        ], "r12", false));
        
        CreateHook(_LEA_14171e63b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 7}"
        ]);
        
        CreateHook(_MOVZX_14177b5ae_Address, CreateCustomInstructionSet([
            "movzx ecx, byte [r12 + rax*0x8 + 0x7]"
        ], "r12", false));
        
        CreateHook(_LEA_141780be8_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 7}"
        ]);
        
        // For DAT_142226bfc
        CreateHook(_CMP_14074211f_Address, CreateCustomInstructionSet([
            "cmp byte [r13 + rcx*0x8 + 0xc], 0x0"
        ], "r13", false));
        
        CreateHook(_CMP_140742760_Address, CreateCustomInstructionSet([
            "cmp byte [r13 + rcx*0x8 + 0xc], 0x0"
        ], "r13", false));
        
        CreateHook(_MOVZX_140770766_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0xc]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140770911_Address, CreateCustomInstructionSet([
            "movzx r12d, byte [r13 + rcx*0x8 + 0xc]"
        ], "r13", false));
        
        CreateHook(_LEA_1407b8b36_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_CMP_1407ba966_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8 + 0xc], dil"
        ], "r12", false));
        
        CreateHook(_MOVZX_1407bde19_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0xc]"
        ], "r12", false));
        
        CreateHook(_MOVZX_1407be616_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0xc]"
        ], "r12", false));
        
        CreateHook(_LEA_1407bf421_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407c84a2_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407e5a4e_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407ecfc0_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407ed288_Address, [
            "use64",
            $"mov rbx, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407ed9e6_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407edc4a_Address, [
            "use64",
            $"mov r8, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407ee1dd_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407ee483_Address, [
            "use64",
            $"mov rdx, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407efdb8_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f1a15_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f2964_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f32f4_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f5514_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f5a63_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f6223_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f6d53_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f83d3_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f9327_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_1407f9ac3_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_CMP_1407fa38f_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + r15*0x8 + 0xc], r14b"
        ], "r12", false));
        
        CreateHook(_MOVZX_140800dbc_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rax*0x8 + 0xc]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140808efa_Address, CreateCustomInstructionSet([
            "movzx r13d, byte [r12 + rcx*0x8 + 0xc]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140809ad7_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0xc]"
        ], "r12", false));
        
        CreateHook(_MOVZX_1409ec814_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0xc]"
        ], "r12", false));
        
        CreateHook(_CMP_140ac4cb6_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0xc], r14b"
        ], "r12", false));
        
        CreateHook(_LEA_140acc3d0_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_MOVZX_140ad3048_Address, CreateCustomInstructionSet([
            "movzx esi, byte [r13 + r12*0x8 + 0xc]"
        ], "r13", false));
        
        CreateHook(_MOVZX_140ad37ee_Address, CreateCustomInstructionSet([
            "movzx esi, byte [r13 + r12*0x8 + 0xc]"
        ], "r13", false));
        
        CreateHook(_LEA_140ada874_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_140b5115b_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_CMP_140b53c15_Address, CreateCustomInstructionSet([
            "cmp byte [r14 + rcx*0x8 + 0xc], r12b"
        ], "r14", false));
        
        CreateHook(_LEA_140b7daa1_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_140b7ec53_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_140b82003_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_MOVZX_140d23838_Address, CreateCustomInstructionSet([
            "movzx eax, byte [rcx + r12*0x1 + 0xc]"
        ], "r12", false));
        
        CreateHook(_LEA_140d73d5f_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_LEA_140e3519a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 12}"
        ]);
        
        CreateHook(_MOVZX_14123d8c3_Address, CreateCustomInstructionSet([
            "movzx ecx, byte [rax + r12*0x1 + 0xc]"
        ], "r12", false));
        
        // For DAT_142226bfd
        CreateHook(_MOVZX_1407ba194_Address, CreateCustomInstructionSet([
            "movzx r15d, byte [rbx + r12*0x1 + 0xd]"
        ], "r12", false));
        
        CreateHook(_LEA_140ad5841_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer + 13}"
        ]);
        
        CreateHook(_LEA_140ad5bb6_Address, [
            "use64",
            $"mov rcx, {(ulong)_pinnedActiveSkillData.Pointer + 13}"
        ]);
        
        CreateHook(_LEA_140b764d7_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 13}"
        ]);
        
        CreateHook(_LEA_14bd9429a_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 13}"
        ]);
        
        // For DAT_142226bfe
        CreateHook(_MOVZX_1407ba19d_Address, CreateCustomInstructionSet([
            "movzx eax, byte [rbx + r12*0x1 + 0xe]"
        ], "r12", false));
        
        // For DAT_142226c00
        CreateHook(_LEA_1407ba233_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 16}"
        ]);
        
        // For DAT_142226c04
        CreateHook(_CMP_140e12a8d_Address, CreateCustomInstructionSet([
            "cmp byte [rdi + r12*0x1 + 0x14], r13b"
        ], "r12", false));
        
        CreateHook(_CMP_140e12b6a_Address, CreateCustomInstructionSet([
            "cmp byte [rdi + r12*0x1 + 0x14], 0x64"
        ], "r12", false));
        
        // For DAT_142226c06
        CreateHook(_MOVZX_1409f1a37_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0x16]"
        ], "r12", false));
        
        // For DAT_142226c07
        CreateHook(_MOVZX_1400fa0d5_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r8*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_1407fac4b_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r15*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140883961_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rbx*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_14088476c_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rbx*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_CMP_1408a8c77_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rdx*0x8 + 0x17], 0x4"
        ], "r12", false));
        
        CreateHook(_MOVZX_1408eb238_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140911e42_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r9*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140911f5b_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_1409257d2_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r9*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_1409258eb_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_1409ec823_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_CMP_140e12342_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rbx*0x8 + 0x17], 0x0"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e12850_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r14 + r12*0x1 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e332ad_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r8*0x8 + 0x17]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e3348d_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r8*0x8 + 0x17]"
        ], "r12", false));
        
        // For DAT_142226c0a
        CreateHook(_MOVZX_1408eb24d_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0x1a]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e332a2_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r8*0x8 + 0x1a]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e33482_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r8*0x8 + 0x1a]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e347d8_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r14 + r12*0x1 + 0x1a]"
        ], "r12", false));
        
        // For DAT_142226c0e
        CreateHook(_CMP_1400f9485_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8 + 0x1e], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_1400fa0fb_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + r8*0x8 + 0x1e], 0x1"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e10715_Address, CreateCustomInstructionSet([
            "movzx r15d, byte [r13 + rdx*0x8 + 0x1e]"
        ], "r13", false));
        
        CreateHook(_CMP_140e110c9_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x1e], 0x3"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e11193_Address, CreateCustomInstructionSet([
            "movzx eax, byte [rax + r12*0x1 + 0x1e]"
        ], "r12", false));
        
        CreateHook(_CMP_140e11dee_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x1e], 0x2"
        ], "r12", false));
        
        CreateHook(_CMP_140e1234c_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rbx*0x8 + 0x1e], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_140e1238f_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rbx*0x8 + 0x1e], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_140e13023_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x1e], sil"
        ], "r12", false));
        
        CreateHook(_CMP_140e33a92_Address, CreateCustomInstructionSet([
            "cmp byte [rcx + r12*0x1 + 0x1e], 0x2"
        ], "r12", false));
        
        CreateHook(_CMP_140e33aee_Address, CreateCustomInstructionSet([
            "cmp byte [rcx + r12*0x1 + 0x1e], 0x1"
        ], "r12", false));
        
        CreateHook(_CMP_140e347c3_Address, CreateCustomInstructionSet([
            "cmp byte [r14 + r12*0x1 + 0x1e], 0x1"
        ], "r12", false));
        
        // For DAT_142226c0f
        CreateHook(_CMP_1400f948f_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8 + 0x1f], 0x0"
        ], "r12", false));
        
        CreateHook(_CMP_1400fa10a_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + r8*0x8 + 0x1f], 0x0"
        ], "r12", false));
        
        CreateHook(_CMP_140e11de0_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x1f], 0x64"
        ], "r12", false));
        
        CreateHook(_CMP_140e12399_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rbx*0x8 + 0x1f], 0x64"
        ], "r12", false));
        
        CreateHook(_CMP_140e1302d_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x1f], dil"
        ], "r12", false));
        
        CreateHook(_CMP_140e347ce_Address, CreateCustomInstructionSet([
            "cmp byte [r14 + r12*0x1 + 0x1f], al"
        ], "r12", false));
        
        // For DAT_142226c10
        CreateHook(_TEST_1400f9499_Address, CreateCustomInstructionSet([
            "test dword [r12 + rcx*0x8 + 0x20], 0x80000"
        ], "r12", false));
        
        CreateHook(_TEST_1400fa119_Address, CreateCustomInstructionSet([
            "test dword [r12 + r8*0x8 + 0x20], 0x80000"
        ], "r12", false));
        
        CreateHook(_TEST_1407703a6_Address, CreateCustomInstructionSet([
            "test dword [rbx + r12*0x1 + 0x20], eax"
        ], "r12", false));
        
        CreateHook(_LEA_14080e0c4_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 32}"
        ]);
        
        CreateHook(_TEST_1408a8c39_Address, CreateCustomInstructionSet([
            "test dword [r12 + rdx*0x8 + 0x20], 0x80000"
        ], "r12", false));
        
        CreateHook(_MOV_140e110a9_Address, CreateCustomInstructionSet([
            "mov ebp, dword [rax + r12*0x1 + 0x20]"
        ], "r12", false));
        
        CreateHook(_TEST_140e1235a_Address, CreateCustomInstructionSet([
            "test dword [r12 + rbx*0x8 + 0x20], 0x80000"
        ], "r12", false));
        
        CreateHook(_TEST_140e123a3_Address, CreateCustomInstructionSet([
            "test dword [r12 + rbx*0x8 + 0x20], 0x100000"
        ], "r12", false));
        
        CreateHook(_TEST_140e12a9b_Address, CreateCustomInstructionSet([
            "test dword [rdi + r12*0x1 + 0x20], 0x80000"
        ], "r12", false));
        
        CreateHook(_TEST_140e13037_Address, CreateCustomInstructionSet([
            "test dword [rax + r12*0x1 + 0x20], 0x80000"
        ], "r12", false));
        
        CreateHook(_TEST_140e33af8_Address, CreateCustomInstructionSet([
            "test dword [rcx + r12*0x1 + 0x20], 0x80000"
        ], "r12", false));
        
        // For DAT_142226c14
        CreateHook(_TEST_1400fa0a0_Address, CreateCustomInstructionSet([
            "test dword [r12 + rcx*0x8 + 0x24], 0x300"
        ], "r12", false));
        
        CreateHook(_TEST_1400fa1da_Address, CreateCustomInstructionSet([
            "test dword [r12 + r8*0x8 + 0x24], 0x300"
        ], "r12", false));
        
        CreateHook(_LEA_140ad7871_Address, [
            "use64",
            $"mov rax, {(ulong)_pinnedActiveSkillData.Pointer + 36}"
        ]);
        
        CreateHook(_MOV_140e33b47_Address, CreateCustomInstructionSet([
            "mov ebx, dword [rax + r12*0x1 + 0x24]"
        ], "r12", false));
        
        // For DAT_142226c18
        CreateHook(_MOV_140e33b4e_Address, CreateCustomInstructionSet([
            "mov eax, dword [rax + r12*0x1 + 0x28]"
        ], "r12", false));
        
        // For DAT_142226c1c
        CreateHook(_MOVZX_1400fa087_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + rcx*0x8 + 0x2c]"
        ], "r12", false));
        
        CreateHook(_MOVZX_1400fa1c7_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + r8*0x8 + 0x2c]"
        ], "r12", false));
        
        CreateHook(_CMP_1407ba25f_Address, CreateCustomInstructionSet([
            "cmp byte [rbx + r12*0x1 + 0x2c], 0xf"
        ], "r12", false));
        
        CreateHook(_TEST_1408a8c2f_Address, CreateCustomInstructionSet([
            "test byte [r12 + rdx*0x8 + 0x2c], r15b"
        ], "r12", false));
        
        CreateHook(_CMP_140ac516e_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x2c], 0x12"
        ], "r12", false));
        
        CreateHook(_CMP_140ac59fa_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x2c], 0x12"
        ], "r12", false));
        
        CreateHook(_CMP_140e104af_Address, CreateCustomInstructionSet([
            "cmp byte [rcx + r12*0x1 + 0x2c], 0x6"
        ], "r12", false));
        
        CreateHook(_CMP_140e106ea_Address, CreateCustomInstructionSet([
            "cmp byte [r13 + rdx*0x8 + 0x2c], 0xd"
        ], "r13", false));
        
        CreateHook(_CMP_140e11083_Address, CreateCustomInstructionSet([
            "cmp byte [rax + r12*0x1 + 0x2c], 0xd"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e111c8_Address, CreateCustomInstructionSet([
            "movzx edx, byte [rcx + r12*0x1 + 0x2c]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e12668_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r14 + r12*0x1 + 0x2c]"
        ], "r12", false));
        
        CreateHook(_CMP_140e12a2c_Address, CreateCustomInstructionSet([
            "cmp byte [r14 + r12*0x1 + 0x2c], 0x12"
        ], "r12", false));
        
        CreateHook(_CMP_140e33a71_Address, CreateCustomInstructionSet([
            "cmp byte [rcx + r12*0x1 + 0x2c], 0x15"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e33cd4_Address, CreateCustomInstructionSet([
            "movzx eax, byte [rax + r12*0x1 + 0x2c]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e34851_Address, CreateCustomInstructionSet([
            "movzx ecx, byte [r14 + r12*0x1 + 0x2c]"
        ], "r12", false));
        
        // For DAT_142226c1d
        CreateHook(_MOVZX_140e4035c_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + 0x2d]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140e40427_Address, CreateCustomInstructionSet([
            "movzx eax, byte [r12 + 0x2d]"
        ], "r12", false));
        
        // Segment 0
        _customInstructionSetPointer = (long)_pinnedSkillElements.Pointer;
        _customInstructionSetOffset = _skillElementsOffset;
        
        // For DATx142260b80
        CreateHook(_LEA_14071a1ce_Address, [
            "use64",
            $"mov r14, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_14071a445_Address, [
            "use64",
            $"mov r15, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_14073acb7_Address, CreateCustomInstructionSet([
            "lea rcx, [rcx + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_CMP_14073ad65_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8], 0x15"
        ], "r12", false));
        
        CreateHook(_LEA_14073ae82_Address, CreateCustomInstructionSet([
            "lea rcx, [rcx + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_CMP_14073b083_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8], 0x15"
        ], "r12", false));
        
        CreateHook(_LEA_14073b110_Address, CreateCustomInstructionSet([
            "lea rcx, [rcx + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_CMP_14076476f_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rbx*0x8], 0xa"
        ], "r12", false));
        
        CreateHook(_MOVSX_14076487d_Address, CreateCustomInstructionSet([
            "movsx r8d, byte [r12 + rbx*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_1407b9f92_Address, CreateCustomInstructionSet([
            "lea r8, [r8 + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_MOVSX_1407fb6f3_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + r14*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_1407fb805_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + r14*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_1407fdee4_Address, CreateCustomInstructionSet([
            "movsx eax, byte [r12]"
        ], "r12", false));
        
        CreateHook(_MOVSX_1407fdf5a_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12]"
        ], "r12", false));
        
        CreateHook(_LEA_14080095e_Address, CreateCustomInstructionSet([
            "lea rsi, [rsi + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LEA_14080095e_Address, [
            "use64",
            $"mov rsi, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140801ef4_Address, [
            "use64",
            $"mov r8, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_MOVSX_140807419_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_140808e2a_Address, CreateCustomInstructionSet([
            "movsx ecx, byte [r12 + rax*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_140815aab_Address, CreateCustomInstructionSet([
            "lea r15, [r15 + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_MOVSX_1408eb262_Address, CreateCustomInstructionSet([
            "movsx eax, byte [r12 + rdx*0x8]"
        ], "r12", false));
        
        CreateHook(_CMP_140911f79_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8], 0x15"
        ], "r12", false));
        
        CreateHook(_CMP_140925909_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8], 0x15"
        ], "r12", false));
        
        CreateHook(_LEA_140911fb8_Address, [
            "use64",
            $"mov rcx, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140925948_Address, [
            "use64",
            $"mov rcx, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_MOVSX_140ac50f3_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + rbx*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_140ac5983_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + rbx*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_140ad202a_Address, CreateCustomInstructionSet([
            "lea rsi, [rsi + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LEA_140ad241e_Address, CreateCustomInstructionSet([
            "lea rsi, [rsi + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LEA_140ad2745_Address, CreateCustomInstructionSet([
            "lea r14, [r14 + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LEA_140b31af6_Address, CreateCustomInstructionSet([
            "lea rdi, [rdi + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LEA_140b31fa5_Address, [
            "use64",
            $"mov r15, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140b325d8_Address, [
            "use64",
            $"mov rdi, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140b32a62_Address, [
            "use64",
            $"mov r14, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_CMP_140b7469f_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + r14*0x8], 0xa"
        ], "r12", false));
        
        CreateHook(_MOVSX_140b748c0_Address, CreateCustomInstructionSet([
            "movsx eax, byte [r12 + r14*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_140b74973_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + r14*0x8]"
        ], "r12", false));
        
        CreateHook(_CMP_140d71d6a_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8], al"
        ], "r12", false));
        
        CreateHook(_CMP_140d72d0c_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8], al"
        ], "r12", false));
        
        CreateHook(_CMP_140d73094_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8], al"
        ], "r12", false));
        
        CreateHook(_CMP_140e16eab_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + r9*0x8], 0xa"
        ], "r12", false));
        
        CreateHook(_MOVSX_140e16f21_Address, CreateCustomInstructionSet([
            "movsx eax, byte [r12 + r9*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_140e16f74_Address, [
            "use64",
            $"mov rcx, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140e32879_Address, [
            "use64",
            $"mov rdx, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140e32cd6_Address, CreateCustomInstructionSet([
            "lea r9, [r9 + r12]"
        ], "r12", true), AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LEA_140e33076_Address, [
            "use64",
            $"mov r15, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140e331d6_Address, [
            "use64",
            $"mov r15, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140e3eb73_Address, [
            "use64",
            $"mov rcx, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140e4051a_Address, [
            "use64",
            $"mov r10, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140fae067_Address, [
            "use64",
            $"mov rdx, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_MOVSX_140fb00f7_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_140fb0317_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + rcx*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_140fb0c79_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + rax*0x8]"
        ], "r12", false));
        
        CreateHook(_MOVSX_140fb0e20_Address, CreateCustomInstructionSet([
            "movsx edx, byte [r12 + rax*0x8]"
        ], "r12", false));
        
        CreateHook(_LEA_140fb3c8f_Address, [
            "use64",
            $"mov r13, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_140fb8094_Address, [
            "use64",
            $"mov r10, {_customInstructionSetPointer}"
        ]);
        
        CreateHook(_LEA_141780647_Address, [
            "use64",
            $"mov rcx, {_customInstructionSetPointer}"
        ]);
        
        // For DATx142260b81
        CreateHook(_CMP_14077033f_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8 + 0x1], 0x0"
        ], "r12", false));
        
        CreateHook(_CMP_140883791_Address, CreateCustomInstructionSet([
            "cmp byte [r13 + rax*0x8 + 0x1], r12b"
        ], "r13", false));
        
        CreateHook(_CMP_14088459c_Address, CreateCustomInstructionSet([
            "cmp byte [r13 + rax*0x8 + 0x1], r12b"
        ], "r13", false));
        
        CreateHook(_CMP_1409ec802_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8 + 0x1], 0x0"
        ], "r12", false));
        
        CreateHook(_CMP_140d07c26_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rax*0x8 + 0x1], 0x0"
        ], "r12", false));
        
        CreateHook(_CMP_140d0ac66_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rcx*0x8 + 0x1], 0x0"
        ], "r12", false));
        
        CreateHook(_LEA_140e35393_Address, [
            "use64",
            $"mov rcx, {_customInstructionSetPointer + 1}"
        ]);
        
        CreateHook(_CMP_14177b599_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rbx*0x8 + 0x1], 0x0"
        ], "r12", false));
        
        // For DATx142260b82
        CreateHook(_MOVZX_140fa4eba_Address, CreateCustomInstructionSet([
            "movzx r10d, byte [r12 + rbp*0x8 + 0x2]"
        ], "r12", false));
        
        CreateHook(_CMP_140fa4ec6_Address, CreateCustomInstructionSet([
            "cmp byte [r12 + rdx*0x8 + 0x2], r10b"
        ], "r12", false));
        
        CreateHook(_LEA_140faf2c4_Address, [
            "use64",
            $"mov rcx, {_customInstructionSetPointer + 2}"
        ]);
        
        CreateHook(_LEA_140faf7db_Address, [
            "use64",
            $"mov r9, {_customInstructionSetPointer + 2}"
        ]);
        
        CreateHook(_MOVZX_140fb0100_Address, CreateCustomInstructionSet([
            "movzx r10d, byte [r12 + rcx*0x8 + 0x2]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140fb031f_Address, CreateCustomInstructionSet([
            "movzx r11d, byte [r12 + rcx*0x8 + 0x2]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140fb0c82_Address, CreateCustomInstructionSet([
            "movzx r8d, byte [r12 + rax*0x8 + 0x2]"
        ], "r12", false));
        
        CreateHook(_MOVZX_140fb0e30_Address, CreateCustomInstructionSet([
            "movzx r8d, byte [r12 + rax*0x8 + 0x2]"
        ], "r12", false));
        
        CreateHook(_LEA_140fb3803_Address, [
            "use64",
            $"mov r9, {_customInstructionSetPointer + 2}"
        ]);
        
        // Funny literal replacements
        _customInstructionSetPointer = 0;
        _customInstructionSetOffset = 0;
        
        CreateHook(_LiteralReplacement_140fa4eda_Address, [
            "use64",
            "inc eax",
            $"cmp eax, {ExpandedSkillElementsLength}"
        ]);
        
        CreateHook(_LiteralReplacement_140faf7ed_Address, [
            "use64",
            $"mov edi, {ExpandedSkillElementsLength}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140fb3814_Address, [
            "use64",
            $"mov r11d, {ExpandedSkillElementsLength}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140fb3d43_Address, [
            "use64",
            "inc eax",
            $"cmp eax, {ExpandedSkillElementsLength}"
        ]);
        
        /*
        CreateHook(_LiteralReplacement_14076ff34_Address, [ // May or may not be related. Something to do with skill activatibility
            "use64",
            $"lea rcx, [r13 + {ExpandedSkillElementsLength}]"
        ]);
        
        CreateHook(_LiteralReplacement_14073b876_Address, [ // May or may not be related. Something to do with skill element
            "use64",
            $"lea rdx, [r13 + {ExpandedSkillElementsLength}]"
        ]);
        
        CreateHook(_LiteralReplacement_14073b458_Address, [ // May or may not be related. Something to do with skill element
            "use64",
            $"lea rdx, [r13 + {ExpandedSkillElementsLength}]"
        ]);
        */
        
        CreateHook(_LiteralReplacement_14073c1c6_Address, [
            "use64",
            "xor cl, cl",
            $"mov edx, {ExpandedActiveSkillDataLength}"
        ]);
        
        CreateHook(_LiteralReplacement_14076472e_Address, [
            "use64",
            $"mov edx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_1407b9f6c_Address, [
            "use64",
            $"mov edx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        // Cannot replace this normally
        _memory.Write((UIntPtr)_LiteralReplacement_14080093d_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        CreateHook(_LiteralReplacement_140801ec9_Address, [
            "use64",
            $"mov ecx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140802097_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140800b29_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        _memory.Write((UIntPtr)_LiteralReplacement_1407ba0c1_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140764905_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        CreateHook(_LiteralReplacement_1408073f8_Address, [
            "use64",
            $"mov r9d, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140808dab_Address, [
            "use64",
            $"mov ecx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        _memory.Write((UIntPtr)_LiteralReplacement_14080988e_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        CreateHook(_LiteralReplacement_140b31f61_Address, [
            "use64",
            $"mov ecx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140b3241f_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        CreateHook(_LiteralReplacement_140b32a23_Address, [
            "use64",
            $"mov edx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140b32ec1_Address + 1, ExpandedActiveSkillDataLength - 1);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140b744d1_Address + 1, ExpandedActiveSkillDataLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140b74c2e_Address + 1, ExpandedActiveSkillDataLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140e16e8b_Address + 2, ExpandedActiveSkillDataLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140fb3d45_Address + 1, ExpandedSkillElementsLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_14073e903_Address + 1, ExpandedSkillElementsLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_14073d3f4_Address + 1, ExpandedSkillElementsLength);
        
        CreateHook(_LiteralReplacement_1409f00b6_Address, [
            "use64",
            $"mov r12d, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140ad2dee_Address, [
            "use64",
            $"mov edx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140ad3b24_Address, [
            "use64",
            $"mov edx, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140ad98bf_Address, [
            "use64",
            $"mov esi, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140ad9b6b_Address, [
            "use64",
            $"mov esi, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140aedd0a_Address, [
            "use64",
            $"mov r11d, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140b110f2_Address, [
            "use64",
            $"mov r10d, {ExpandedActiveSkillDataLength - 1}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140d23a21_Address, [
            "use64",
            $"mov eax, {ExpandedSkillElementsLength - 2}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140d23a57_Address, [
            "use64",
            $"mov ecx, {ExpandedActiveSkillDataLength - 2}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140d23af6_Address + 1, ExpandedActiveSkillDataLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140d6b831_Address + 1, ExpandedActiveSkillDataLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140d6b8e0_Address + 1, ExpandedActiveSkillDataLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140d6b9be_Address + 1, ExpandedActiveSkillDataLength);
        
        _memory.Write((UIntPtr)_LiteralReplacement_140d6baaa_Address + 1, ExpandedActiveSkillDataLength);
        
        CreateHook(_LiteralReplacement_140d6bd94_Address, [
            "use64",
            $"mov ecx, {ExpandedActiveSkillDataLength}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140d73e36_Address, [
            "use64",
            $"mov eax, {ExpandedActiveSkillDataLength}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140e2bc3b_Address, [
            "use64",
            $"mov edx, {ExpandedSkillElementsLength - 2}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_140e2bc68_Address, [
            "use64",
            $"mov edx, {ExpandedActiveSkillDataLength - 2}"
        ], AsmHookBehaviour.ExecuteAfter);
        
        CreateHook(_LiteralReplacement_14bc3c1c2_Address, [
            "use64",
            $"mov edx, {ExpandedActiveSkillDataLength - 2}"
        ], AsmHookBehaviour.ExecuteAfter);
        

        // _LiteralReplacement_14bc3c1c2_Address

        // fsadfjsdjflksjfdokjsfdj
    }
    
    private void CreateHook(long address, string[] function, AsmHookBehaviour asmHookBehaviour = AsmHookBehaviour.DoNotExecuteOriginal)
    {
        _asmHooks.Add(_hooks!.CreateAsmHook(function, address, asmHookBehaviour).Activate());
    }

    private string[] CreateCustomInstructionSet(string[] customInstructions, string dataOffsetRegister, bool registerIsOffset)
    {
        _customInstructionSetList.Clear();
        
        _customInstructionSetList.AddRange([
            "use64",
            $"push {dataOffsetRegister}",
            registerIsOffset ? $"mov {dataOffsetRegister}, {_customInstructionSetOffset}" : $"mov {dataOffsetRegister}, {_customInstructionSetPointer}"
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
﻿using System.Diagnostics;
using Project.Utils;
using Reloaded.Hooks.Definitions;
using IReloadedHooks = Reloaded.Hooks.Definitions.IReloadedHooks;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;

namespace SkillTableExpanded;

public class ScannerWrapper
{
    private static readonly nint BaseAddress = Process.GetCurrentProcess().MainModule?.BaseAddress ?? 0;
    
    private readonly IStartupScanner _scanner;
    private readonly IReloadedHooks _hooks;

    public ScannerWrapper(IStartupScanner scanner, IReloadedHooks hooks)
    {
        _scanner = scanner;
        _hooks = hooks;
    }

    public void ScanForData(string name, string pattern, int instructionLength, int instructionOffset, int extraOffset, Action<nint> callback)
    {
        _scanner.AddMainModuleScan(pattern, result =>
        {
            unsafe
            {
                if (!result.Found)
                {
                    Log.Error($"Failed to find pattern for {name}. Pattern: {pattern}");
                
                    return;
                }

                var baseAddress = BaseAddress + result.Offset;
            
                Log.Information($"{name} found at: {baseAddress:X}");
            
                var offsetAddress = baseAddress + instructionOffset;
                var offsetAddressPointer = (int*)offsetAddress;
                var offset = *offsetAddressPointer;

                callback(baseAddress + instructionLength + offset + extraOffset);
            }
        });
    }

    public void GetFunctionHook<T>(string name, string pattern, T function, Action<IHook<T>> callback)
    {
        _scanner.AddMainModuleScan(pattern, result =>
        {
            if (!result.Found)
            {
                Log.Error($"Failed to find pattern for {name}. Pattern: {pattern}");
                
                return;
            }

            var address = BaseAddress + result.Offset;
            
            Log.Information($"{name} found at: {address:X}");
            
            callback(_hooks.CreateHook(function, address).Activate());
        });
    }
    
    public void GetFunctionHook<T>(string name, string pattern, int instructionLength, int instructionOffset, int extraOffset, T function, Action<IHook<T>> callback)
    {
        _scanner.AddMainModuleScan(pattern, result =>
        {
            unsafe
            {
                if (!result.Found)
                {
                    Log.Error($"Failed to find pattern for {name}. Pattern: {pattern}");
                
                    return;
                }

                var baseAddress = BaseAddress + result.Offset;
            
                Log.Information($"{name} found at: {baseAddress:X}");
            
                var offsetAddress = baseAddress + instructionOffset;
                var offsetAddressPointer = (int*)offsetAddress;
                var offset = *offsetAddressPointer;
                
                callback(_hooks.CreateHook(function, baseAddress + instructionLength + offset + extraOffset).Activate());
            }
        });
    }

    public void GetFunctionWrapper<T>(string name, string pattern, Action<T> callback)
    {
        _scanner.AddMainModuleScan(pattern, result =>
        {
            if (!result.Found)
            {
                Log.Error($"Failed to find pattern for {name}. Pattern: {pattern}");
                
                return;
            }

            var address = BaseAddress + result.Offset;
            
            Log.Information($"{name} found at: {address:X}");

            callback(_hooks.CreateWrapper<T>(address, out _));
        });
    }
    
    public void GetFunctionWrapper<T>(string name, string pattern, int instructionLength, int instructionOffset, int extraOffset, Action<T> callback)
    {
        _scanner.AddMainModuleScan(pattern, result =>
        {
            unsafe
            {
                if (!result.Found)
                {
                    Log.Error($"Failed to find pattern for {name}. Pattern: {pattern}");
                
                    return;
                }

                var baseAddress = BaseAddress + result.Offset;
            
                Log.Information($"{name} found at: {baseAddress:X}");
            
                var offsetAddress = baseAddress + instructionOffset;
                var offsetAddressPointer = (int*)offsetAddress;
                var offset = *offsetAddressPointer;

                callback(_hooks.CreateWrapper<T>(baseAddress + instructionLength + offset + extraOffset, out _));
            }
        });
    }
    
    public void Scan(string name, string pattern, Action<nint> callback)
    {
        _scanner.AddMainModuleScan(pattern, result =>
        {
            if (!result.Found)
            {
                Log.Error($"Failed to find pattern for {name}. Pattern: {pattern}");
                
                return;
            }

            var address = BaseAddress + result.Offset;
            
            Log.Information($"{name} found at: {address:X}");
            
            callback(address);
        });
    }
}
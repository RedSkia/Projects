

using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ILGuard;




public static class AssemblyEx
{
    public static bool TryLoadAssembly(string path, [NotNullWhen(true)] out AssemblyDefinition? assembly, ReaderParameters? parameters = null)
    {
        assembly = null;
        parameters ??= new ReaderParameters
        {
            ReadSymbols = false,
            InMemory = true,
            ReadingMode = ReadingMode.Deferred,
            ReadWrite = false,
        };
        try
        {
            assembly = AssemblyDefinition.ReadAssembly(path, parameters);
            return assembly is not null;
        }
        catch
        {
            return false;
        }
    }

    public static uint CountILInstructions(this AssemblyDefinition assembly)
    {
        
        uint total = 0;
        foreach (var module in assembly.Modules)
        {
            var stack = new Stack<TypeDefinition>(module.Types);
            while (stack.Count > 0)
            {
                var type = stack.Pop();
                var methods = type.Methods;
                for (int i = 0; i < methods.Count; i++)
                {
                    var method = methods[i];
                    if (method.HasBody)
                        total += (uint)method.Body.Instructions.Count;
                }

                var nested = type.NestedTypes;
                for (int i = 0; i < nested.Count; i++)
                    stack.Push(nested[i]);
            }
        }

        return total;
    }


    public static AssemblyDefinition MergeAssemblies(params AssemblyDefinition[]? assemblies)
    {
        if ((assemblies?.Length ?? 0) < 2)
            throw new ArgumentException("At least two assemblies must be provided.", nameof(assemblies));

        // Pick primary assembly (first one)
        var primary = assemblies[0];

        // Collect entry points from all assemblies
        var entryPoints = assemblies
            .Select(a => a.EntryPoint)
            .Where(ep => ep != null)
            .ToList();

        // If primary has no entry point, create one
        var primaryMain = primary.EntryPoint ?? CreateMain(primary);

        // Clear IL instructions for the new unified Main
        primaryMain.Body.Instructions.Clear();
        var il = primaryMain.Body.GetILProcessor();

        // Append calls to all entry points
        foreach (var ep in entryPoints)
        {
            if (ep == null) continue;

            // Import the method into primary module
            var imported = primary.MainModule.ImportReference(ep);

            // If the method is not static, instantiate its declaring type
            if (!ep.IsStatic)
            {
                var ctor = primary.MainModule.ImportReference(
                    ep.DeclaringType.Methods.First(m => m.IsConstructor && !m.IsStatic)
                );
                var tempVar = new VariableDefinition(primary.MainModule.ImportReference(ep.DeclaringType));
                primaryMain.Body.Variables.Add(tempVar);

                il.Append(il.Create(OpCodes.Newobj, ctor));
                il.Append(il.Create(OpCodes.Stloc, tempVar));
                il.Append(il.Create(OpCodes.Ldloc, tempVar));
            }

            il.Append(il.Create(OpCodes.Call, imported));
        }

        // Return from Main
        if (primaryMain.ReturnType.FullName == "System.Void")
            il.Append(il.Create(OpCodes.Ret));
        else
        {
            il.Append(il.Create(OpCodes.Ldc_I4_0)); // default int return
            il.Append(il.Create(OpCodes.Ret));
        }

        primary.EntryPoint = primaryMain;

        // Merge types and resources from other assemblies
        foreach (var asm in assemblies.Skip(1))
        {
            foreach (var module in asm.Modules)
            {
                foreach (var type in module.Types)
                {
                    if (type.Name == "<Module>") continue;
                    primary.MainModule.Types.Add(type);
                }

                foreach (var res in module.Resources)
                {
                    if (!primary.MainModule.Resources.Any(r => r.Name == res.Name))
                        primary.MainModule.Resources.Add(res);
                    else
                        Console.WriteLine($"[!] Resource conflict skipped: {res.Name}");
                }
            }
        }

        return primary;
    }

    private static MethodDefinition CreateMain(AssemblyDefinition asm)
    {
        var mainMethod = new MethodDefinition(
            "Main",
            MethodAttributes.Public | MethodAttributes.Static,
            asm.MainModule.TypeSystem.Void
        );

        var programType = asm.MainModule.Types.FirstOrDefault(t => t.Name != "<Module>")
                          ?? asm.MainModule.Types[0];

        programType.Methods.Add(mainMethod);
        asm.EntryPoint = mainMethod;
        return mainMethod;
    }

}
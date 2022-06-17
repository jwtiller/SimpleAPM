using Injectors.Loggers;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Injectors
{
    public class ConsoleStopWatchInjector
    {
        public void Run(string fileName)
        {
            var module = ModuleDefinition.ReadModule(fileName,
                    new ReaderParameters { ReadWrite = true, ReadingMode = ReadingMode.Immediate, InMemory = true });

            foreach (var type in module.Types)
            {
                var stopWatchCtor =
                    module.ImportReference(typeof(Stopwatch).GetConstructor(BindingFlags.Instance | BindingFlags.Public,
                        null, new Type[0], null));

                var stopWatchRef = module.ImportReference(typeof(Stopwatch));
                var stopWatchStartRef = module.ImportReference(typeof(Stopwatch).GetMethod("Start", Type.EmptyTypes));
                var stopWatchStopRef = module.ImportReference(typeof(Stopwatch).GetMethod("Stop", Type.EmptyTypes));
                var stopWatchElapsedRef = module.ImportReference(typeof(Stopwatch).GetProperty("Elapsed")?.GetMethod);
                var consoleWriteLineRef = new ConsoleLogger(module).Reference;
                var timeSpanRef = module.ImportReference(typeof(TimeSpan));

                foreach (var method in type.Methods)
                {
                    if (method.HasBody)
                    {
                        var processor = method.Body.GetILProcessor();

                        var variableDeclaration = new VariableDefinition(stopWatchRef);
                        method.Body.Variables.Add(variableDeclaration);

                        var firstInstruction = method.Body.Instructions.First();

                        var newObjOpCode = processor.Create(OpCodes.Newobj, stopWatchCtor);
                        var stoLocOpCode = processor.Create(OpCodes.Stloc, variableDeclaration);
                        var ldLocOpCode = processor.Create(OpCodes.Ldloc, variableDeclaration);
                        var callStartOpCode = processor.Create(OpCodes.Call, stopWatchStartRef);

                        processor.InsertBefore(firstInstruction, newObjOpCode);
                        processor.InsertBefore(firstInstruction, stoLocOpCode);
                        processor.InsertBefore(firstInstruction, ldLocOpCode);
                        processor.InsertBefore(firstInstruction, callStartOpCode);

                        var ldLocOpcode2 = processor.Create(OpCodes.Ldloc, variableDeclaration);
                        var callStopOpCode = processor.Create(OpCodes.Call, stopWatchStopRef);
                        var ldStrOpCode = processor.Create(OpCodes.Ldstr, $"---Method {method.FullName} took {{0}}");
                        var callElapsedOpcode = processor.Create(OpCodes.Call, stopWatchElapsedRef);
                        var callWriteLineOpCode = processor.Create(OpCodes.Call, consoleWriteLineRef);
                        var boxOpcode = processor.Create(OpCodes.Box, timeSpanRef);

                        var lastInstruction = processor.Body.Instructions.Last();
                        processor.Replace(lastInstruction, ldLocOpcode2);
                        processor.Body.Instructions.Add(callStopOpCode);
                        processor.Body.Instructions.Add(ldStrOpCode);
                        processor.Body.Instructions.Add(ldLocOpCode);
                        processor.Body.Instructions.Add(callElapsedOpcode);
                        processor.Body.Instructions.Add(boxOpcode);
                        processor.Body.Instructions.Add(callWriteLineOpCode);
                        processor.Body.Instructions.Add(lastInstruction);

                        foreach (var bodyInstruction in processor.Body.Instructions)
                        {
                            if (bodyInstruction.OpCode != OpCodes.Br && bodyInstruction.OpCode != OpCodes.Br_S) continue;
                            if (((Instruction)bodyInstruction.Operand).OpCode != OpCodes.Ret) continue;
                            bodyInstruction.Operand = ldLocOpcode2;
                        }

                    }
                }
            }
            module.Write(fileName);
        }

    }
}

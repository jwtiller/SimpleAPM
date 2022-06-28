using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectors.Loggers
{
    public class ConsoleLogger : ILogger
    {
        public MethodReference Reference(ModuleDefinition module) => module.ImportReference(typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string), typeof(object) }));
    }
}

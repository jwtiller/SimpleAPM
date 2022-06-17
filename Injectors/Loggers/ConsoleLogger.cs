using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectors.Loggers
{
    public class ConsoleLogger
    {
        private readonly ModuleDefinition _module;
        public ConsoleLogger(ModuleDefinition module)
        {
            _module = module;
        }
        public MethodReference Reference => _module.ImportReference(typeof(Console).GetMethod(nameof(Console.WriteLine), new[] { typeof(string), typeof(object) }));
    }
}

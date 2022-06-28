using Injectors.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Injectors
{
    public class Injector<TInjector,TLogger> 
        where TInjector : IInjector, new()
        where TLogger : ILogger, new()
    {
        private readonly TInjector _injector;
        private readonly ILogger _logger;

        public Injector()
        {
            _injector = new TInjector();
            _logger = new TLogger();
        }

        public void Build(string fileName) => _injector.Build(fileName, _logger);
    }
}

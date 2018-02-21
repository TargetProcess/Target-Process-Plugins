using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NBehave.Narrator.Framework;

namespace Tp.Testing.Common.NBehave
{
    public class TextScenarioRunner
    {
        private readonly IEventListener _eventListener;
        private readonly IEnumerable<Assembly> _actionStepAssemblies;
        private readonly StoryRunnerFilter _storyRunnerFilter;

        public TextScenarioRunner(IEventListener eventListener, IEnumerable<Assembly> actionStepAssemblies,
            StoryRunnerFilter storyRunnerFilter)
        {
            _eventListener = eventListener;
            _actionStepAssemblies = actionStepAssemblies;
            _storyRunnerFilter = storyRunnerFilter;
        }

        public void Run(string scenario)
        {
            Run(new MemoryStream(Encoding.ASCII.GetBytes(scenario)));
        }

        protected void Run(Stream scenario)
        {
            var runner = new TextRunner(_eventListener) { StoryRunnerFilter = _storyRunnerFilter };

            _actionStepAssemblies.ToList().ForEach(runner.LoadAssembly);
            runner.Load(scenario);
            runner.Run();
        }
    }
}

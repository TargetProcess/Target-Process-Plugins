using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using NBehave.Narrator.Framework;
using NBehave.Narrator.Framework.EventListeners;

namespace Tp.Testing.Common.NBehave
{
    public static class BddScenarioExtensions
    {
        private static readonly Assembly[] ActionStepAssemblies;

        static BddScenarioExtensions()
        {
            ActionStepAssemblies = GetActionStepAssemblies().ToArray();
        }

        private static IEnumerable<Assembly> GetActionStepAssemblies()
        {
            var actionStepAssemblies = new List<Assembly>();
            actionStepAssemblies.AddRange(GetAssemblyBy(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")));
            actionStepAssemblies.AddRange(GetAssemblyBy(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.exe")));
            return actionStepAssemblies;
        }

        public static void Execute(this string scenario, params IEventListener[] eventListeners)
        {
            var stackTrace = new StackTrace();
            var type = stackTrace.GetFrame(1).GetMethod().DeclaringType;
            scenario.Execute(new StoryRunnerFilter(type.Namespace, type.Name, "."), new[] { type.Assembly }, eventListeners);
        }

        private static void Execute(this string scenario, StoryRunnerFilter filter, params Assembly[] assemblies)
        {
            Execute(scenario, filter, assemblies, new IEventListener[] { });
        }

        private static void Execute(this string scenario, StoryRunnerFilter filter, IEnumerable<Assembly> assemblies,
            params IEventListener[] eventListenersArg)
        {
            var eventListeners = new List<IEventListener>();
            eventListeners.AddRange(eventListenersArg);

            eventListeners.Add(new ColorfulConsoleOutputEventListener());
            eventListeners.Add(new FailSpecResultEventListener());

            var multiEventListener = new MultiOutputEventListener(eventListeners.ToArray());

            var builder = new TextScenarioRunner(multiEventListener, assemblies, filter);
            builder.Run(scenario);
        }

        public static void Execute(this string scenario, StoryRunnerFilter filter, params IEventListener[] eventListeners)
        {
            scenario.Execute(filter, ActionStepAssemblies);
        }

        private static IEnumerable<Assembly> GetAssemblyBy(IEnumerable<string> assemblyNames)
        {
            var assemblies = new List<Assembly>();
            foreach (var assemblyName in assemblyNames)
            {
                try
                {
                    var assembly = Assembly.LoadFrom(assemblyName);
                    if (assembly.IsDefined(typeof(ActionStepsAssemblyAttribute), false))
                    {
                        assemblies.Add(assembly);
                    }
                }
                catch //just ignore any kind of load exceptions
                {
                }
            }
            return assemblies;
        }
    }
}

using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NBehave.Narrator.Framework;

namespace Tp.Testing.Common.NBehave
{
    public static class In
    {
        public static StoryRunnerFilter Context<TActionStepsClass>()
        {
            return new StoryRunnerFilter().And<TActionStepsClass>();
        }

        public static StoryRunnerFilter GlobalContext()
        {
            return new StoryRunnerFilter();
        }

        public static StoryRunnerFilter DSL()
        {
            const string dslNamespace = "Tp.Testing.Common.ActionSteps.DSL";
            var dslClasses =
                Assembly.Load("Tp.Testing.Common.ActionSteps").GetTypes().Where(x => x.Namespace == dslNamespace).Select(x => x.Name);
            var dslClassesAsString = string.Join("|", dslClasses.ToArray());

            return new StoryRunnerFilter(dslNamespace, dslClassesAsString, ".*");
        }
    }

    public static class StoryRunnerFilterExtensions
    {
        public static StoryRunnerFilter Context<TActionStepsClass>(this StoryRunnerFilter filter)
        {
            var type = typeof(TActionStepsClass);
            var classFilterValue = filter.ClassNameFilter.UpdateWith(type.Name);
            var namespaceFilter = filter.NamespaceFilter.UpdateWith(type.Namespace);
            return new StoryRunnerFilter(namespaceFilter, classFilterValue, filter.MethodNameFiler.ToString());
        }

        public static string UpdateWith(this Regex initialFilter, string valueToApply)
        {
            var filterValue = initialFilter.ToString().TrimStart('^').TrimEnd('$');
            if (filterValue == ".")
            {
                return valueToApply;
            }
            return filterValue + string.Format("|{0}", valueToApply);
        }

        public static StoryRunnerFilter And<TActionStepsClass>(this StoryRunnerFilter filter)
        {
            return filter.Context<TActionStepsClass>();
        }
    }
}

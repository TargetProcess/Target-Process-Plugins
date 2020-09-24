using StructureMap;

namespace Tp.Core.Features
{
    public interface ITpFeatureListFactory
    {
        ITpFeatureList GetInstance();
    }
    
    public class CachingTpFeatureListFactory : ITpFeatureListFactory
    {
        private readonly ITpFeatureList _tpFeatureList;

        public CachingTpFeatureListFactory(IContainer container)
        {
            _tpFeatureList = container.GetInstance<ITpFeatureList>();
        }
        
        public ITpFeatureList GetInstance() => _tpFeatureList;
    }
    
    public class NonCachingTpFeatureListFactory : ITpFeatureListFactory
    {
        public ITpFeatureList GetInstance() => ObjectFactory.GetInstance<ITpFeatureList>();
    }
}

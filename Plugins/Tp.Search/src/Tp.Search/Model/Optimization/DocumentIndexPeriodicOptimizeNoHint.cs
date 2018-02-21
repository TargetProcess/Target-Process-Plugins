namespace Tp.Search.Model.Optimization
{
    class DocumentIndexPeriodicOptimizeNoHint : IDocumentIndexPeriodicOptimizeHint
    {
        public bool Advice()
        {
            return false;
        }
    }
}

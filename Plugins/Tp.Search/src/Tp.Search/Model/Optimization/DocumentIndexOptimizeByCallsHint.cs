namespace Tp.Search.Model.Optimization
{
    class DocumentIndexOptimizeByCallsHint : IDocumentIndexOptimizeHint
    {
        private readonly int _deferredOptimizeThreshold;
        private int _counter;

        public DocumentIndexOptimizeByCallsHint(int deferredOptimizeThreshold)
        {
            _deferredOptimizeThreshold = deferredOptimizeThreshold;
            _counter = 0;
        }

        public bool Advice()
        {
            if (++_counter == _deferredOptimizeThreshold)
            {
                _counter = 0;
                return true;
            }
            return false;
        }
    }
}

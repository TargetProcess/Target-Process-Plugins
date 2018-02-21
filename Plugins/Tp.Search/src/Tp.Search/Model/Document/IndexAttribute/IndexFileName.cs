using System;

namespace Tp.Search.Model.Document.IndexAttribute
{
    [AttributeUsage(AttributeTargets.Field)]
    class IndexFileName : Attribute
    {
        private readonly string _fileName;

        public IndexFileName(string fileName)
        {
            _fileName = fileName;
        }

        public string FileName
        {
            get { return _fileName; }
        }
    }
}

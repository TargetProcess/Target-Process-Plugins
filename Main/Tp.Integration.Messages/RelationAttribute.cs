#region

using System;

#endregion

namespace Tp.Integration.Common
{
    /// <summary>
    /// It is a marker for relation properties. Only for system usage.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    [Serializable]
    public class RelationAttribute : Attribute
    {
        private readonly string _entity;
        private readonly string _field;

        ///<summary>
        /// Ctor
        ///</summary>
        ///<param name="entity">Entity Name</param>
        ///<param name="field">Field</param>
        public RelationAttribute(string entity, string field)
        {
            _entity = entity;
            _field = field;
        }

        public string Field
        {
            get { return _field; }
        }

        public string Entity
        {
            get { return _entity; }
        }
    }
}
using System.Data;

// ReSharper disable once CheckNamespace

namespace System
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SqlFunctionAttribute : Attribute
    {
        public string Name { get; private set; }

        public DbType DbType { get; private set; }

        public SqlFunctionAttribute(string name, DbType dbType)
        {
            Name = name;
            DbType = dbType;
        }
    }

    public class SqlDateFunctionAttribute : SqlFunctionAttribute
    {
        private readonly DatePart _datePartSpecifier;

        public SqlDateFunctionAttribute(string name, DbType dbType, DatePart datePartSpecifier = DatePart.None) : base(name, dbType)
        {
            _datePartSpecifier = datePartSpecifier;
        }

        public DatePart DatePartSpecifier
        {
            get { return _datePartSpecifier; }
        }
    }

    public enum DatePart
    {
        None,
        Year,
        Quarter,
        Month,
        DayOfYear,
        Day,
        Week,
        Hour,
        Minute,
        Second,
        Millisecond,
        Microsecond,
        Nanosecond
    }

    public class TableValuedSqlFunctionAttribute : SqlFunctionAttribute
    {
        public string ResultColumnName { get; set; }

        public TableValuedSqlFunctionAttribute(string name, string resultColumnName, DbType dbType) : base(name, dbType)
        {
            ResultColumnName = resultColumnName;
        }
    }
}

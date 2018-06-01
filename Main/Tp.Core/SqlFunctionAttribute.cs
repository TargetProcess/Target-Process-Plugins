using System.Data;

// ReSharper disable once CheckNamespace

namespace System
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SqlFunctionAttribute : Attribute
    {
        public string Name { get; }

        public DbType DbType { get; }

        public SqlFunctionAttribute(string name, DbType dbType)
        {
            Name = name;
            DbType = dbType;
        }
    }

    public class SqlDateFunctionAttribute : SqlFunctionAttribute
    {
        public SqlDateFunctionAttribute(string name, DbType dbType, DatePart datePartSpecifier = DatePart.None) : base(name, dbType)
        {
            DatePartSpecifier = datePartSpecifier;
        }

        public DatePart DatePartSpecifier { get; }
    }

    // NOTE: Names should be the same as DATEDIFF scalar T-SQL function datepart argument.
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

namespace Tp.Integration.Common
{
    /// <summary>
    /// 	Describes the type of the Custom Field
    /// </summary>
    public enum FieldTypeEnum
    {
        /// <summary>
        /// 	Undefined
        /// </summary>
        None = -1,

        /// <summary>
        /// 	The text
        /// </summary>
        Text = 0,

        /// <summary>
        /// 	The drop down
        /// </summary>
        DropDown = 1,

        /// <summary>
        /// 	The check box
        /// </summary>
        CheckBox = 2,

        /// <summary>
        /// 	URL
        /// </summary>
        URL = 3,

        /// <summary>
        /// 	The date
        /// </summary>
        Date = 4,

        /// <summary>
        /// 	The rich text
        /// </summary>
        RichText = 5,

        ///<summary>
        ///	The number
        ///</summary>
        Number = 6,

        /// <summary>
        /// 	Target Process Entity
        /// </summary>
        Entity = 7,

        /// <summary>
        /// 	The multiple selection list
        /// </summary>
        MultipleSelectionList = 8,

        /// <summary>
        /// 	URL with suffix and postfix parts
        /// </summary>
        TemplatedURL = 9,

        /// <summary>
        /// 	Multiple Target Process Entities
        /// </summary>
        MultipleEntities = 10,

        /// <summary>
        /// 	Money
        /// </summary>
        Money = 11
    }
}

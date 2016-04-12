Ext.ns('Tp.controls.menu');

Tp.controls.menu.AssignmentCollectorStorage = Ext.extend(Object, {
    data: null,
    showWithoutFiltering: false,

    constructor: function(config) {
        Ext.apply(this, config);
    },

    createMenuItem: function(dataItem) {
        var menuItem =
        {
            clickHideDelay: 500,
            text: dataItem.Text,
            htmlToRender: dataItem.Html,
            isGroup: dataItem.IsGroup,
            isShowMore: dataItem.IsShowMore,
            clickable: true,
            dataItem: dataItem,
            cls: dataItem.Cls,
            activeClass: ''
        };

        if (dataItem.ShowIcon) {
            menuItem.iconCls = 'menu-user-icon';
        }

        if (dataItem.IsGroup) {
            menuItem.hideOnClick = false;
        }
        return menuItem;
    },

    findKeys: function(contextFilterKeys, item) {
        var foundKeys = [];
        if (contextFilterKeys.length == 0)
            return foundKeys;

        for (var j = 0; j < contextFilterKeys.length; j++) {
            var resultKey;
            resultKey = Array.findOne(item.FilterKeys, function(key) {
                if (key == contextFilterKeys[j]) return true;
            });
            if (resultKey) {
                foundKeys.push(resultKey);
            }
        }
        return foundKeys;
    },

    IsAssignableAlreadyAssignedToUserFromContext: function(item, context) {
        return item.Id == context.UserId;
    },

    IsSameProject: function(item, context) {
        return item.Projects.indexOf(context.ProjectId) != -1;
    },

    IsProjectIdDefined: function(item) {
        return item.Projects && item.Projects.length > 0;
    },

    filterDataForRole: function(roleId, projectId, showAll) {
        var filteredData = [];
        var allDataWithoutShowMore = [];

        var countOfItems = 0;
        var showMore = null;
        var arrayLenght = this.data.length;

        for (var i = 0; i < arrayLenght; i++) {
            var dataItem = this.data[i];

            if (!dataItem.IsShowMore)
                allDataWithoutShowMore.push(dataItem);
            else
                showMore = dataItem;

            if (showAll)
                continue;

            if (roleId == dataItem.RoleId || dataItem.IsExtra) {

                //Check that data item is not a group or not empty group
                if (this.checkIsValidGroup(dataItem, i, arrayLenght, projectId) || !dataItem.IsGroup) {
                    if (!dataItem.IsExtra)
                        countOfItems++;

                    if (!dataItem.IsShowMore)
                        filteredData.push(dataItem);
                }
            }
        }

        if (countOfItems == 0)
            return allDataWithoutShowMore;

        if (showMore != null)
            filteredData.push(showMore);

        return filteredData;
    },

    checkIsValidGroup: function(dataItem, index, arrayLenght, projectId) {
        return (dataItem.IsGroup && index < arrayLenght - 1 && this.data[index + 1].RoleId == dataItem.RoleId && this.data[index + 1].Projects.contains(projectId));
    },

    appendCriteriaKeys: function(context, showAll) {
        var result = [];
        var contextFilterKeys = context.FilterKeys;
        var items = this.filterDataForRole(context.ActorId, context.ProjectId, showAll);

        for (var i = 0; i < items.length; i++) {
            var item = items[i];

            if (item.Id == context.UserId && !item.IsShowMore) {
                continue;
            }

            if (this.IsProjectIdDefined(item) && !this.IsSameProject(item, context)) {
                continue;
            }

            if (this.IsAssignableAlreadyAssignedToUserFromContext(item, context)) {
                continue;
            }

            var foundKeys = this.findKeys(contextFilterKeys, item);

            if (foundKeys.length == 0 && contextFilterKeys.length > 0 && item.FilterKeys.length > 0) {
                continue;
            }

            if (foundKeys.length > 0) {
                var clonedItem = Tp.util.clone(item);
                clonedItem.Text = String.format("{0}({1})", clonedItem.Text, foundKeys.join(", "));
                result.push(clonedItem);
                continue;
            }

            if (item.IsGroup && result.length > 0)
                if (result[result.length - 1].IsGroup)
                    result.pop();
            result.push(item);
        }

        if (result.length > 0)
            if (result[result.length - 1].IsGroup)
                result.pop();
        return result;
    },

    getMenuItems: function(context, showAll, callbackFunction, callbackContext) {
        if (!this.data) {

            Ext.Ajax.request({
                url: new Tp.WebServiceURL('/PageServices/AssignmentMenuService.asmx/GetMenuItems').toString(),
                headers: { 'Content-Type': 'application/json' },
                method: 'POST',
                success: function (res) {
                    this.data = jsonParse(res.responseText).d;
                    var items = this.afterLoadMenuItems(context, showAll);
                    callbackFunction.call(callbackContext, items);
                },
                failure: function () {
                    SetLastWarning("Could not load assignments!");
                },
                scope: this
            })
        }
        else {
            var items = this.afterLoadMenuItems(context, showAll);
            callbackFunction.call(callbackContext, items);
        }
    },

    afterLoadMenuItems: function(context, showAll) {
        var arrayWithCriteriaKeys = this.appendCriteriaKeys(context, showAll || this.showWithoutFiltering);
        var result = [];
        for (var i = 0; i < arrayWithCriteriaKeys.length; i++) {
            var menuItem = arrayWithCriteriaKeys[i];
            result.push(this.createMenuItem(menuItem));
        }
        return result;
    }
});

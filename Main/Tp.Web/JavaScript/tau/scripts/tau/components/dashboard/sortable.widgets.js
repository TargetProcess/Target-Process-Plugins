define(["require","jQuery","tau/core/class","tau/components/react/mixins/sortable.axis.y"],function(t){var e=t("jQuery"),r=t("tau/core/class"),a=t("tau/components/react/mixins/sortable.axis.y");return r.mixin(a,{_findLastSortable:function(t){return e(t).find(this.sortableOptions.sortableItemSelector+":last")},_findSortableTargetFromEvent:function(t){var r=this._super(t);return r.length?r:e(t.target).closest(".i-role-dashboard-column")}})});
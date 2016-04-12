Ext.ns('Tp.custom.plugins');

Tp.controls.Prioritization.ChangeStateColumnPlugin = Ext.extend(Object, {
	header: "State",
	width: 100,
	menuDisabled: true,
	dataIndex: 'entity_state',
	rowIndex: -1,
	id: 'stateColumnPlugin',
	moveable: false,
	grid: null,
	mouseEntered: false,
	store: null,
	editor: null,

	constructor: function (config) {
		this.editor = new Ext.form.ComboBox({
			typeAhead: true,
			triggerAction: 'all',
			lazyRender: true,
			mode: 'local',
			loadingText: 'Loading...',
			editable: false,
			store: new Ext.data.SimpleStore({
				fields: ['id', 'name'],
				proxy: new Ext.data.DataProxy({})
			}),
			valueField: 'id',
			displayField: 'name'
		});
		Tp.controls.Prioritization.ChangeStateColumnPlugin.superclass.constructor.call(this, config);
	},

	init: function (grid) {
		this.grid = grid;
		this.grid.on('beforeedit', this.onBeforeEdit, this);
		this.grid.on('validateedit', this.onValidateEdit, this);
		if (this.editor) {
			this.editor.on('beforequery', this.onBeforeQuery, this);
			this.editor.on('select', this.onSelect, this);
			this.editor.store.proxy.load = this.onLoad;
			this.editor.store.proxy.onSuccessLoadResponse = this.onSuccessLoadResponse;
			this.editor.store.proxy.onErrorLoadRequest = this.onErrorLoadRequest;
			this.editor.store.proxy.createRecords = function (state, id, text, recordType) {
				Tp.util.validateForNulls([this.data]);
				var jsonData = new Object();
				jsonData.data = [];
				var nextData = Array.findOne(
                        this.data,
                        function (item) {
                        	return item[id] == state;
                        }
                        );
				if (nextData) {
					Array.forEach(
                            this.data,
                            function (item) {
                            	if (Array.contains(nextData.nextStates, item[id])) {
                            		var i = new Object();
                            		i[id] = item[id];
                            		i[text] = item[text];
                            		jsonData.data.push(i);
                            	}
                            }
                            );
				}
				jsonData.results = jsonData.data.length;
				var reader = new Ext.data.JsonReader({ totalProperty: "results", root: "data" }, recordType);
				return reader.readRecords(jsonData);
			};
		}
	},

	onLoad: function (params, reader, callback, scope, arg) {
		var callbackDelegate = callback.createDelegate(scope, [arg], true);
		if (this.data) {
			for (var n = 0; n < this.data.length; n++) {
				if (this.data[n][arg.combo.valueField] == arg.state) {
					var records = this.createRecords(arg.state, arg.combo.valueField, arg.combo.displayField, arg.combo.store.recordType);
					callbackDelegate(records);
					return;
				}
			}
		}
		Tp.Web.PageServices.KanbanBoard.KanbanBoardService.loadEntityStatesByEntityState(
                arg.state,
                this.onSuccessLoadResponse.createDelegate(this),
                this.onErrorLoadRequest.createDelegate(this),
        { callback: callbackDelegate, arg: arg });
	},

	onSuccessLoadResponse: function (recordSet, e) {
		Tp.util.validateForNulls([recordSet]);

		if (!this.data) {
			this.data = [recordSet];
		}
		else {
			this.data.push(recordSet);
		} //  : Array.add(this.data, recordSet);
		this.data = recordSet;
		var records = this.createRecords(e.arg.state, e.arg.combo.valueField, e.arg.combo.displayField, e.arg.combo.store.recordType);
		e.callback(records);
	},

	onErrorLoadRequest: function () {
		throw "onErrorLoadRequest";
	},

	onBeforeQuery: function (e) {
		var r = this.grid.store.getAt(this.rowIndex);
		var state = r.data['entityStateId'];
		var context = { combo: e.combo, state: state };
		Tp.util.validateForNulls([context]);
		e.combo.store.load(context);
	},

	onBeforeEdit: function (e) {
		if (e.field == this.dataIndex)
			this.rowIndex = e.row;
	},

	onValidateEdit: function (e) {
		if (this.context != null) {
			e.value = this.context.data.name;
		}
	},

	onSelect: function (combo, record) {
		var r = this.grid.store.getAt(this.rowIndex);
		var entityId = r.data['id'];
		var context = { record: r, data: record.data };
		Tp.util.validateForNulls([context]);
		Tp.Web.PageServices.KanbanBoard.KanbanBoardService.changeEntityState('', entityId, record.data.id, 'State was changed from {0} to {1} using Kanban Board.', -1, -1,
                this.changeEntityState_onSuccess.createDelegate(this),
                this.changeEntityState_onFailure.createDelegate(this),
                context);
	},

	// contains selected data from combobox
	context: null,

	changeEntityState_onSuccess: function (x, context) {
		this.context = context;
		context.record.set('entityStateId', context.data.id);
		context.record.set('entity_state', context.data.name);

		this.editor.triggerBlur();
		context.record.commit();

		//this.fireEvent("statechanged", this, context.entity);
		Tp.util.Notifier.notify('OK', 'State changed to "' + context.data.name + '"');
	},

	changeEntityState_onFailure: function (x, context) {
		this.context = null;
		this.editor.triggerBlur();
		context.record.reject();
		Tp.util.Notifier.error('Error', 'Changing state failed, please reload this page.\n<hr/>\n' + x.get_message());
	}
});

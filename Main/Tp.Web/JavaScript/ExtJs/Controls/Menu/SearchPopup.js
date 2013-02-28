Ext.ns('Tp.controls.menu');
Tp.controls.menu.SearchPopup = Ext.extend(Tp.controls.menu.BasePopup, {

	constructor: function (config) {
		Tp.controls.menu.SearchPopup.superclass.constructor.call(this, config);
		this.alignToElement = Ext.get(this.alignTo);
		Ext.EventManager.onWindowResize(this.onResize, this);

		this.on('aftershow', this.onAfterShow, this);
		this.on('beforehide', this.onBeforeHide, this);

		var addvancedSearchButton = Ext.get('popupSearchAdvanced');
		addvancedSearchButton.on('click', this.onSearchClick, this);
		this.searchDiv = new Ext.get('topSearch');

		Ext.get(this.generalSearchTextBox).on('focus', this._generalSearchOnFocus, this); // Ext.get(this.searchTextBox)
		Ext.get(this.generalSearchTextBox).on('blur', this._generalSearchOnBlur, this); // Ext.get(this.searchTextBox)
	},

	_generalSearchOnFocus: function () {
		if (this.isVisible()) return;
		Ext.fly(this.generalSearchTextBox).addClass('search-on');
	},

	_generalSearchOnBlur: function () {
		if (this.isVisible()) return;
		if (Ext.fly(this.generalSearchTextBox).getValue()) return;
		Ext.fly(this.generalSearchTextBox).removeClass('search-on');
	},
	_createContainer: function () {
		this.container = Ext.get(this.containerEl);
		this.container.setVisibilityMode(Ext.Element.DISPLAY);
	},

	alignToTrigerElement: function () {
		this.container.setTop(this.alignToElement.getTop() - 8); // + this.container.getHeight() + this.alignToElement.getHeight());
		this.container.setLeft(this.alignToElement.getRight() - this.container.getWidth() + 22);
	},

	synchronizeElements: function (source, dest) {
		var object = {
			value: source.getValue(),
			selectionStart: source.dom.selectionStart,
			selectionEnd: source.dom.selectionEnd
		};

		source.addClass('search-on');

		dest.dom.value = source.dom.value; //(object);

		dest.dom.selectionStart = object.selectionStart;
		dest.dom.selectionEnd = object.selectionEnd;

		dest.focus(1);
	},

	onAfterShow: function () {
		this.container.removeClass('dropDownPanel');
		this.alignToTrigerElement();

		this.synchronizeElements(Ext.get(this.generalSearchTextBox), Ext.get(this.searchTextBox));

		this.searchDiv.hide();

		var observer = searchObserverFactory.getObserverByElement(this.searchTextBox);
		observer.setSearchButtonVisibility();
	},

	onSearchClick: function () {
		this.hide();
	},

	onBeforeHide: function () {
		this.searchDiv.show();
		this.synchronizeElements(Ext.get(this.searchTextBox), Ext.get(this.generalSearchTextBox));
	},

	onResize: function () {
		this.alignToTrigerElement();
	}
});

Tp.controls.menu.SearchObserver = Ext.extend(Object, {
	_extElements: null,
	_searchButtonElement: null,
	_entityTypesListElement: null,
	_entityStatesListElement: null,

	constructor: function (config) {
		Ext.apply(this, config);
		this._searchButtonElement = Ext.get(this.searchButton);
		this._attachToControls();
		this.setSearchButtonVisibility();
		this._setupEntityStatesEnabling();
	},

	_attachToControls: function () {
		this._extElements = [];
		for (var i = 0; i < this.elements.length; i++) {
			var element = Ext.get(this.elements[i]);
			this._extElements.push(element);
			element.on('change', this._onValueChanged, this);
			element.on('click', this._onValueChanged, this);
			var keyHandler = function () {
				window.setTimeout(Function.createDelegate(this, this._onValueChanged), 0);
			};

			element.on('keydown', keyHandler, this);
		}

		this._entityTypesListElement = Ext.get(this.entityTypesList);
		this._entityTypesListElement.on('change', this._onEntityTypeChange, this);

		this._entityStatesListElement = Ext.get(this.entityStateList);
	},

	_setupEntityStatesEnabling: function () {
		var options = this._entityStatesListElement.getAttribute('options');
		var entityKindId = this._entityTypesListElement.getValue(true);

		if (isNaN(entityKindId)) {
			for (var i = 0; i < options.length; i++) {
				Ext.fly(options[i]).set({ disabled: false }, false);
			}
		}
		else {
			var disabledCount = 0;
			for (var i = 1; i < options.length; i++) {
				var itemElement = Ext.fly(options[i]);
				var disabled = !_entityStateCollection.isStateEvailableForEntity(itemElement.getValue(), entityKindId);

				itemElement.set({ disabled: disabled }, false);
				if (disabled) {
					disabledCount++;
				}

				if (disabled && this._entityStatesListElement.getValue() == itemElement.getValue()) {
					this._entityStatesListElement.set({ value: '' }, false);
				}
			}

			Ext.fly(options[0]).set({ disabled: disabledCount == options.length - 1 }, false);
		}
	},

	_onEntityTypeChange: function () {
		return this._setupEntityStatesEnabling();
	},

	_onValueChanged: function () {
		this.setSearchButtonVisibility();
	},

	setSearchButtonVisibility: function () {
		for (var i = 0; i < this._extElements.length; i++) {
			if (this._extElements[i].getValue() != null && this._extElements[i].getValue() != '') {
				this._searchButtonElement.dom.disabled = null;
				return;
			}
		}

		this._searchButtonElement.set({ disabled: true }, true);
	}
});

Tp.controls.menu.EntityStateContaier = Ext.extend(Object, {
	_states: null,

	constructor: function (config) {
		this._states = config;
	},

	isStateEvailableForEntity: function (stateName, enityKindId) {
		var stateNames = this._getEntityStateNamesForEntity(enityKindId);
		for (var i = 0; i < stateNames.length; i++) {
			if (stateNames[i].Key == stateName) {
				return true;
			}
		}
		return false;
	},

	getAllStateIdsForEntityState: function (stateName) {
		var result = [];
		for (var i = 0; i < this._states.length; i++) {
			for (var j = 0; j < this._states[i].EntityStateName.length; j++) {
				if (this._states[i].EntityStateName[j].Key == stateName) {
					result = result.concat(this._states[i].EntityStateName[j].Value);
					break;
				}
			}
		}
		return result;
	},

	getStateIdsForEntityType: function (stateName, enityKindId) {
		var stateNames = this._getEntityStateNamesForEntity(enityKindId);
		for (var i = 0; i < stateNames.length; i++) {
			if (stateNames[i].Key == stateName) {
				return stateNames[i].Value;
			}
		}
		return [];
	},

	_getEntityStateNamesForEntity: function (enityKindId) {
		for (var i = 0; i < this._states.length; i++) {
			if (this._states[i].EnityKind == enityKindId) {
				return this._states[i].EntityStateName;
			}
		}
		return [];
	}
});

Tp.controls.menu.SearchObserverFactory = Ext.extend(Object, {
	_cache: null,
	_nullObserver: null,
	constructor: function () {
		this._cache = [];
		this._nullObserver = Tp.NullObject(Tp.controls.menu.SearchObserver);
	},

	create: function (config) {
		var searchObserver = new Tp.controls.menu.SearchObserver(config);
		var copyOfConfig = {};
		Ext.apply(copyOfConfig, config);

		copyOfConfig.Contains = function (element) {
			if (this.elements.indexOf(element) != -1) {
				return true;
			}

			return this.searchButton == element;
		};

		this._cache.push({ config: copyOfConfig, observer: searchObserver });
	},

	getObserverByElement: function (element) {
		for (var i = 0; i < this._cache.length; i++) {
			if (this._cache[i].config.Contains(element)) {
				return this._cache[i].observer;
			}
		}
		return this._nullObserver;
	}
});
var searchObserverFactory = new Tp.controls.menu.SearchObserverFactory();
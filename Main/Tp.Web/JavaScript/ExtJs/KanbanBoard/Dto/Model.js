Ext.ns('Tp.data');

/**
 * Represents entity type, such as <em>User Story</em>, <em>Feature</em>, etc.
 *
 * @param {Number} id Entity type unique identifier.
 * @param {String} name Entity type system name.
 * @param {String} title Human readable entity type name.
 */
Tp.data.EntityType = function(id, name, title) {
    this.id = id;
    this.name = name;
    this.title = title;
};

/**
 * Represents state of an entity, such as <em>Open</em>, <em>Done</em>, etc.
 *
 * @param {Number} id Entity state unique identifier.
 * @param {String} name Entity state name.
 * @param {Array} nextStates Array of next states.
 */
Tp.data.EntityState = function(id, name, nextStates) {
    this.id = id;
    this.name = name;
    this.nextStates = nextStates || [];
};

/**
 * Represents a process entity.
 *
 * @param {Number} id Process unique identifier.
 * @param {String} name Process name.
 * @param {Array} entityTypes Array of entity types.
 * @param {Array} entityStates Array of entity states.
 */
Tp.data.Process = function(id, name, entityTypes, entityStates) {
    this.id = id;
    this.name = name;
    this.entityTypes = entityTypes;
    this.entityStates = entityStates;
};

/**
 * Represents an entity, such as <em>Bug</em>, <em>Task</em>, etc.
 *
 * @param {Number} id Entity unique identifier.
 * @param {Tp.data.EntityType} entityType Entity type descriptor.
 * @param {String} name Entity name.
 */
Tp.data.Entity = function(id, entityType, name) {
    this.id = id;
    this.entityType = entityType;
    this.name = name;
    this.priorityName = '';
    this.priorityImportance = 0;
};
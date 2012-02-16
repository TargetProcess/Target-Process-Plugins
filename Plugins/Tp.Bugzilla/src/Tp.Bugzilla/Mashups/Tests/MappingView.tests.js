require(["Bugzilla/MappingView"],
    function (mappingView) {
        module('test mapping view', {
            setup: function () {
                this.data = [
                    {
                        "Key": "NEW",
                        "Value": [
                            { "Id": 5, "Name": "Open", "Selected": true },
                            { "Id": 6, "Name": "Fixed", "Selected": false },
                            { "Id": 7, "Name": "Invalid", "Selected": false },
                            { "Id": 8, "Name": "Closed", "Selected": false }
                        ]
                    }, {
                        "Key": "RESOLVED",
                        "Value": [
                            { "Id": 5, "Name": "Open", "Selected": false },
                            { "Id": 6, "Name": "Fixed", "Selected": false },
                            { "Id": 7, "Name": "Invalid", "Selected": false },
                            { "Id": 8, "Name": "Closed", "Selected": false }
                        ]
                    }, {
                        "Key": "CLOSED",
                        "Value": [
                            { "Id": 5, "Name": "Open", "Selected": false },
                            { "Id": 6, "Name": "Fixed", "Selected": false },
                            { "Id": 7, "Name": "Invalid", "Selected": false },
                            { "Id": 8, "Name": "Closed", "Selected": true }
                        ]
                    }];
                this.placeholder = $('<div><div class="bugzilla-map-states" /></div>');
                this.view = new mappingView({
                    placeholder: this.placeholder,
                    mappingTemplate:'batched',
                    key: 'StatesMapping',
                    Caption: 'States Mapping',
                    Description: 'Specify Bugzilla-to-TargetProcess states mapping by name (Ex: Resolved -> Fixed)<br/> Required for correct bugs import.',
                    KeyName: 'Bugzilla State',
                    ValueName: 'TargetProcess State'
                });
            },

            teardown: function () {
            }
        });

        test('should display empty mapping', function () {
            this.view.render('.bugzilla-map-states')([]);
            ok(this.placeholder.find('.mapping-block').length === 1, "mapping is empty, only header is shown");
        });

        test('should show existing mapping from profile', function () {
            this.view.render('.bugzilla-map-states')(this.data);
            ok(this.placeholder.find('.mapping-block').length === 4, "all items from datasource are shown");

            var item = $(this.placeholder.find('.mapping-block')[1]);
            ok(item.find('.externalvalue').attr('value') == 'NEW', 'external label is valid');
            ok(item.find('.tpvalue').val() == 5, 'selected value is valid');
            ok(item.find('.tpvalue option:selected').text() == 'Open', 'selected item\'s text is valid');

            item = $(this.placeholder.find('.mapping-block')[2]);
            ok(item.find('.externalvalue').attr('value') == 'RESOLVED', 'external label is valid');
            ok(item.find('.tpvalue').val() == '0', 'selected value is valid');
            ok(item.find('.tpvalue option:selected').text() == '-Select-', 'selected item\'s text is valid');

            item = $(this.placeholder.find('.mapping-block')[3]);
            ok(item.find('.externalvalue').attr('value') == 'CLOSED', 'external label is valid');
            ok(item.find('.tpvalue').val() == 8, 'selected value is valid');
            ok(item.find('.tpvalue option:selected').text() == 'Closed', 'selected item\'s text is valid');
        });

        test('should get mapping values from view', function () {
            this.view.render('.bugzilla-map-states')(this.data);
            var mappings = this.view.getMappings();

            ok(mappings.length == 3, 'all mappings retrieved');

            var mapping = mappings[0];
            ok(mapping.Key == 'NEW', 'mapping key is valid');
            ok(mapping.Value.Id == 5, 'mapping value id is valid');
            ok(mapping.Value.Name == 'Open', 'mapping value name is valid');

            var mapping = mappings[1];
            ok(mapping.Key == 'RESOLVED', 'mapping key is valid');
            ok(typeof mapping.Value.Id == 'undefined', 'mapping value id is not specified');
            ok(typeof mapping.Value.Name == 'undefined', 'mapping value name is not specified');

            var mapping = mappings[2];
            ok(mapping.Key == 'CLOSED', 'mapping key is valid');
            ok(mapping.Value.Id == 8, 'mapping value id is valid');
            ok(mapping.Value.Name == 'Closed', 'mapping value name is valid');
        });
    });
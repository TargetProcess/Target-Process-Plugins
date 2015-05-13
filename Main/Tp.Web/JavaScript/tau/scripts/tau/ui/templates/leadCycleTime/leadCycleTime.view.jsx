define(function(require) {
    var React = require('libs/react/react-ex');
    var _ = require('Underscore');

    var VerticalLine = React.createClass({
        getDefaultProps: function() {
            return {
                y: 5,
                width: 1,
                height: 70,
                fill: 'rgb(226,226,227)'
            };
        },
        render: function() {
            return <rect x={this.props.x} y={this.props.y}
                width={this.props.width} height={this.props.height}
                fill={this.props.fill} />;
        }
    });

    var LifecycleBar = React.createClass({
        getDefaultProps: function() {
            return {
                x: 0,
                y: 5,
                height: 5,
                rx: 3,
                ry: 3
            };
        },
        render: function() {
            return <rect x={this.props.x} y={this.props.y}
                width={this.props.width} height={this.props.height}
                fill={this.props.fill}
                rx={this.props.rx} ry={this.props.ry}
                className={"ui-link " + this.props.className} />;
        }
    });

    return React.createClass({
        getDefaultProps: function() {
            return {
                leftDate: null,
                movingDate: null,
                rightDate: null,
                rightDateLabel: null,
                cycleTime: null,
                leadTime: null,
                cyclePercentage: null,
                leadPercentage: null
            };
        },

        _getMetrics: function(percentage) {
            var BAR_WIDTH = 80;

            return {
                isPresent: _.isNumber(percentage) && !_.isNaN(percentage),
                percent: percentage,
                left: BAR_WIDTH * (1 - percentage),
                width: BAR_WIDTH * percentage
            };
        },

        render: function() {
            var metrics = {
                lead: this._getMetrics(this.props.leadPercentage),
                cycle: this._getMetrics(this.props.cyclePercentage)
            };

            var textLeftFixStyle = {position: 'relative', left: '-15px'};
            var text = {
                fillColor: 'rgb(174,174,174)',
                x: 10,
                y: 55,
                transform: 'rotate(-90 10 55)',
                style: {fontSize: '10px', fontWeight: 'bold'}
            };

            var leftDateLabel = (<div className="t3-entity-lifecycle-label" style={{left: '6.5%'}}>
                <svg width="30" height="60" style={textLeftFixStyle}>
                    <text x={text.x} y={text.y} fill={text.fillColor} style={text.style} transform={text.transform}>
                        {this.props.leftDate}
                    </text>
                </svg>
            </div>);

            var startDateLabelStyle = {
                left: 3 + (metrics.cycle.percent >= 0.9 ? 3 : 0) + metrics.cycle.left + '%'
            };
            var startDateLabel = this.props.movingDate ?
                (<div className="t3-entity-lifecycle-label" style={startDateLabelStyle}>
                    <svg width="30" height="60">
                        <text x={text.x} y={text.y} fill={text.fillColor} style={text.style} transform={text.transform}>
                            {this.props.movingDate}
                        </text>
                    </svg>
                </div>) :
                null;

            var endDateLabelStyle = {
                top: '-5px',
                left: metrics.cycle.isPresent && metrics.cycle.percent < 0.2 ? '91%' : '87%'
            };
            var rightDateLabel = (<div className="t3-entity-lifecycle-label" style={endDateLabelStyle}>
                <svg width="30" height="60" style={textLeftFixStyle}>
                    <text x={text.x} y={text.y} fill={text.fillColor} style={text.style} transform={text.transform}>
                        <tspan>{this.props.rightDateLabel}</tspan>
                        <tspan x={text.x} dy="1.4em">{this.props.rightDate}</tspan>
                    </text>
                </svg>
            </div>);

            var leadCycleTimeLabels = (<div style={{position: 'absolute', left: '87.5%', top: '51px'}}>
                <svg width="60" height="30">
                    <text x="0" y="11" fill="rgb(70,155,219)" style={text.style}>{this.props.cycleTime}</text>
                    <text x="0" y="23" fill="rgb(95,95,95)" style={text.style}>{this.props.leadTime}</text>
                </svg>
            </div>);

            var leadTimeBar = metrics.lead.isPresent ?
                (<LifecycleBar
                    className="i-role-lead-time"
                    fill={'url(#leadTimeBar)'}
                    width={metrics.lead.width + '%'}
                    x={6.5 + metrics.lead.left + '%'}
                    y="67"
                />) :
                null;

            var cycleTimeBar = metrics.cycle.isPresent ?
                (<LifecycleBar
                    className="i-role-cycle-time"
                    fill={'url(#cycleTimeBar)'}
                    width={metrics.cycle.width + '%'}
                    x={6.5 + metrics.cycle.left + '%'}
                    y="58"
                />) :
                null;

            return (
                <div className="ui-lead-cycle-time">
                    {leftDateLabel}
                    {startDateLabel}
                    {rightDateLabel}
                    {leadCycleTimeLabels}
                    <svg width="100%" height="80" className="main-plot">
                        <defs>
                            <linearGradient id="cycleTimeBar" x1="0%" y1="0%" x2="0%" y2="100%">
                                <stop offset="0%" style={{stopColor: 'rgb(186,210,239)', stopOpacity: 1}} />
                                <stop offset="100%" style={{stopColor: 'rgb(98,143,198)', stopOpacity: 1}} />
                            </linearGradient>
                            <linearGradient id="leadTimeBar" x1="0%" y1="0%" x2="0%" y2="100%">
                                <stop offset="0%" style={{stopColor: 'rgb(178,184,194)', stopOpacity: 1}} />
                                <stop offset="100%" style={{stopColor: 'rgb(141,144,152)', stopOpacity: 1}} />
                            </linearGradient>
                        </defs>
                        {leadTimeBar}
                        <VerticalLine x="6%" />
                        <VerticalLine x="86.8%" />
                        {metrics.cycle.isPresent ? (<VerticalLine x={6 + metrics.cycle.left + '%'} height="60"/>) : null}
                        {cycleTimeBar}
                    </svg>
                </div>
            );
        }
    });
});

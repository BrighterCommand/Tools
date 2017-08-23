# Tools
This project contains tooling for Paramore.

## Message Viewer
Brighter, Brightside et al. store their messages in a backing store before enqueuing via message oriented middleware. This allows both replay and analysis of what has been raised from an app.

## CLI
A number of tools focused on providing CLI for Paramore

### Brighter Monitoring
When monitoring is enabled, Brighter will sends monitoring messages for command and event pipelines to a queue on the [control bus](http://www.enterpriseintegrationpatterns.com/patterns/messaging/ControlBus.html). Brighter Monitoring reads from that firehose and outputs to the console. It could be used as the first filter in a chain for processing monitoring output, or as an example of how to consume the monitoring firehose. 

### Brighter Management
Brighter Dispatcher instances can be configured at runtime by sending them messages across the control bus. This project provides a CLI for configuring dispatcher instances






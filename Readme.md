<h1>Kafka Consume Bug Demonstration</h1>

This repo demonstrates an issue with the kafka consumer where when the consumer first starts consuming messages a nonfatal ConsumeException is thrown with the message "no previously committed offset available: Local: No offset stored" then after that each attempt to consume times out.<br /><br />

To see this issue in action add breakpoints to line 49, 53, 57 and 63 of the file ConsumeMessages\PartitionConsumer.cs and start the docker-compose project in debug mode. The breakpoint will be hit and you will see the exception thrown. After the exception is thrown the consumer will continue to attempt to consume messages but each attempt will time out.<br /><br />

If you remove the breakpoint from line 49 consuming works as expected. This issue seems to happen any time you put a breakpoint after consumer.Assign (line 34) and before consumer.Consume completes (line 50).
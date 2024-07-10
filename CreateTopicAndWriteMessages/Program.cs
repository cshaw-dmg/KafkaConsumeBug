// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Confluent.Kafka.Admin;

var messages = 1000;
var topic = Shared.Config.Topic;
var clientConfig = new ClientConfig()
{
	BootstrapServers = Shared.Config.BootstrapServers,
	SecurityProtocol = Confluent.Kafka.SecurityProtocol.Plaintext,
};
var adminClient = new AdminClientBuilder(clientConfig).Build();

try
{
	await adminClient.DescribeTopicsAsync(TopicCollection.OfTopicNames(new string[] { topic }));

	Console.WriteLine($"Topic {topic} already exists, not creating");
}
catch (DescribeTopicsException ex)
{
	Console.WriteLine(ex.ToString());

	Console.WriteLine($"Creating topic: {topic}");

	await adminClient.CreateTopicsAsync(new TopicSpecification[]
	{
		new() { Name = topic, NumPartitions = Shared.Config.NumberOfPartitions},
	});
}

var producer = new ProducerBuilder<byte[], byte[]>(new ProducerConfig(clientConfig)).Build();

Console.WriteLine($"Writing {messages} messages to the topic");

var lastMessageWritten = DateTime.UtcNow;

for (var i = 0; i < messages; i++)
{
	await producer.ProduceAsync(topic, new Message<byte[], byte[]>
	{
		Key = Guid.NewGuid().ToByteArray(),
		Value = Guid.NewGuid().ToByteArray(),
	});

	if ((DateTime.UtcNow - lastMessageWritten).TotalSeconds > 1)
	{
		Console.WriteLine($"{i} of {messages} messages have been written");
		lastMessageWritten = DateTime.UtcNow;
	}
}

Console.WriteLine("Done writing messages");

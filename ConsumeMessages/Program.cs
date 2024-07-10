using ConsumeMessages;

Console.WriteLine("Starting consumers");
var groupId = $"kafka-consumer-bug-{Guid.NewGuid()}";
var consumers = new List<Task>();

for (var currentPartiton = 0; currentPartiton < Shared.Config.NumberOfPartitions; ++currentPartiton)
{
	var partition = currentPartiton;

	consumers.Add(Task.Run(() => new PartitionConsumer().StartConsuming(partition, groupId)));
}

await Task.WhenAll(consumers);

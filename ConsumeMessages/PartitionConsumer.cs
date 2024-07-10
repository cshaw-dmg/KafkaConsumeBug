using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsumeMessages;
internal class PartitionConsumer
{
	public void StartConsuming(int partition, string groupId)
	{
		var consumerConfig = new ConsumerConfig()
		{
			GroupId = groupId,
			AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Error,
			EnableAutoOffsetStore = true,
			GroupInstanceId = $"{groupId}-{partition}",
			SessionTimeoutMs = 120000,
			BootstrapServers = Shared.Config.BootstrapServers,
			SecurityProtocol = Confluent.Kafka.SecurityProtocol.Plaintext,
			Debug = "consumer,cgrp,topic,fetch",
		};
		var topicPartition = new TopicPartition(Shared.Config.Topic, partition);

		var consumer = new ConsumerBuilder<byte[], byte[]>(consumerConfig).Build();

		Console.WriteLine($"Consumer-{partition}: subscribing to topic");
		consumer.Subscribe(topicPartition.Topic);

		Console.WriteLine($"Consumer-{partition}: assigning topicPartition");
		consumer.Assign(new TopicPartitionOffset(topicPartition, Offset.Beginning));

		consumer.Seek(new TopicPartitionOffset(topicPartition, Offset.Beginning));

		Console.WriteLine($"Consumer-{partition}: getting watermark offsets");
		consumer.GetWatermarkOffsets(topicPartition);

		Console.WriteLine($"Consumer-{partition}: querying watermark offsets");
		consumer.QueryWatermarkOffsets(topicPartition, TimeSpan.FromSeconds(10));

		while (true)
		{
			try
			{
				Console.WriteLine($"Consumer-{partition}: starting consume");
				var result = consumer.Consume(TimeSpan.FromSeconds(10));

				if (result == null)
				{
					Console.WriteLine($"Consumer-{partition}: consume timed out");
				}
				else
				{
					Console.WriteLine($"Consumer-{partition}: consume successful");
					break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Consumer-{partition}: Exception: {ex}");
			}
		}

		Console.WriteLine($"Consumer-{partition}: closing consumer");
	}
}

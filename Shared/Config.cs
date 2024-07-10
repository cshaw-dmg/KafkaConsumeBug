namespace Shared;
public static class Config
{
	public static string BootstrapServers => "kafka:9092";

	public static string Topic => "test-topic";

	public static int NumberOfPartitions => 1;
}

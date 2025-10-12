namespace TIMS_X.Server.Models;

public class AwsSmsNotification
{
	public string DestinationNumber { get; set; }

	public string InboundMessageId { get; set; }

	public string MessageBody { get; set; }

	public string MessageKeyword { get; set; }

	public string OriginationNumber { get; set; }

	public string PreviousPublishedMessageId { get; set; }
}

public class SubscriptionConfirmation
{
	public string Message { get; set; }

	public string MessageId { get; set; }

	public string Signature { get; set; }

	public string SignatureCertURL { get; set; }

	public string SignatureVersion { get; set; }

	public string SubscribeURL { get; set; }

	public string Timestamp { get; set; }

	public string Token { get; set; }

	public string TopicArn { get; set; }

	public string Type { get; set; }
}

public class SnsNotification
{
	public string Message { get; set; }

	public string MessageId { get; set; }

	public string Signature { get; set; }

	public string SignatureCertURL { get; set; }

	public string SignatureVersion { get; set; }

	public string Subject { get; set; }

	public string Timestamp { get; set; }

	public string TopicArn { get; set; }

	public string Type { get; set; }

	public string UnsubscribeURL { get; set; }
}
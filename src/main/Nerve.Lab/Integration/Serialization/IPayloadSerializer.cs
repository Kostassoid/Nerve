namespace Kostassoid.Nerve.Lab.Integration.Serialization
{
	public interface IPayloadSerializer
	{
		byte[] Serialize(object payload);
		object Deserialize(byte[] serialized);
	}
}
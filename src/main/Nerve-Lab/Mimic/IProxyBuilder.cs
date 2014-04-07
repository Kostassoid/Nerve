namespace Kostassoid.Nerve.Lab.Mimic
{
	using Core;

	public interface IProxyBuilder
	{
		T Build<T>(ICell cell) where T : class;
	}
}
namespace Kostassoid.Nerve.Lab.Mimic.LinFu
{
	using Core;
	using global::LinFu.DynamicProxy;

	public class LinFuProxyBuilder : IProxyBuilder
	{
		private readonly ProxyFactory _factory = new ProxyFactory();

		public T Build<T>(ICell cell) where T : class
		{
			return _factory.CreateProxy<T>(new LinFuInterceptor(cell));
		}
	}
}
namespace Kostassoid.Nerve.Lab.Mimic.Castle
{
	using Core;
	using global::Castle.DynamicProxy;

	public class CastleProxyBuilder : Mimic.IProxyBuilder
	{
		private readonly ProxyGenerator _factory = new ProxyGenerator();

		public T Build<T>(ICell cell) where T : class
		{
			return _factory.CreateInterfaceProxyWithoutTarget<T>(new CastleInterceptor(cell));
		}
	}
}
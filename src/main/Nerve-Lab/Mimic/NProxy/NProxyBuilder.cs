namespace Kostassoid.Nerve.Lab.Mimic.NProxy
{
	using System;
	using Core;
	using global::NProxy.Core;

	public class NProxyBuilder : IProxyBuilder
	{
		private readonly ProxyFactory _factory = new ProxyFactory();

		public T Build<T>(ICell cell) where T : class
		{
			return _factory.CreateProxy<T>(Type.EmptyTypes, new NProxyInterceptor(cell));
		}
	}
}
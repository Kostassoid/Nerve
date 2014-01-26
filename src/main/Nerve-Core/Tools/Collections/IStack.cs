namespace Kostassoid.Nerve.Core.Tools.Collections
{
	using System.Collections.Generic;

	public interface IStack<T> : IEnumerable<T>
	{
		IStack<T> Push(T value);
		IStack<T> Pop();
		T Peek();
		bool IsEmpty { get; }
	}
}
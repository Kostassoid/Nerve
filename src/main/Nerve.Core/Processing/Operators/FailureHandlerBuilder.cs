namespace Kostassoid.Nerve.Core.Processing.Operators
{
	using System;
	using System.Collections.Generic;
	using Tools.CodeContracts;

	/// <summary>
	/// Untyped failure handler builder.
	/// </summary>
	public class FailureHandlerBuilder
	{
		/// <summary>
		/// Failure handler delegate.
		/// </summary>
		/// <param name="exception">Exception.</param>
		/// <param name="signal">Signal.</param>
		protected delegate bool FailureHandlerDelegate(Exception exception, ISignal signal);

		private readonly IDictionary<Type, FailureHandlerDelegate> _handlers
			= new Dictionary<Type, FailureHandlerDelegate>();

		/// <summary>
		/// Register failure handler for specific exception type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="failureHandler"></param>
		protected void Register(Type type, FailureHandlerDelegate failureHandler)
		{
			Requires.True(typeof(Exception).IsAssignableFrom(type), "type", "Expected exception type but got [{0}].", type);

			_handlers[type] = failureHandler;
		}

		/// <summary>
		/// Defines a handler for specific exception type.
		/// </summary>
		/// <param name="failureHandler">Exception handler.</param>
		/// <typeparam name="TException">Expected exception type.</typeparam>
		public void On<TException>(Func<TException, ISignal, bool> failureHandler)
			where TException : Exception
		{
			Register(typeof(TException), (exception, signal)
				=> failureHandler((TException)exception, signal));
		}

		private bool Process(SignalException exception)
		{
			FailureHandlerDelegate failureHandler;
			if (!_handlers.TryGetValue(exception.InnerException.GetType(), out failureHandler))
				return false;

			return failureHandler(exception.InnerException, exception.Signal);
		}

		internal Func<SignalException, bool> Build()
		{
			return Process;
		}
	}

	/// <summary>
	/// Typed failure handler builder.
	/// </summary>
	public class FailureHandlerBuilder<T> : FailureHandlerBuilder
	{
		/// <summary>
		/// Defines a handler for specific exception type.
		/// </summary>
		/// <param name="failureHandler">Exception handler.</param>
		/// <typeparam name="TException">Expected exception type.</typeparam>
		public void On<TException>(Func<TException, ISignal<T>, bool> failureHandler)
			where TException : Exception
		{
			Register(typeof(TException), (exception, signal)
				=> failureHandler((TException)exception, signal.As<T>()));
		}
	}
}
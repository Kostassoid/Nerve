namespace EventStore
{
	using System.Threading.Tasks;

	using Kostassoid.Nerve.Core;

	public class TaskSignalHandler : ISignalProcessor
	{
		readonly TaskCompletionSource<object> _completionSource = new TaskCompletionSource<object>();

		public Task<object> Task
		{
			get
			{
				return _completionSource.Task;
			}
		}

		public void OnSignal(ISignal signal)
		{
			_completionSource.SetResult(signal.Payload);
		}

		public bool OnFailure(SignalException exception)
		{
			_completionSource.SetException(exception.InnerException);
			return true;
		}
	}
}
namespace Kostassoid.Nerve.Core.Fibers.Core
{
	using System;

	///<summary>
    /// Allows for the registration and deregistration of subscriptions
    ///</summary>
    internal interface ISubscriptionRegistry
    {
        ///<summary>
        /// Register subscription to be unsubcribed from when the fiber is disposed.
        ///</summary>
        ///<param name="toAdd"></param>
        void RegisterSubscription(IDisposable toAdd);

        ///<summary>
        /// Deregister a subscription.
        ///</summary>
        ///<param name="toRemove"></param>
        ///<returns></returns>
        bool DeregisterSubscription(IDisposable toRemove);
    }
}
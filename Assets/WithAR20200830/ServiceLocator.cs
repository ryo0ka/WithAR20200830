using System;
using System.Collections.Generic;

namespace WithAR20200830
{
	// who said this is an anti pattern? totally fine for a hackathon >:)
	public sealed class ServiceLocator
	{
		public static ServiceLocator Instance = new ServiceLocator();

		readonly Dictionary<Type, object> _services;

		public ServiceLocator()
		{
			_services = new Dictionary<Type, object>();
		}

		public void Register<T>(T service)
		{
			if (_services.ContainsKey(typeof(T)))
			{
				throw new Exception($"Service already registered for type {typeof(T)}");
			}

			_services.Add(typeof(T), service);
		}

		public T Locate<T>()
		{
			if (!_services.TryGetValue(typeof(T), out var service))
			{
				throw new Exception($"Service not found for type {typeof(T)}");
			}

			return (T) service;
		}
	}
}
using System;

/// <summary>
/// Like Lazy, only can recreate the object on demand
/// </summary>
/// <typeparam name="T"></typeparam>
public class LazyWeak<T>
{
	private static readonly object __noObject = 3;

	private readonly Func<T> _factory;
	private readonly WeakReference _reference;


	public LazyWeak(Func<T> factory, T initial)
	{
		_factory = factory;
		_reference = new WeakReference(initial);
	}

	public LazyWeak(Func<T> factory)
	{
		_factory = factory;
		_reference = new WeakReference(__noObject);
	}

	public bool IsValueCreated
	{
		get	{
			return _reference.IsAlive && !ReferenceEquals(_reference.Target, __noObject);
		}
	}

	public T Value
	{
		get {
			var result = _reference.Target;
			if (ReferenceEquals(result, __noObject) || !_reference.IsAlive)
				_reference.Target = result = _factory();
			return (T)result;
		}
	}
}
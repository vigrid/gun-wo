public class ArrayIterator<T> where T : class
{
	private int _index;
	private readonly T[] _array;


	public ArrayIterator(T[] array)
	{
		_array = array;
	}


	public T Current
	{
		get
		{
			if (_array != null && _array.Length > 0)
			{
				return _array[_index];
			}

			return null;
		}
	}


	public void Next()
	{
		if (_array != null && _array.Length > 0)
		{
			_index = (_index + 1) % _array.Length;
		}
	}

	public void Prev()
	{
		if (_array != null && _array.Length > 0)
		{
			_index = (_index + _array.Length - 1) % _array.Length;
		}
	}
}
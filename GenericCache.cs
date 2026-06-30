public class GenericCache<T>
{
    private readonly Dictionary<string, CacheItem<T>> _cache = new();
    private readonly ReaderWriterLockSlim _lock = new();

    public void Add(string key, T value, TimeSpan expirationDuration)
    {
        _lock.EnterWriteLock();
        try
        {
            CacheItem<T> item = new()
            {
                Value = value,
                ExpirationTime = DateTime.Now.Add(expirationDuration)
            };

            _cache[key] = item;
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public T GetItem(string key)
    {
        _lock.EnterReadLock();
        try
        {
            if (_cache.TryGetValue(key, out var cacheItem))
            {
                if (cacheItem.IsExpired())
                {
                    _lock.ExitReadLock();
                    _lock.EnterWriteLock();
                    try
                    {
                        _cache.Remove(key);
                        throw new KeyNotFoundException($"The item with key '{key}' has expired and has been removed from the cache.");
                    }
                    finally
                    {
                        _lock.ExitWriteLock();
                    }
                }

                return cacheItem.Value;
            }

            throw new KeyNotFoundException($"The item with key '{key}' was not found in the cache.");
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}

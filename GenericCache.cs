public class GenericCache<T>
{
    private readonly Dictionary<string, CacheItem<T>> _cache = new();

    public void Add(string key, T value, TimeSpan expirationDuration)
    {
        CacheItem<T> item = new()
        {
            Value = value,
            ExpirationTime = DateTime.Now.Add(expirationDuration)
        };

        _cache[key] = item;
    }

    public T GetItem(string key)
    {
        if (_cache.TryGetValue(key, out var cacheItem))
        {
            if (cacheItem.IsExpired())
            {
                _cache.Remove(key);
                throw new KeyNotFoundException($"The item with key '{key}' has expired and has been removed from the cache.");
            }

            return cacheItem.Value;
        }

        throw new KeyNotFoundException($"The item with key '{key}' was not found in the cache.");
    }
}
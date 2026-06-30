public class CacheItem<T>
{
    public required T Value { get; set; }
    public required DateTime ExpirationTime { get; set; }

    public bool IsExpired()
    {
        return DateTime.Now > ExpirationTime;
    }
}
using StackExchange.Redis;

namespace FidoDino.Infrastructure.Redis
{
    public class RedisConnectionFactory
    {
        private readonly string _connectionString;
        private Lazy<ConnectionMultiplexer> _connection;

        /// <summary>
        /// Khởi tạo RedisConnectionFactory với chuỗi kết nối Redis.
        /// </summary>
        public RedisConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_connectionString));
        }

        /// <summary>
        /// Lấy kết nối Redis đồng bộ (singleton).
        /// </summary>
        public ConnectionMultiplexer GetConnection() => _connection.Value;
        /// <summary>
        /// Lấy kết nối Redis bất đồng bộ (mỗi lần gọi tạo kết nối mới).
        /// </summary>
        public async Task<ConnectionMultiplexer> GetConnectionAsync() => await ConnectionMultiplexer.ConnectAsync(_connectionString);
    }
}
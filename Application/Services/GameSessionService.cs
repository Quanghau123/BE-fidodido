using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Interfaces.Game;
using FidoDino.Application.Interfaces;
using StackExchange.Redis;
using FidoDino.Application.Interface;
using FidoDino.Application.DTOs.Game;
using FidoDino.Common.Exceptions;
using System.Text.Json;

namespace FidoDino.Application.Services
{
    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionRepository _sessionRepository;
        private readonly IDatabase _redis;
        private readonly IEffectService _effectService;

        private const int SESSION_CACHE_HOURS = 2;

        public GameSessionService(
            IGameSessionRepository sessionRepository,
            IConnectionMultiplexer redis,
            IEffectService effectService)
        {
            _sessionRepository = sessionRepository;
            _redis = redis.GetDatabase();
            _effectService = effectService;
        }

        /// <summary>
        /// Lấy phiên chơi đang hoạt động của người dùng (Redis cache-first, DB là source of truth).
        /// </summary>
        public async Task<GameSession?> GetActiveSessionAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            // Try Redis
            var sessionIdVal = await _redis.StringGetAsync($"game:session:user:{userId}");
            if (sessionIdVal.HasValue &&
                Guid.TryParse(sessionIdVal!, out var cachedSessionId))
            {
                var cachedSession = await _sessionRepository.GetByIdAsync(cachedSessionId);

                // Session đã end hoặc không tồn tại → clear cache
                if (cachedSession == null || cachedSession.EndTime != null)
                {
                    await ClearSessionCacheAsync(userId, cachedSessionId);
                }
                else
                {
                    return cachedSession; // Active thật
                }
            }

            // Fallback DB (truth)
            var session = await _sessionRepository.GetActiveSessionByUserIdAsync(userId);
            if (session == null)
                return null;

            // Restore Redis
            await CacheSessionAsync(session);

            return session;
        }

        /// <summary>
        /// Bắt đầu phiên chơi mới cho người dùng.
        /// </summary>
        public async Task<GameSessionDto> StartSessionAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId is required");

            if (await _effectService.HasEffectAsync(userId, "BlockPlay"))
                throw new ForbiddenException("User is currently blocked from starting a session.");

            var activeSession = await _sessionRepository.GetActiveSessionByUserIdAsync(userId);
            if (activeSession != null)
                throw new ConflictException("User already has an active session.");

            var session = new GameSession
            {
                GameSessionId = Guid.NewGuid(),
                UserId = userId,
                TotalScore = 0,
                IsActive = true,
                StartTime = DateTime.UtcNow
            };

            await _sessionRepository.AddAsync(session);

            // Cache Redis ngay khi start
            await CacheSessionAsync(session);

            return new GameSessionDto
            {
                GameSessionId = session.GameSessionId,
                UserId = session.UserId,
                TotalScore = session.TotalScore,
                StartTime = session.StartTime,
                EndTime = session.EndTime
            };
        }

        /// <summary>
        /// Kết thúc phiên chơi và clear cache Redis.
        /// </summary>
        public async Task EndSessionAsync(Guid sessionId)
        {
            if (sessionId == Guid.Empty)
                throw new ArgumentException("SessionId is required");

            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session == null)
                throw new NotFoundException("Session not found");

            if (session.EndTime != null)
                return;

            session.EndTime = DateTime.UtcNow;
            session.IsActive = false;

            await _sessionRepository.UpdateAsync(session);

            // Clear Redis
            await ClearSessionCacheAsync(session.UserId, session.GameSessionId);
        }

        private async Task CacheSessionAsync(GameSession session)
        {
            await _redis.StringSetAsync(
                $"game:session:{session.GameSessionId}",
                JsonSerializer.Serialize(new
                {
                    session.UserId,
                    session.TotalScore
                }),
                TimeSpan.FromHours(SESSION_CACHE_HOURS)
            );

            await _redis.StringSetAsync(
                $"game:session:user:{session.UserId}",
                session.GameSessionId.ToString(),
                TimeSpan.FromHours(SESSION_CACHE_HOURS)
            );
        }

        private async Task ClearSessionCacheAsync(Guid userId, Guid sessionId)
        {
            await _redis.KeyDeleteAsync($"game:session:{sessionId}");
            await _redis.KeyDeleteAsync($"game:session:user:{userId}");
        }
    }
}

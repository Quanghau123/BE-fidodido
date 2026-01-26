using System.Text.Json;
using FidoDino.Application.Interfaces;
using FidoDino.Domain.Entities.Game;
using FidoDino.Domain.Enums.Game;
using Microsoft.EntityFrameworkCore;

namespace FidoDino.Infrastructure.Data.Seeders
{

    public class IceRewardSeeder : ISeeder
    {
        public async Task SeedAsync(FidoDinoDbContext context)
        {
            if (await context.Ices.AnyAsync()) return;
            var json = await File.ReadAllTextAsync("Infrastructure/Data/Seeders/IceReward.json");
            var items = JsonSerializer.Deserialize<List<IceSeedModel>>(json);
            if (items == null) return;

            foreach (var iceSeed in items)
            {
                var ice = new Ice
                {
                    IceId = Guid.NewGuid(),
                    IceType = Enum.Parse<IceType>(iceSeed.IceType),
                    RequiredShake = iceSeed.RequiredShake,
                    Probability = iceSeed.Probability
                };
                context.Ices.Add(ice);
                foreach (var rewardSeed in iceSeed.Rewards)
                {
                    Effect? effect = null!;
                    Guid? effectId = null;
                    if (!string.IsNullOrEmpty(rewardSeed.EffectType) && rewardSeed.EffectType != "None")
                    {
                        effect = new Effect
                        {
                            EffectId = Guid.NewGuid(),
                            EffectType = Enum.Parse<EffectType>(rewardSeed.EffectType),
                            DurationSeconds = rewardSeed.DurationSeconds
                        };
                        context.Effects.Add(effect);
                        effectId = effect.EffectId;
                    }
                    var reward = new Reward
                    {
                        RewardId = Guid.NewGuid(),
                        RewardName = rewardSeed.RewardName,
                        Score = rewardSeed.Score,
                        EffectId = effectId,
                        Effect = effect
                    };
                    context.Rewards.Add(reward);
                    var iceReward = new IceReward
                    {
                        IceRewardId = Guid.NewGuid(),
                        IceId = ice.IceId,
                        RewardId = reward.RewardId,
                        Probability = rewardSeed.Probability
                    };
                    context.IceRewards.Add(iceReward);
                }
            }
            await context.SaveChangesAsync();
        }

        private class IceSeedModel
        {
            public string IceType { get; set; } = null!;
            public int RequiredShake { get; set; }
            public double Probability { get; set; }
            public List<RewardSeedModel> Rewards { get; set; } = new();
        }
        private class RewardSeedModel
        {
            public string RewardName { get; set; } = null!;
            public int Score { get; set; }
            public string? EffectType { get; set; }
            public int DurationSeconds { get; set; }
            public double Probability { get; set; }
        }
    }
}

using PatternCipher.Application.Models.v1;
using PatternCipher.Infrastructure.Firebase.CloudSave.DTOs;

namespace PatternCipher.Infrastructure.Firebase.CloudSave.Mappers
{
    /// <summary>
    /// A static helper class to map between domain models and data transfer objects (DTOs).
    /// This decouples the core application logic from the data persistence format.
    /// </summary>
    public static class PlayerProfileMapper
    {
        /// <summary>
        /// Maps a domain <see cref="PlayerProfile"/> object to a <see cref="PlayerProfileDto"/>.
        /// </summary>
        /// <param name="domainModel">The domain model to convert.</param>
        /// <returns>A new DTO instance with data from the domain model.</returns>
        public static PlayerProfileDto ToDto(PlayerProfile domainModel)
        {
            if (domainModel == null) return null;

            return new PlayerProfileDto
            {
                Progress = new PlayerProgressDto
                {
                    LastCompletedLevel = domainModel.Progress?.LastCompletedLevel ?? 0
                    // Map other progress fields here
                },
                Settings = new PlayerSettingsDto
                {
                    IsSoundEnabled = domainModel.Settings?.IsSoundEnabled ?? true,
                    IsHapticsEnabled = domainModel.Settings?.IsHapticsEnabled ?? true
                    // Map other settings fields here
                }
                // LastUpdated is handled by Firestore [ServerTimestamp]
            };
        }

        /// <summary>
        /// Maps a <see cref="PlayerProfileDto"/> object back to a domain <see cref="PlayerProfile"/>.
        /// </summary>
        /// <param name="dto">The DTO to convert.</param>
        /// <returns>A new domain model instance with data from the DTO.</returns>
        public static PlayerProfile ToDomain(PlayerProfileDto dto)
        {
            if (dto == null) return null;

            return new PlayerProfile
            {
                Progress = new PlayerProgress
                {
                    LastCompletedLevel = dto.Progress?.LastCompletedLevel ?? 0
                    // Map other progress fields here
                },
                Settings = new PlayerSettings
                {
                    IsSoundEnabled = dto.Settings?.IsSoundEnabled ?? true,
                    IsHapticsEnabled = dto.Settings?.IsHapticsEnabled ?? true
                    // Map other settings fields here
                }
                // LastUpdated from the DTO can be used for conflict resolution if needed, but not part of the core domain model.
            };
        }
    }
}
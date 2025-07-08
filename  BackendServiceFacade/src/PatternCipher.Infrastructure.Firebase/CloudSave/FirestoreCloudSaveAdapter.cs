using Firebase.Firestore;
using PatternCipher.Application.Models.v1;
using PatternCipher.Application.Services;
using PatternCipher.Infrastructure.Firebase.CloudSave.DTOs;
using PatternCipher.Infrastructure.Firebase.CloudSave.Mappers;
using PatternCipher.Infrastructure.Firebase.Common;
using System;
using System.Threading.Tasks;

namespace PatternCipher.Infrastructure.Firebase.CloudSave
{
    /// <summary>
    /// Implements the cloud save service interface using Cloud Firestore.
    /// It is responsible for serializing domain models into Firestore documents and vice-versa.
    /// </summary>
    public class FirestoreCloudSaveAdapter : ICloudSaveService
    {
        private readonly FirebaseFirestore _db;
        private const string UsersCollection = "users";

        /// <summary>
        /// Initializes a new instance of the <see cref="FirestoreCloudSaveAdapter"/> class.
        /// </summary>
        public FirestoreCloudSaveAdapter()
        {
            _db = FirebaseFirestore.DefaultInstance;
        }

        /// <inheritdoc/>
        public async Task<FirebaseResult> SavePlayerProfileAsync(string userId, PlayerProfile profile)
        {
            if (string.IsNullOrEmpty(userId) || profile == null)
            {
                return FirebaseResult.Failure(new FirebaseError(0, "Invalid user ID or profile data."));
            }

            try
            {
                PlayerProfileDto dto = PlayerProfileMapper.ToDto(profile);
                DocumentReference docRef = _db.Collection(UsersCollection).Document(userId);
                await docRef.SetAsync(dto, SetOptions.MergeAll);
                return FirebaseResult.Success();
            }
            catch (FirestoreException ex)
            {
                return FirebaseResult.Failure(new FirebaseError((int)ex.ErrorCode, ex.Message, ex));
            }
            catch (Exception ex)
            {
                return FirebaseResult.Failure(new FirebaseError(0, ex.Message, ex));
            }
        }

        /// <inheritdoc/>
        public async Task<FirebaseResult<PlayerProfile>> LoadPlayerProfileAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return FirebaseResult<PlayerProfile>.Failure(new FirebaseError(0, "User ID cannot be null or empty."));
            }

            try
            {
                DocumentReference docRef = _db.Collection(UsersCollection).Document(userId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    PlayerProfileDto dto = snapshot.ConvertTo<PlayerProfileDto>();
                    PlayerProfile profile = PlayerProfileMapper.ToDomain(dto);
                    return FirebaseResult<PlayerProfile>.Success(profile);
                }
                else
                {
                    return FirebaseResult<PlayerProfile>.Failure(new FirebaseError(404, "Player profile not found."));
                }
            }
            catch (FirestoreException ex)
            {
                return FirebaseResult<PlayerProfile>.Failure(new FirebaseError((int)ex.ErrorCode, ex.Message, ex));
            }
            catch (Exception ex)
            {
                return FirebaseResult<PlayerProfile>.Failure(new FirebaseError(0, ex.Message, ex));
            }
        }
    }
}
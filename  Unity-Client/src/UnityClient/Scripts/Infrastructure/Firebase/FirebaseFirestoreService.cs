using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Firestore;
using UnityEngine; // For Debug

namespace PatternCipher.Client.Infrastructure.Firebase
{
    public class FirebaseFirestoreService
    {
        private FirebaseFirestore _db;
        public bool IsInitialized { get; private set; } = false;

        public FirebaseFirestoreService()
        {
            // Initialization of _db should happen after FirebaseApp is ready.
        }

        public async Task InitializeAsync()
        {
            if (IsInitialized) return;

            // Assuming FirebaseApp.CheckAndFixDependenciesAsync() has been called elsewhere
            _db = FirebaseFirestore.DefaultInstance;
            if (_db != null)
            {
                IsInitialized = true;
                Debug.Log("FirebaseFirestoreService Initialized.");
                // Example: Configure settings like persistence
                // FirebaseFirestoreSettings settings = new FirebaseFirestoreSettings();
                // settings.PersistenceEnabled = true; // Enable offline persistence
                // _db.Settings = settings;
            }
            else
            {
                Debug.LogError("FirebaseFirestoreService: FirebaseFirestore.DefaultInstance is null. Firebase not initialized correctly.");
            }
            await Task.CompletedTask;
        }

        public async Task SetDocumentAsync<T>(string collectionPath, string documentId, T data) where T : class
        {
            if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return; }
            if (string.IsNullOrEmpty(collectionPath) || string.IsNullOrEmpty(documentId) || data == null)
            {
                Debug.LogError("Invalid parameters for SetDocumentAsync.");
                return;
            }

            DocumentReference docRef = _db.Collection(collectionPath).Document(documentId);
            try
            {
                await docRef.SetAsync(data, SetOptions.MergeAll); // MergeAll to update fields or create if not exists
                Debug.Log($"Firestore: Document '{documentId}' in '{collectionPath}' set/updated.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firestore: Error setting document '{documentId}' in '{collectionPath}': {ex.Message}");
                throw; // Re-throw to allow caller to handle
            }
        }

        public async Task<T> GetDocumentAsync<T>(string collectionPath, string documentId) where T : class
        {
            if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return null; }
             if (string.IsNullOrEmpty(collectionPath) || string.IsNullOrEmpty(documentId))
            {
                Debug.LogError("Invalid parameters for GetDocumentAsync.");
                return null;
            }

            DocumentReference docRef = _db.Collection(collectionPath).Document(documentId);
            try
            {
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
                if (snapshot.Exists)
                {
                    Debug.Log($"Firestore: Document '{documentId}' from '{collectionPath}' retrieved.");
                    return snapshot.ConvertTo<T>();
                }
                else
                {
                    Debug.LogWarning($"Firestore: Document '{documentId}' in '{collectionPath}' does not exist.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firestore: Error getting document '{documentId}' from '{collectionPath}': {ex.Message}");
                return null; // Or throw
            }
        }

        public async Task UpdateDocumentAsync(string collectionPath, string documentId, Dictionary<string, object> updates)
        {
            if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return; }
            if (string.IsNullOrEmpty(collectionPath) || string.IsNullOrEmpty(documentId) || updates == null || updates.Count == 0)
            {
                Debug.LogError("Invalid parameters for UpdateDocumentAsync.");
                return;
            }

            DocumentReference docRef = _db.Collection(collectionPath).Document(documentId);
            try
            {
                await docRef.UpdateAsync(updates);
                Debug.Log($"Firestore: Document '{documentId}' in '{collectionPath}' updated.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firestore: Error updating document '{documentId}' in '{collectionPath}': {ex.Message}");
                throw;
            }
        }

        public async Task DeleteDocumentAsync(string collectionPath, string documentId)
        {
            if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return; }
            if (string.IsNullOrEmpty(collectionPath) || string.IsNullOrEmpty(documentId))
            {
                Debug.LogError("Invalid parameters for DeleteDocumentAsync.");
                return;
            }
            DocumentReference docRef = _db.Collection(collectionPath).Document(documentId);
            try
            {
                await docRef.DeleteAsync();
                Debug.Log($"Firestore: Document '{documentId}' in '{collectionPath}' deleted.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firestore: Error deleting document '{documentId}' in '{collectionPath}': {ex.Message}");
                throw;
            }
        }

        public async Task<List<T>> QueryCollectionAsync<T>(string collectionPath, Query query = null) where T : class
        {
            if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return new List<T>(); }
             if (string.IsNullOrEmpty(collectionPath))
            {
                Debug.LogError("Invalid collectionPath for QueryCollectionAsync.");
                return new List<T>();
            }

            Query collectionQuery = query ?? _db.Collection(collectionPath);
            List<T> results = new List<T>();
            try
            {
                QuerySnapshot querySnapshot = await collectionQuery.GetSnapshotAsync();
                foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
                {
                    if (documentSnapshot.Exists)
                    {
                        results.Add(documentSnapshot.ConvertTo<T>());
                    }
                }
                Debug.Log($"Firestore: Query on '{collectionPath}' returned {results.Count} documents.");
                return results;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Firestore: Error querying collection '{collectionPath}': {ex.Message}");
                return new List<T>(); // Or throw
            }
        }
        
        public CollectionReference GetCollectionReference(string collectionPath)
        {
            if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return null; }
            return _db.Collection(collectionPath);
        }

        public DocumentReference GetDocumentReference(string collectionPath, string documentId)
        {
             if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return null; }
            return _db.Collection(collectionPath).Document(documentId);
        }

        public WriteBatch CreateBatch()
        {
            if (!IsInitialized) { Debug.LogError("Firestore not initialized."); return null; }
            return _db.StartBatch();
        }
    }
}
// src/infrastructure/repositories/leaderboard.repository.ts
import { db } from '../firebase';
import { FIRESTORE_COLLECTIONS } from '../../config/constants';
import { LeaderboardEntry } from '../../domain/models/leaderboard-entry.model';
import { Firestore } from 'firebase-admin/firestore';

export class LeaderboardRepository {
    constructor(private readonly firestore: Firestore) {}

    /**
     * Writes a new leaderboard entry to the database.
     * @param {LeaderboardEntry} entry - The leaderboard entry to persist.
     */
    async submitScore(entry: LeaderboardEntry): Promise<void> {
        // We could set a specific document ID, but letting Firestore auto-generate it is fine.
        await this.firestore.collection(FIRESTORE_COLLECTIONS.LEADERBOARDS).add(entry);
    }
}
// src/domain/models/leaderboard-entry.model.ts
import { FieldValue } from 'firebase-admin/firestore';

export interface LeaderboardEntry {
    userId: string;
    userName: string;
    levelId: string;
    score: number;
    moves: number;
    timeInSeconds: number;
    submittedAt: FieldValue; // Will be a server timestamp
}
// src/application/leaderboard.service.ts
import { ScoreValidatorService } from '../domain/services/score-validator.service';
import { LeaderboardRepository } from '../infrastructure/repositories/leaderboard.repository';
import { LeaderboardEntry } from '../domain/models/leaderboard-entry.model';
import { SubmitScoreDto } from './dtos/submit-score.dto';
import { FieldValue } from 'firebase-admin/firestore';
import { HttpsError, CallableContext } from 'firebase-functions/v1/https';
import { logger } from 'firebase-functions';

export class LeaderboardService {
    constructor(
        private readonly scoreValidator: ScoreValidatorService,
        private readonly leaderboardRepository: LeaderboardRepository,
    ) {}

    /**
     * Handles the full process of a score submission.
     * @param {SubmitScoreDto} submissionDto - The incoming data from the client.
     * @param {CallableContext} context - The authentication context of the caller.
     * @throws {HttpsError} - Throws specific errors for invalid data or business rule violations.
     */
    async handleScoreSubmission(
        submissionDto: SubmitScoreDto,
        context: CallableContext
    ): Promise<void> {
        // Ensure auth context is present, although this should be checked at the entry point too.
        if (!context.auth) {
            logger.warn('handleScoreSubmission called without authentication context.');
            throw new HttpsError('unauthenticated', 'The function must be called while authenticated.');
        }
        
        // 1. Validate plausibility (Business Rules)
        if (!this.scoreValidator.isPlausible(submissionDto)) {
            throw new HttpsError('invalid-argument', 'The submitted score is not plausible.');
        }
        
        // 2. Construct Domain Model
        const newEntry: LeaderboardEntry = {
            ...submissionDto,
            userId: context.auth.uid,
            userName: context.auth.token.name || 'Anonymous Player',
            submittedAt: FieldValue.serverTimestamp(),
        };

        // 3. Persist to repository
        await this.leaderboardRepository.submitScore(newEntry);
    }
}
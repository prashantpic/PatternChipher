// src/domain/services/score-validator.service.ts
import { SCORE_VALIDATION_RULES } from '../../config/constants';
import { SubmitScoreDto } from '../../application/dtos/submit-score.dto';

export class ScoreValidatorService {
    /**
     * Checks if a score submission is plausible based on defined business rules.
     * @param {SubmitScoreDto} submission - The score data to validate.
     * @returns {boolean} - True if the score is plausible, false otherwise.
     */
    public isPlausible(submission: SubmitScoreDto): boolean {
        const { score, moves, timeInSeconds } = submission;

        if (score < SCORE_VALIDATION_RULES.MIN_SCORE || score > SCORE_VALIDATION_RULES.MAX_SCORE) {
            return false;
        }
        if (moves < SCORE_VALIDATION_RULES.MIN_MOVES || moves > SCORE_VALIDATION_RULES.MAX_MOVES) {
            return false;
        }
        if (timeInSeconds < SCORE_VALIDATION_RULES.MIN_TIME_SECONDS || timeInSeconds > SCORE_VALIDATION_RULES.MAX_TIME_SECONDS) {
            return false;
        }

        // Future complex logic can be added here. For example:
        // const maxPossibleScore = calculateMaxScoreForLevel(submission.levelId, moves);
        // if (score > maxPossibleScore) { return false; }

        return true;
    }
}
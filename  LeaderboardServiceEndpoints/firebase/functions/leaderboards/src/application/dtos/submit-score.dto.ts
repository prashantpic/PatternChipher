// src/application/dtos/submit-score.dto.ts
import * as Joi from 'joi';

export interface SubmitScoreDto {
    levelId: string;
    score: number;
    moves: number;
    timeInSeconds: number;
}

// Joi schema for validation at the entry point
export const submitScoreSchema = Joi.object<SubmitScoreDto>({
    levelId: Joi.string().required(),
    score: Joi.number().integer().min(0).required(),
    moves: Joi.number().integer().min(1).required(),
    timeInSeconds: Joi.number().integer().min(1).required(),
});
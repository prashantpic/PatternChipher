import { z } from "zod";

/**
 * Defines the schema for the data payload when submitting a score.
 * This enforces the data contract for the `submitScore` callable function.
 */
export const submitScoreSchema = z.object({
  levelId: z.string({
    required_error: "levelId is required.",
    invalid_type_error: "levelId must be a string.",
  }).min(1, { message: "levelId cannot be empty." }),

  score: z.number({
    required_error: "score is required.",
    invalid_type_error: "score must be a number.",
  }).int("score must be an integer.").positive("score must be a positive number."),

  moves: z.number({
    invalid_type_error: "moves must be a number.",
  }).int("moves must be an integer.").positive("moves must be a positive number.").optional(),

  timeInSeconds: z.number({
    invalid_type_error: "timeInSeconds must be a number.",
  }).positive("timeInSeconds must be a positive number.").optional(),
});

/**
 * The inferred TypeScript type from the `submitScoreSchema`.
 * This provides static type checking for validated data.
 */
export type SubmitScorePayload = z.infer<typeof submitScoreSchema>;
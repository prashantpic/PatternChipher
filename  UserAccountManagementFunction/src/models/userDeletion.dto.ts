/**
 * Data Transfer Object for a user deletion request.
 */
export interface UserDeletionRequest {
  /**
   * The unique identifier (UID) of the user to be deleted.
   */
  readonly uid: string;
}
import * as functions from "firebase-functions";
import { onDeleteUserRequest } from "./src/handlers/userDeletion.handler";

// Expose the handler as a deployable cloud function named 'deleteUser'.
export const deleteUser = functions.https.onCall(onDeleteUserRequest);
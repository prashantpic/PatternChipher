# Specification

# 1. Sequence Design Overview

- **Sequence_Diagram:**
  ### . Cloud Save Activation and Synchronization
  Shows the flow of a user enabling the optional Cloud Save feature. This includes linking to a permanent account (Google/Apple), the initial push of local data to the cloud, and the subsequent synchronization of progress after completing a level. It also considers the conflict resolution logic.

  #### .4. Purpose
  To define the interaction pattern for a key online feature, covering authentication, data synchronization, and conflict resolution between local and cloud states.

  #### .5. Type
  UserJourney

  #### .6. Participant Repository Ids
  
  - REPO-PATT-001
  - REPO-PATT-005
  - REPO-PATT-006
  - REPO-PATT-007
  - REPO-PATT-004
  
  #### .7. Key Interactions
  
  - User initiates sign-in via the UI.
  - BackendServiceFacade handles the OAuth flow with AuthenticationServiceEndpoints.
  - After successful sign-in, BackendServiceFacade queries CloudDataEndpoints (Firestore) for existing save data.
  - If no cloud data exists, it reads local data via LocalPersistenceEndpoints and writes it to CloudDataEndpoints.
  - If cloud data exists, it compares timestamps and performs conflict resolution (last-write-wins or prompts user).
  - After a future level completion, the updated local profile is automatically pushed to CloudDataEndpoints by BackendServiceFacade.
  
  #### .8. Related Feature Ids
  
  - FR-ONL-003
  - DM-006
  - DM-002
  
  #### .9. Domain
  Online Services

  #### .10. Metadata
  
  - **Complexity:** High
  - **Priority:** High
  


---

# 2. Sequence Diagram Details

- **Success:** True
- **Cache_Created:** True
- **Status:** refreshed
- **Cache_Id:** vhnnpnpo4kfn6l2opmkfgp00e8idc5eh6evb1dqk
- **Cache_Name:** cachedContents/vhnnpnpo4kfn6l2opmkfgp00e8idc5eh6evb1dqk
- **Cache_Display_Name:** repositories
- **Cache_Status_Verified:** True
- **Model:** models/gemini-2.5-pro-preview-03-25
- **Workflow_Id:** I9v2neJ0O4zJsz8J
- **Execution_Id:** AIzaSyCGei_oYXMpZW-N3d-yH-RgHKXz8dsixhc
- **Project_Id:** 6
- **Record_Id:** 8
- **Cache_Type:** repositories


---


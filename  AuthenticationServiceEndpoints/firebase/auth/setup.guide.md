# Authentication Provider Setup Guide

This document provides the manual, step-by-step instructions required to configure social sign-in providers in their respective developer consoles. These steps cannot be automated and are a prerequisite for the providers to function correctly.

---

## 1. Google Sign-In Setup (for Android & iOS)

### Step 1: Configure Firebase Project
Before Google Sign-In can work, you must link your Android application to Firebase and provide its signing certificate fingerprints.

1.  Navigate to your Firebase project in the [Firebase Console](https://console.firebase.google.com/).
2.  Go to **Project Settings** (click the gear icon next to "Project Overview").
3.  Scroll down to the "Your apps" card. Select your Android application.
4.  Under "SHA certificate fingerprints", click **Add fingerprint**.
5.  Obtain your app's SHA-1 and SHA-256 fingerprints. You can get these from your keystore file using the `keytool` command or directly from Android Studio's Gradle tasks.
6.  Add both the SHA-1 and SHA-256 fingerprints to your Firebase project settings. This is crucial for both debug and release builds.

### Step 2: Google Cloud Console Configuration
Firebase uses the Google Cloud Platform (GCP) for managing OAuth credentials.

1.  Navigate to the [Google Cloud Platform Console](https://console.cloud.google.com/). Ensure you have selected the project that is linked to your Firebase project.
2.  In the navigation menu, go to **APIs & Services > Credentials**.
3.  Under **OAuth 2.0 Client IDs**, you should see client IDs that Firebase automatically created for your app (e.g., "Android client for ...", "iOS client for ..."). If you need to create them manually, select **Create Credentials > OAuth client ID**.
4.  **For Android:** Ensure the client ID has the correct package name and SHA-1 certificate fingerprint associated with it.
5.  **For iOS:** Ensure the client ID has the correct iOS bundle ID.
6.  Navigate to the **OAuth consent screen** tab.
    -   Ensure the **User type** is set to "External".
    -   Fill in the required app information:
        -   **App name:** The public name of your game.
        -   **User support email:** An email address for users to contact for help.
        -   **Developer contact information:** An email address for Google to contact you.
    -   Save the changes. You do not need to submit the app for verification unless you are using sensitive scopes, which Firebase Authentication does not require by default.

### Step 3: Client Integration Files
After making changes in the Firebase console, you must update the configuration files in your Unity project.

1.  In the Firebase Console, go back to **Project Settings > General**.
2.  In the "Your apps" card, find your application.
3.  For Android, download the latest `google-services.json` file.
4.  For iOS, download the latest `GoogleService-Info.plist` file.
5.  Place these files in the `Assets` directory of your Unity project (or the appropriate subdirectory as recommended by the Firebase Unity SDK documentation).

---

## 2. Apple Sign-In Setup (for iOS)

### Step 1: Apple Developer Portal Configuration
Configuring Sign in with Apple requires setting up an App ID and a Services ID.

1.  Navigate to the [Apple Developer Portal](https://developer.apple.com/account/) and go to **Certificates, Identifiers & Profiles**.
2.  Select **Identifiers** from the side menu.
3.  Select your game's **App ID**.
4.  Under the **Capabilities** tab, find and enable **Sign in with Apple**. Click "Edit" and ensure it is set as a primary App ID. Save the changes.
5.  Next, you must create a **Services ID** to represent your app for web authentication flows.
    -   Go back to **Identifiers** and click the `+` button to register a new identifier.
    -   Select **Services IDs** and click "Continue".
    -   Provide a **Description** and an **Identifier** (e.g., `com.yourcompany.yourapp.signin`). This will be used in your backend configuration.
    -   Click "Continue" and then "Register".
    -   Find your newly created Services ID and check the box for **Sign in with Apple**. Click **Configure** to associate it with your primary App ID.

### Step 2: Xcode Project Configuration
After building your Unity project for iOS, you must enable the capability in Xcode.

1.  Open the generated `.xcodeproj` or `.xcworkspace` file in Xcode.
2.  Select the project root in the Project Navigator.
3.  Select your main app target.
4.  Go to the **Signing & Capabilities** tab.
5.  Click **+ Capability**.
6.  Find and double-click **Sign in with Apple** to add it to your project.
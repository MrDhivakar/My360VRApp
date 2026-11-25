# 360 Memories VR App

Unity + Google Cardboard XR app for viewing 360° photos with head tracking.

## Setup Steps

### One-time (local machine)
1. Install **Unity Hub** + **Unity 2022.3.40f1** (LTS).
2. Open this project in Unity once — it auto-generates ProjectSettings.
3. In `Edit > Project Settings > XR Plug-in Management > Android`, enable **Cardboard**.
4. Get Unity Personal license file (`.ulf`) → see GameCI activation guide.

### GitHub Secrets
Add these to `Settings > Secrets and variables > Actions`:
- `UNITY_LICENSE` → contents of your `.ulf` file
- `UNITY_EMAIL` → Unity account email
- `UNITY_PASSWORD` → Unity account password

### Build
Push to `main`. GitHub Actions builds APK → download from **Actions → Artifacts**.

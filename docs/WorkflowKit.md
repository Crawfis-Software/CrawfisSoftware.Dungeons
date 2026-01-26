# Workflow kit (NuGet + Release assets + optional Unity UPM)

The workflow kit will:

- build + pack a .NET library into NuGet artifacts (`.nupkg` + `.snupkg`)
- optionally create a Unity Package Manager (UPM) tarball (`.tgz`) if a Unity package folder exists
- publish NuGet packages to nuget.org using OIDC (`NuGet/login@v1`)
- attach artifacts to the GitHub Release

This repo contains a reusable GitHub Actions setup that you can reuse in two ways:
## **Preferred: use the zip kit**
   - Unzip the workflow-kit.zip file into another repo root and adjust a few `env` values (see below).

## **Manual: copy individual files**
Copy these files and folders into the target repo root:

- `.github/workflows/nuget-package.yml`
- `.github/workflows/release-please.yml`
- `.github/workflows/commitlint.yml`
- `.release-please-config.json`
- `.release-please-manifest.json`
- `commitlint.config.cjs`
- `docs/WorkflowKit.md`

## Minimal repo-specific edits

Edit `.github/workflows/nuget-package.yml` and update the top-level `env` values:

- `DOTNET_PROJECT`: path to the `.csproj` to pack
- `UPM_PACKAGE_ID`: e.g. `com.yourcompany.yourlib`
- `UPM_PACKAGE_DIR`: e.g. `Packages/com.yourcompany.yourlib`

If a repo has **no Unity package**, you can leave the UPM values alone; the workflow will just print a skip message.

## Required GitHub settings

### Secrets

- `NUGET_USER`: username/email used by `NuGet/login@v1` to mint a temporary API key via OIDC

### Environment

- Create an environment named `release` (used by the `publish-nuget` job)

## Expected repo structure (recommended)

- `<RepoRoot>/<YourLibrary>.csproj`
- Optional Unity UPM folder: `Packages/<UPM_PACKAGE_ID>/package.json`

## Creating a new �drop-in zip�

If you change the workflow and want to create a new zip, there is a PowerShell script for that.
From the repository root in a PowerShell-capable terminal (including the GitHub Copilot terminal in Visual Studio), run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/create-workflow-kit.ps1 -OutFile workflow-kit.zip
```

If you omit `-OutFile`, the script will default to `workflow-kit.zip` in the repo root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/create-workflow-kit.ps1
```

The script:

- validates that all expected files exist
- stages them with their relative paths (e.g., `.github/workflows/...`, `docs/WorkflowKit.md`)
- creates `workflow-kit.zip` in the repo root

## Using the kit in another repository

### Quick Setup (Recommended)

1. Unzip `workflow-kit.zip` into the target repo root.
2. Run the complete setup script:
   ```powershell
   # Auto-detect everything (easiest)
   powershell -NoProfile -ExecutionPolicy Bypass -File tools/Setup-NuGetWorkflow.ps1 -AutoDetect

   # Or specify values explicitly
   powershell -NoProfile -ExecutionPolicy Bypass -File tools/Setup-NuGetWorkflow.ps1 `
     -CsprojPath "YourProject.csproj" `
     -Authors "Your Company" `
     -Description "Your library description"
   ```

   This single script:
   - ✅ Updates your .csproj with all required NuGet properties
   - ✅ Updates the workflow file with your project-specific values
   - ✅ Validates the configuration
   - ✅ Provides next-step instructions

3. Follow the displayed next steps to complete GitHub setup.

### Manual Setup (Advanced)

If you prefer to configure things step-by-step:

1. Unzip `workflow-kit.zip` into the target repo root.

2. **Update your .csproj for NuGet packaging:**
   ```powershell
   # Interactive mode
   powershell -NoProfile -ExecutionPolicy Bypass -File tools/Update-CsprojForNuGet.ps1 -CsprojPath "YourProject.csproj"
   ```
   See `tools/CSPROJ-CHECKLIST.md` for manual checklist.

3. **Update workflow configuration:**
   ```powershell
   # Auto-detect values
   powershell -NoProfile -ExecutionPolicy Bypass -File tools/Update-WorkflowConfig.ps1 -AutoDetect

   # Or specify explicitly
   powershell -NoProfile -ExecutionPolicy Bypass -File tools/Update-WorkflowConfig.ps1 `
     -DotnetProject "YourProject.csproj" `
     -UpmPackageId "com.company.package" `
     -UpmPackageDir "Packages/com.company.package"
   ```

4. **GitHub repository settings:**
   - Add secret: `NUGET_USER` (your NuGet.org username/email)
   - Create environment: `release`

5. **Test locally:**
   ```bash
   dotnet pack YourProject.csproj -c Release -o ./artifacts
   # Verify both .nupkg and .snupkg files are created
   ```

6. **Trigger the workflow:**
   - Commit with conventional commit message (e.g., `feat: add new feature`)
   - Push to main branch
   - Release Please will create a PR
   - Merge the PR to trigger release and NuGet publish

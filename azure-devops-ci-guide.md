# Azure DevOps CI/CD Pipeline Guide for .NET Projects

This guide explains how to implement Azure DevOps CI/CD automation similar to the OSDP.Net project. It covers automated builds, testing, code quality enforcement, and multi-platform publishing.

## Table of Contents

- [Overview](#overview)
- [Git Branching Strategy](#git-branching-strategy)
- [Project Structure](#project-structure)
- [Pipeline Configuration](#pipeline-configuration)
- [Build Template](#build-template)
- [Package Template](#package-template)
- [Centralized Project Configuration](#centralized-project-configuration)
- [Code Quality Enforcement](#code-quality-enforcement)
- [Multi-Platform Publishing](#multi-platform-publishing)
- [Setting Up Azure DevOps](#setting-up-azure-devops)
- [Best Practices](#best-practices)

## Overview

The pipeline provides:

- **Automatic builds** on every push to the main branch
- **Tag-based releases** triggered by version tags (e.g., `v1.0.0`)
- **Automated testing** with test result reporting
- **Code quality gates** using ReSharper code inspection
- **NuGet package generation** for library distribution
- **Multi-platform executables** (Windows, Linux, macOS) as self-contained single-file apps

## Git Branching Strategy

This project uses a **single main branch** strategy, also known as trunk-based development. This approach prioritizes simplicity and continuous integration.

### How It Works

```
main ─────●─────●─────●─────●─────●─────●─────●─────●
          │     │     │     │           │     │
          │     │     │     │           │     └── v1.0.2 (tag)
          │     │     │     │           └── v1.0.1 (tag)
          │     │     │     └── feature commit
          │     │     └── bug fix commit
          │     └── v1.0.0 (tag)
          └── initial commit
```

### Core Principles

| Principle | Description |
|-----------|-------------|
| **Single branch** | All development happens on `main` |
| **Small commits** | Frequent, small, well-tested changes |
| **Tag-based releases** | Version tags (`v1.0.0`) trigger packaging |
| **Always releasable** | Main branch is always in a deployable state |

### Development Workflow

1. **Make changes locally**
   ```bash
   # Make your changes
   git add .
   git commit -m "Add new feature X"
   ```

2. **Run quality checks before pushing**
   ```bash
   # Build and test
   dotnet build
   dotnet test

   # Run code inspection
   jb inspectcode YourProject.sln --output=inspectcode-results.xml
   ```

3. **Push to main**
   ```bash
   git push origin main
   ```

4. **CI automatically validates**
   - Build triggers on push to main
   - Tests run automatically
   - Code inspection enforces quality standards

### Release Workflow

When ready to release:

1. **Update version** in `Directory.Build.props`:
   ```xml
   <VersionPrefix>1.2.0</VersionPrefix>
   ```

2. **Commit the version change**:
   ```bash
   git add Directory.Build.props
   git commit -m "Bump version to 1.2.0"
   git push origin main
   ```

3. **Create and push a version tag**:
   ```bash
   git tag v1.2.0
   git push origin v1.2.0
   ```

4. **Pipeline automatically**:
   - Runs the build job (validates the tag commit)
   - Runs the package job (creates NuGet package and platform binaries)
   - Publishes artifacts

### Why Single Main Branch?

| Advantage | Explanation |
|-----------|-------------|
| **Simplicity** | No branch management overhead, no merge conflicts between long-lived branches |
| **Continuous Integration** | Every commit is integrated immediately, issues caught early |
| **Always Releasable** | Main is always in a known-good state |
| **Clear History** | Linear history makes debugging and bisecting easier |
| **Faster Feedback** | Changes are validated by CI within minutes |

### When to Use Feature Branches

While the default is direct commits to main, short-lived feature branches are acceptable for:

- **Multi-day work** that cannot be broken into smaller commits
- **Experimental changes** that need isolated testing
- **Collaborative work** where multiple developers need to share code before it's ready

If using feature branches:

```bash
# Create short-lived branch
git checkout -b feature/short-description
# ... make changes ...
git push origin feature/short-description

# After CI passes and review complete, merge to main
git checkout main
git merge feature/short-description
git push origin main

# Delete the branch
git branch -d feature/short-description
git push origin --delete feature/short-description
```

### Branch Protection (Optional)

For teams, consider enabling branch protection on main:

- Require pull request reviews before merging
- Require status checks to pass (build must succeed)
- Require linear history (no merge commits)

This adds a review step while maintaining the single-branch philosophy.

### Comparison with Other Strategies

| Strategy | Branches | Complexity | Best For |
|----------|----------|------------|----------|
| **Single Main (this guide)** | 1 | Low | Small teams, libraries, rapid iteration |
| GitFlow | 5+ | High | Large teams, scheduled releases |
| GitHub Flow | 2+ | Medium | Web apps with continuous deployment |

## Project Structure

Organize your CI configuration files in a dedicated directory:

```
your-project/
├── ci/
│   ├── azure-pipelines.yml    # Main pipeline definition
│   ├── build.yml              # Build and test template
│   └── package.yml            # Packaging template
├── src/
│   ├── YourLibrary/           # Core library project
│   └── YourConsoleApp/        # Console application
├── test/
│   └── YourLibrary.Tests/     # Unit tests
├── Directory.Build.props       # Centralized build properties
└── YourProject.sln
```

## Pipeline Configuration

### Main Pipeline File (`ci/azure-pipelines.yml`)

This is the entry point that defines triggers and orchestrates jobs:

```yaml
# Trigger on main branch pushes and version tags
trigger:
  branches:
    include:
    - main
  tags:
    include:
    - v*

pool:
  vmImage: 'windows-latest'

variables:
  solution: '**/*.sln'
  buildPlatform: 'Any CPU'
  buildConfiguration: 'Release'

jobs:
  # Build job runs on every push
  - job: build
    pool:
      vmImage: 'windows-latest'
    steps:
      - checkout: self
      - template: build.yml

  # Package job only runs on version tags
  - job: package
    condition: and(succeeded(), startsWith(variables['Build.SourceBranch'], 'refs/tags/v'))
    pool:
      vmImage: 'windows-latest'
    dependsOn:
      build
    steps:
      - checkout: self
        persistCredentials: true
        clean: true
      - template: package.yml
```

### Key Features

| Feature | Implementation |
|---------|---------------|
| Branch triggers | `trigger.branches.include: [main]` |
| Tag-based releases | `trigger.tags.include: [v*]` |
| Conditional packaging | `condition: and(succeeded(), startsWith(...))` |
| Job dependencies | `dependsOn: build` |
| Template reuse | `template: build.yml` |

## Build Template

### Build and Test (`ci/build.yml`)

```yaml
steps:
  # Install the required .NET SDK version
  - task: UseDotNet@2
    displayName: 'Install .NET SDK'
    inputs:
      packageType: 'sdk'
      version: '8.x'  # Specify your target SDK version

  # Build all projects
  - task: DotNetCoreCLI@2
    displayName: 'dotnet build'
    inputs:
      command: 'build'
      projects: '**/*.csproj'
      arguments: '--configuration $(buildConfiguration)'

  # Run tests
  - task: DotNetCoreCLI@2
    displayName: 'dotnet test'
    inputs:
      command: 'test'
      nobuild: true
      projects: '**/*Tests/*.csproj'
      arguments: '--configuration $(buildConfiguration)'

  # Install ReSharper command-line tools
  - task: CmdLine@2
    displayName: 'Install ReSharper Tools'
    inputs:
      script: 'dotnet tool install -g JetBrains.ReSharper.GlobalTools'

  # Run code inspection
  - task: CmdLine@2
    displayName: 'Perform code inspection'
    inputs:
      script: 'jb inspectcode YourProject.sln -o=$(Build.ArtifactStagingDirectory)/Resharper.sarif'

  # Publish inspection results as artifact
  - task: PublishPipelineArtifact@1
    inputs:
      targetPath: '$(Build.ArtifactStagingDirectory)'
      artifactName: CodeAnalysisLogs

  # Fail build if code inspection finds issues
  - task: PowerShell@2
    displayName: 'Check inspection results and fail if needed'
    inputs:
      targetType: 'inline'
      script: |
        $sarifPath = "$(Build.ArtifactStagingDirectory)/Resharper.sarif"
        $sarif = Get-Content $sarifPath | ConvertFrom-Json

        $errorCount = 0
        $warningCount = 0

        foreach ($run in $sarif.runs) {
            if ($run.results) {
                foreach ($result in $run.results) {
                    if ($result.level -eq "error") {
                        $errorCount++
                    } elseif ($result.level -eq "warning") {
                        $warningCount++
                    }
                }
            }
        }

        Write-Host "Found $errorCount errors and $warningCount warnings"

        # Fail on both errors and warnings
        if ($errorCount -gt 0 -or $warningCount -gt 0) {
            Write-Host "##vso[task.logissue type=error]Code inspection found $errorCount errors and $warningCount warnings"
            exit 1
        }
```

## Package Template

### NuGet and Multi-Platform Publishing (`ci/package.yml`)

```yaml
steps:
  - task: UseDotNet@2
    displayName: 'Install .NET SDK'
    inputs:
      packageType: 'sdk'
      version: '8.x'

  # Create NuGet package
  - task: DotNetCoreCLI@2
    displayName: 'dotnet pack'
    inputs:
      command: 'pack'
      arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
      packagesToPack: 'src/YourLibrary/YourLibrary.csproj'

  # Publish for Windows x64
  - task: DotNetCoreCLI@2
    displayName: 'dotnet publish for win-x64'
    inputs:
      command: 'publish'
      publishWebProjects: false
      projects: 'src/YourConsoleApp/YourConsoleApp.csproj'
      arguments: >-
        -r win-x64
        --configuration $(BuildConfiguration)
        --self-contained true
        -p:PublishSingleFile=true
        -p:IncludeNativeLibrariesForSelfExtract=true
        -p:EnableCompressionInSingleFile=true
        --output $(Build.ArtifactStagingDirectory)/YourConsoleApp/win-x64
      zipAfterPublish: false
      modifyOutputPath: false

  # Publish for Linux x64
  - task: DotNetCoreCLI@2
    displayName: 'dotnet publish for linux-x64'
    inputs:
      command: 'publish'
      publishWebProjects: false
      projects: 'src/YourConsoleApp/YourConsoleApp.csproj'
      arguments: >-
        -r linux-x64
        --configuration $(BuildConfiguration)
        --self-contained true
        -p:PublishSingleFile=true
        -p:IncludeNativeLibrariesForSelfExtract=true
        -p:EnableCompressionInSingleFile=true
        --output $(Build.ArtifactStagingDirectory)/YourConsoleApp/linux-x64
      zipAfterPublish: false
      modifyOutputPath: false

  # Publish for Linux ARM64 (Raspberry Pi, etc.)
  - task: DotNetCoreCLI@2
    displayName: 'dotnet publish for linux-arm64'
    inputs:
      command: 'publish'
      publishWebProjects: false
      projects: 'src/YourConsoleApp/YourConsoleApp.csproj'
      arguments: >-
        -r linux-arm64
        --configuration $(BuildConfiguration)
        --self-contained true
        -p:PublishSingleFile=true
        -p:IncludeNativeLibrariesForSelfExtract=true
        -p:EnableCompressionInSingleFile=true
        --output $(Build.ArtifactStagingDirectory)/YourConsoleApp/linux-arm64
      zipAfterPublish: false
      modifyOutputPath: false

  # Publish for macOS ARM64 (Apple Silicon)
  - task: DotNetCoreCLI@2
    displayName: 'dotnet publish for osx-arm64'
    inputs:
      command: 'publish'
      publishWebProjects: false
      projects: 'src/YourConsoleApp/YourConsoleApp.csproj'
      arguments: >-
        -r osx-arm64
        --configuration $(BuildConfiguration)
        --self-contained true
        -p:PublishSingleFile=true
        -p:IncludeNativeLibrariesForSelfExtract=true
        -p:EnableCompressionInSingleFile=true
        --output $(Build.ArtifactStagingDirectory)/YourConsoleApp/osx-arm64
      zipAfterPublish: false
      modifyOutputPath: false

  # Publish all artifacts
  - task: PublishPipelineArtifact@1
    inputs:
      targetPath: '$(Build.ArtifactStagingDirectory)'
```

### Common Runtime Identifiers

| Platform | RID |
|----------|-----|
| Windows x64 | `win-x64` |
| Windows ARM64 | `win-arm64` |
| Linux x64 | `linux-x64` |
| Linux ARM64 | `linux-arm64` |
| macOS x64 (Intel) | `osx-x64` |
| macOS ARM64 (Apple Silicon) | `osx-arm64` |

## Centralized Project Configuration

### Directory.Build.props

Place this file in your repository root to share properties across all projects:

```xml
<Project>
  <PropertyGroup>
    <!-- Version management - single source of truth -->
    <VersionPrefix>1.0.0</VersionPrefix>

    <!-- Package metadata -->
    <Authors>Your Name</Authors>
    <Company>Your Company</Company>
    <Copyright>Copyright © $(Company) $([System.DateTime]::Now.Year)</Copyright>
    <Product>Your Product</Product>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
    <PackageProjectUrl>https://github.com/yourorg/yourproject</PackageProjectUrl>
    <PackageTags>tag1;tag2;tag3</PackageTags>
    <PackageReleaseNotes>See https://github.com/yourorg/yourproject/releases</PackageReleaseNotes>

    <!-- Repository information -->
    <RepositoryUrl>https://github.com/yourorg/yourproject.git</RepositoryUrl>
    <RepositoryType>git</RepositoryType>
    <PublishRepositoryUrl>true</PublishRepositoryUrl>
    <EmbedUntrackedSources>true</EmbedUntrackedSources>

    <!-- Enable Source Link for debugging NuGet packages -->
    <IncludeSymbols>true</IncludeSymbols>
    <SymbolPackageFormat>snupkg</SymbolPackageFormat>

    <!-- CI build detection -->
    <ContinuousIntegrationBuild Condition="'$(TF_BUILD)' == 'true'">true</ContinuousIntegrationBuild>
  </PropertyGroup>

  <!-- Source Link for GitHub -->
  <ItemGroup>
    <PackageReference Include="Microsoft.SourceLink.GitHub" Version="8.0.0" PrivateAssets="All" />
  </ItemGroup>
</Project>
```

### Benefits of Centralized Configuration

1. **Single version source**: Update `VersionPrefix` in one place
2. **Consistent metadata**: All packages share author, license, and URL information
3. **Automatic CI detection**: `TF_BUILD` variable enables deterministic builds
4. **Source Link support**: Enables debugging into NuGet package source code

## Code Quality Enforcement

### ReSharper Code Inspection

The pipeline uses JetBrains ReSharper command-line tools to enforce code quality:

1. **Installation**: `dotnet tool install -g JetBrains.ReSharper.GlobalTools`
2. **Execution**: `jb inspectcode YourProject.sln -o=output.sarif`
3. **SARIF output**: Standard format for code analysis results
4. **Build failure**: Pipeline fails if errors or warnings are found

### Local Development Workflow

Before committing, run code inspection locally:

```powershell
# Install ReSharper tools (one-time)
dotnet tool install -g JetBrains.ReSharper.GlobalTools

# Run inspection
jb inspectcode YourProject.sln --output=inspectcode-results.xml

# Check for issues
$json = Get-Content inspectcode-results.xml -Raw | ConvertFrom-Json
$issues = $json.runs[0].results | Where-Object { $_.level -eq 'error' -or $_.level -eq 'warning' }
foreach ($issue in $issues) {
    Write-Host "$($issue.level.ToUpper()): $($issue.ruleId)" -ForegroundColor $(if($issue.level -eq 'error'){'Red'}else{'Yellow'})
    Write-Host "  Message: $($issue.message.text)"
    Write-Host "  File: $($issue.locations[0].physicalLocation.artifactLocation.uri)"
    Write-Host "  Line: $($issue.locations[0].physicalLocation.region.startLine)"
    Write-Host ""
}
```

## Multi-Platform Publishing

### Single-File Self-Contained Applications

The publishing configuration creates standalone executables:

| Option | Purpose |
|--------|---------|
| `--self-contained true` | Include .NET runtime (no installation required) |
| `-p:PublishSingleFile=true` | Bundle into single executable |
| `-p:IncludeNativeLibrariesForSelfExtract=true` | Include native libraries in bundle |
| `-p:EnableCompressionInSingleFile=true` | Compress for smaller file size |

### Adding New Platform Targets

To add a new platform, copy an existing publish task and update:

1. `displayName` - Human-readable name
2. `-r` argument - Runtime identifier
3. `--output` path - Include platform in path

## Setting Up Azure DevOps

### Initial Setup

1. **Create a new pipeline**:
   - Go to Pipelines > New Pipeline
   - Select your repository source (GitHub, Azure Repos, etc.)
   - Choose "Existing Azure Pipelines YAML file"
   - Select `ci/azure-pipelines.yml`

2. **Configure permissions**:
   - Ensure the pipeline has access to your repository
   - For NuGet publishing, add service connections

3. **Set up branch policies** (optional but recommended):
   - Require build validation before PR merge
   - Require code review approval

### Release Workflow

1. Update version in `Directory.Build.props`:
   ```xml
   <VersionPrefix>1.2.0</VersionPrefix>
   ```

2. Commit and push to main

3. Create a version tag:
   ```bash
   git tag v1.2.0
   git push origin v1.2.0
   ```

4. The pipeline automatically:
   - Runs the build job
   - On success, runs the package job
   - Publishes NuGet packages and platform binaries as artifacts

### Downloading Artifacts

After a successful release build:

1. Go to Pipelines > Runs
2. Select the tagged build
3. Click on the published artifacts
4. Download NuGet packages or platform-specific executables

## Best Practices

### Version Management

- Use semantic versioning (MAJOR.MINOR.PATCH)
- Keep version in `Directory.Build.props` for single source of truth
- Use version tags (`v1.2.3`) to trigger releases

### Code Quality

- Fix all ReSharper warnings before committing
- Run code inspection locally before pushing
- Configure `.editorconfig` for consistent formatting

### Pipeline Efficiency

- Use template files to avoid duplication
- Leverage job dependencies to fail fast
- Use conditional execution for release-only tasks

### Security

- Never store secrets in pipeline YAML files
- Use Azure DevOps variable groups for sensitive data
- Enable branch protection on main branch

### Artifact Management

- Keep artifact names descriptive
- Include platform/architecture in output paths
- Consider artifact retention policies for cost management

## Extending the Pipeline

### Adding Code Coverage

```yaml
- task: DotNetCoreCLI@2
  displayName: 'dotnet test with coverage'
  inputs:
    command: 'test'
    projects: '**/*Tests/*.csproj'
    arguments: '--configuration $(buildConfiguration) --collect:"XPlat Code Coverage"'

- task: PublishCodeCoverageResults@1
  inputs:
    codeCoverageTool: 'Cobertura'
    summaryFileLocation: '$(Agent.TempDirectory)/**/coverage.cobertura.xml'
```

### Adding NuGet Publishing

```yaml
- task: NuGetCommand@2
  displayName: 'Push to NuGet.org'
  inputs:
    command: 'push'
    packagesToPush: '$(Build.ArtifactStagingDirectory)/**/*.nupkg'
    nuGetFeedType: 'external'
    publishFeedCredentials: 'NuGet.org'  # Service connection name
```

### Adding GitHub Release

```yaml
- task: GitHubRelease@1
  displayName: 'Create GitHub Release'
  inputs:
    gitHubConnection: 'GitHub'  # Service connection name
    repositoryName: '$(Build.Repository.Name)'
    action: 'create'
    target: '$(Build.SourceVersion)'
    tagSource: 'gitTag'
    assets: '$(Build.ArtifactStagingDirectory)/**/*'
```

## Troubleshooting

### Common Issues

| Issue | Solution |
|-------|----------|
| Build fails on SDK version | Update `UseDotNet@2` version input |
| ReSharper not found | Ensure `dotnet tool install` step runs first |
| Test projects not found | Check `projects` glob pattern matches your structure |
| Artifacts not published | Verify `targetPath` points to correct directory |

### Debugging Pipeline Failures

1. Check the failed step's logs
2. Add `System.Debug: true` variable for verbose logging
3. Use `script` tasks with `echo` commands to inspect variables
4. Run the same commands locally to reproduce issues
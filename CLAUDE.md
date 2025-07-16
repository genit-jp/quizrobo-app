# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity-based mobile quiz application for Japanese children called "QuizRobo" (キッズクイズ). The app targets elementary school students with educational quizzes across various subjects and difficulty levels.

## Development Commands

### Build Commands
- **Development Build**: `sh scripts/build_for_debug.sh` - Creates development build with S3 distribution
- **iOS Development**: `sh scripts/build_ios_dev.sh` - Unity iOS build for development (Bundle ID: jp.genit.kidsquiz.dev)
- **iOS Production**: `sh scripts/build_ios_prod.sh` - Unity iOS build for production (Bundle ID: jp.genit.kidsquiz)

### Unity Build Methods
- `Builder.BuildDev()` - Development build with debug bundle identifier
- `Builder.BuildProd()` - Production build with release bundle identifier

### Distribution
- Development builds are automatically uploaded to S3 for internal testing
- Production builds require manual App Store submission

## Architecture Overview

### Scene Structure
- **TitleScene**: Firebase initialization and authentication
- **SelectScene**: Quiz selection and user interface (main hub)
- **GameScene**: Quiz gameplay (currently disabled in codebase)
- **LaboScene**: Character customization features

### Key Singleton Managers
- `MasterData`: Centralized game data management with remote JSON fetching
- `UserDataManager`: User data persistence with Firebase Firestore integration
- `FirebaseManager`: Firebase services (Auth, Firestore, Remote Config, Messaging)
- `AdManager`: Google Mobile Ads integration
- `Clock`: Time management utilities

### Data Flow
1. Remote master data fetched from Google Apps Script JSON endpoint
2. User data stored in Firebase Firestore with real-time synchronization
3. Local caching for offline functionality and performance

## Firebase Integration

### Services Used
- **Authentication**: Anonymous login system
- **Firestore**: Real-time user data storage
- **Remote Config**: Feature flags and configuration
- **Messaging**: Push notifications (configured but not actively used)

### User Data Structure
```
users/{userId}/
├── playerName, grade, totalMedal, stage, rating
├── lastLoginDateTime, consecutiveLoginNum
└── answers/{answerId} - Individual quiz performance data
```

## External Services

### Advertisement (Google Mobile Ads)
- Adaptive banner ads on SelectScene
- Interstitial ads after game completion
- Revenue attribution via Adjust SDK

### Analytics & Attribution
- **Adjust SDK**: User acquisition tracking
- **Event Types**: FirstPlay, Login, StartGame, GetReward
- **iOS App Tracking Transparency**: Privacy compliance

## Code Organization

### Scripts Structure
- `Assets/Scripts/Models/`: Data models (QuizData, ChapterData, MasterData)
- `Assets/Scripts/Common/`: Shared utilities and managers
- `Assets/Scripts/Game/`: Quiz gameplay logic (mostly disabled)
- `Assets/Scripts/Select/`: Quiz selection and main menu
- `Assets/Scripts/Utils/`: Helper functions and builders

### UI Architecture
- **DialogBase**: Base class for all modal dialogs with animation
- **Component Pattern**: Reusable UI components (buttons, panels, content)
- **Prefab System**: Modular UI loaded via Resources.Load

## Development Notes

### Unity Version
- Unity 2021.3.37f1 (LTS)
- Target platforms: iOS primary, Android secondary

### Language & Localization
- Primary language: Japanese
- Mixed Japanese/English in code comments and variable names
- Text assets stored in Resources for easy localization

### Quiz System Status
- Core quiz gameplay is currently disabled (commented out)
- Quiz selection algorithm exists but not active
- Focus is on SelectScene UI and user management

### Key Dependencies
- UniTask for async operations
- DOTween for animations
- Firebase Unity SDK
- Google Mobile Ads SDK
- Adjust SDK for attribution

## Common Development Tasks

### Adding New Quiz Content
1. Update master data JSON endpoint
2. Modify QuizData model if needed
3. Test with MasterData.FetchMasterData()

### UI Dialog Development
1. Extend DialogBase for new modals
2. Create prefab in Resources/Prefabs/
3. Use Utils.OpenDialog() for instantiation

### Firebase Data Changes
1. Update UserDataManager methods
2. Test with Firebase offline persistence
3. Verify real-time listeners work correctly

### Build Configuration
1. Development builds use jp.genit.kidsquiz.dev bundle ID
2. Production builds use jp.genit.kidsquiz bundle ID
3. Both builds target iOS App Store distribution

## Testing

- No automated test framework is currently configured
- Manual testing required for all changes
- Firebase integration requires real device testing
- Ad integration testing needs Google AdMob test units
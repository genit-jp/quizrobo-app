fastlane documentation
----

# Installation

Make sure you have the latest version of the Xcode command line tools installed:

```sh
xcode-select --install
```

For _fastlane_ installation instructions, see [Installing _fastlane_](https://docs.fastlane.tools/#installing-fastlane)

# Available Actions

## iOS

### ios upload_binary

```sh
[bundle exec] fastlane ios upload_binary
```

Upload IPA to App Store Connect (TestFlight / Review)

### ios upload_metadata

```sh
[bundle exec] fastlane ios upload_metadata
```

Upload metadata & screenshots from fastlane/metadata/ios

### ios upload_iap_metadata

```sh
[bundle exec] fastlane ios upload_iap_metadata
```

Upload in-app purchase metadata

----


## Android

### android upload_binary

```sh
[bundle exec] fastlane android upload_binary
```



### android upload_metadata

```sh
[bundle exec] fastlane android upload_metadata
```

Upload metadata & screenshots from fastlane/metadata/android

### android upload_iap_metadata

```sh
[bundle exec] fastlane android upload_iap_metadata
```

Upload in-app purchase metadata

----

This README.md is auto-generated and will be re-generated every time [_fastlane_](https://fastlane.tools) is run.

More information about _fastlane_ can be found on [fastlane.tools](https://fastlane.tools).

The documentation of _fastlane_ can be found on [docs.fastlane.tools](https://docs.fastlane.tools).

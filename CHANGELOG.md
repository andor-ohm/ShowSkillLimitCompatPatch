# Changelog

## Version 1.0.2

- Updated the compatibility hook for Show Skill Limit 1.0.4 and its current `ShowSkillLimit.SkillLimitPatch` implementation
- Fixed the recurring `ShowSkillLimit: error` message spam in Character Reload's custom character screen
- Verified both mods and the compatibility patch load together without related errors or exceptions
- Left module dependencies unversioned so compatible Bannerlord and dependency updates are not blocked by the launcher
- This release targets the current Show Skill Limit generation only; use an earlier compatibility-patch release with older Show Skill Limit versions

## Version 1.0.1

- Added configurable build/deploy support for non-default Bannerlord install paths
- Added an MCM toggle for debug messages, disabled by default
- Switched debug output from file logging to in-game message notifications
- Shortened the compatibility notification text and refined when it triggers

## Version 1.0.0

- Initial release
- Fixed `ShowSkillLimit: error` spam on Character Reload custom character screens
- Added the initial Show Skill Limit compatibility guard patch

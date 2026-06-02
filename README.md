# PrettyLog

[![openupm](https://img.shields.io/npm/v/com.acproject.prettylog?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.acproject.prettylog/)

PrettyLog is a small, editor-focused logging helper for Unity that formats Unity Console output with colored, sized and optionally bold tags to make debugging information more readable.

Key features
- Colored channel tags and optional sub-channel tags
- Channel and sub-channel registration APIs
- Per-channel and per-subchannel muting
- Convenience static facades for quick colored logs (`PrettyQuickLog`) and structured channel-based logs (`PrettyLog`)

Install

- OpenUPM registry:

	```bash
	openupm add com.acproject.prettylog
	```

- Or add via Git (Unity Package Manager) using the repository URL or local package path.

Quick examples

- Quick colored log (editor only):

	```csharp
	using UnityEngine;
	PrettyQuickLog.Log("Hello world", Color.cyan, isBold: true);
	PrettyQuickLog.LogWarning("Be careful", Color.yellow);
	PrettyQuickLog.LogError("Oops", Color.red, isBold: true);
	```

- Channel-based logging (recommended for organized projects):

	```csharp
	// Register a channel (recommended at startup)
	PrettyLog.RegisterChannel("Gameplay", Color.green);

	// Register a sub-channel with custom styling
	PrettyLog.RegisterSubChannel("Gameplay", "Input", "#7DBA84", isBold: true, fontSize: 14);

	// Write logs
	PrettyLog.Log("Gameplay", "Player moved");
	PrettyLog.Log("Gameplay", "Input", "Button pressed");
	PrettyLog.LogWarning("Gameplay", "Physics", "Suspicious velocity");
	PrettyLog.LogError("Gameplay", "Networking", "Lost connection");
	```

Muting channels and sub-channels

```csharp
PrettyLog.SetChannelMute("Gameplay", true);
PrettyLog.SetSubChannelMute("Gameplay", "Input", true);
```

Color helpers

```csharp
// Try parse hex -> Color (falls back to white on invalid input)
Color c = PrettyLog.TryParseHexOrDefault("#7DBA84", Color.white);
```

Contributing

Contributions, issues and suggestions are welcome — please open a PR or an issue.

License

This project is distributed under the terms of the included `LICENSE` file.

Changelog

See `CHANGELOG.md` for details.

This README was generated 
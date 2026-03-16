# RPG Dungeon .NET 10.0 Upgrade Tasks

## Overview

This document tracks the execution of the RPG Dungeon console application upgrade from .NET Framework 4.0 Client Profile to .NET 10.0. The single project will be converted to SDK-style format and upgraded to the target framework in one atomic operation.

**Progress**: 2/2 tasks complete (100%) ![0%](https://progress-bar.xyz/100)

---

## Tasks

### [✓] TASK-001: Verify prerequisites *(Completed: 2026-03-15 01:44)*
**References**: Plan §Prerequisites

- [✓] (1) Verify .NET 10.0 SDK installed per Plan §Prerequisites
- [✓] (2) SDK version meets minimum requirements (**Verify**)

---

### [✓] TASK-002: Atomic project conversion and framework upgrade *(Completed: 2026-03-15 01:46)*
**References**: Plan §SDK-Style Project Conversion, Plan §Target Framework Update, Plan §Breaking Changes Catalog

- [✓] (1) Convert Night.csproj from classic format to SDK-style format per Plan §SDK-Style Project Conversion (use automated conversion tool or manual approach)
- [✓] (2) Project file converted to SDK-style with `Sdk="Microsoft.NET.Sdk"` (**Verify**)
- [✓] (3) Update TargetFramework property to net10.0 in Night.csproj
- [✓] (4) Target framework updated to net10.0 (**Verify**)
- [✓] (5) Restore dependencies (none expected - no NuGet packages)
- [✓] (6) Dependencies restored successfully (**Verify**)
- [✓] (7) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus on file I/O paths, serialization if present, console encoding)
- [✓] (8) Solution builds with 0 errors (**Verify**)
- [✓] (9) Commit all changes with message: "TASK-002: Complete upgrade to .NET 10.0 - SDK conversion and framework update"

---




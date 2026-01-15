Shoots.Runtime

Shoots.Runtime is a sealed, versioned runtime engine written in .NET 8.

It is designed to execute structured commands deterministically, with a hard runtime boundary and zero ambient behavior. Once complete, this runtime is intended to be foundational and not modified by downstream tooling, Codex, or AI-assisted refactors.

If you are looking for extensibility without chaos, or plugins without coupling, this is it.

Design goals

Deterministic command execution

Sealed runtime boundary

Contract-only abstractions

Explicit versioning and compatibility rules

Safe external module loading

No hidden dispatch

No reflection soup

No service locator abuse

No partial state

No surprises

This runtime favors clarity over convenience and correctness over cleverness.

Project structure

Shoots.Runtime.Abstractions

Contract-only

Interfaces, records, value types

No logic

No helpers

No parsing

No execution

Shoots.Runtime.Core

Runtime engine

Command registry

Execution semantics

Boundary enforcement

Shoots.Runtime.Loader

External module discovery and validation

Version gating

Safe instantiation

Shoots.Runtime.Sandbox

Test host for exercising the runtime

Used during development only

Runtime invariants (non-negotiable)

These rules are enforced by design and must not be violated:

All command execution flows through RuntimeEngine.Execute

Modules never call other modules

Modules never dispatch commands

Modules are pure handlers

No global state

No async sprawl

No dynamic dispatch hacks

Breaking these rules breaks the runtime.

Module contract

Modules implement IRuntimeModule and expose handlers that obey the runtime contract.

Handler input

RuntimeRequest

RuntimeContext

IRuntimeServices

Handler output

Handlers must return a RuntimeResult.

RuntimeResult.Output may be only:

null

an IRuntimeValue

a primitive (string, bool, number)

Forbidden:

anonymous objects

dictionaries

dynamic values

This restriction is intentional. Structured output is mandatory.

External module loading (Step 9)

Shoots.Runtime supports loading external modules from assemblies without coupling.

How loading works

A directory is scanned for *.dll

Each assembly must declare a module manifest

Manifest metadata is read (no type scanning)

Version compatibility is validated

The module type is validated

The module is instantiated

Incompatible or invalid modules are rejected safely

There is no partial state. A module either loads completely or not at all.

Failures are never fatal to the runtime host.

Module manifest

External module assemblies must declare a manifest at the assembly level.

Example:

using Shoots.Runtime.Loader;

[assembly: RuntimeModuleManifest(
    moduleId: "example.echo",
    moduleType: typeof(EchoModule),
    minMajor: 1,
    minMinor: 0,
    maxMajor: 1,
    maxMinor: 9)]


If an assembly does not declare a manifest, it is ignored.

This avoids reflection scanning, ambiguity, and accidental activation.

Version gating

Each module declares:

Minimum supported runtime version

Maximum supported runtime version

A module is rejected if the host runtime version falls outside this range.

This prevents silent breakage when the runtime evolves.

Loader behavior

The loader is intentionally strict and boring:

Bad assemblies are ignored

Missing manifests are ignored

Incompatible versions are ignored

Instantiation failures are ignored

The runtime always continues in a valid state.

Status

Steps 1–8: complete and sealed

Step 9: external module loading implemented

Runtime builds cleanly

Sandbox runs

Abstractions are frozen

Execution semantics enforced

Once finalized, Shoots.Runtime is not meant to be modified.

Build tooling lives above it, not inside it.

Philosophy (short version)

This runtime exists so higher-level systems can be aggressive, experimental, and fast without contaminating the core.

The runtime stays boring so everything else can afford not to be.

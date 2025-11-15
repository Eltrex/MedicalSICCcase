using SPTarkov.Server.Core.Models.Spt.Mod;
using System.Collections.Generic;
using SemVerVersion = SemanticVersioning.Version;
using SemVerRange = SemanticVersioning.Range;

namespace MedicalSICCcase;

public record ModMetadata : AbstractModMetadata
{
    public override string ModGuid { get; init; } = "de.eltrex.medicalsiccrevised";
    public override string Name { get; init; } = "MedicalSICCcase";
    public override string Author { get; init; } = "Revingly(original mod), Nanami, Eltrex";
    public override List<string>? Contributors { get; init; } = new() { "Revingly(original mod), Nanami, Eltrex" };
    public override SemVerVersion Version { get; init; } = new SemVerVersion("5.0.2");
    public override SemVerRange SptVersion { get; init; } = new SemVerRange("~4.0.0");
    public override List<string>? Incompatibilities { get; init; } = new();
    public override Dictionary<string, SemVerRange>? ModDependencies { get; init; } = new();
    public override string? Url { get; init; } = null;
    public override bool? IsBundleMod { get; init; } = true;
    public override string? License { get; init; } = "NCSA";
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ILGuard.Obfuscator.PriceSystem.Models;

public sealed class PriceConfiguration
{
    [JsonPropertyName("Line pricing per feature"), JsonPropertyOrder(1)]
    public UInt32 LinePricePerFeature { get; set; } = 10000;

    [JsonPropertyName("Features"), JsonPropertyOrder(2)]
    public FeatureModel[]? Features { get; set; } =
    {
        new FeatureModel(0.50m, ("Identifier Renaming", "Obfucates All Identifiers")),
        new FeatureModel(0.70m, ("Anti-Tamper", "Inserts random runtime checksums")),
        new FeatureModel(0.30m, ("SingularLine", "Trims code into 1 line")),
    };


    [JsonPropertyName("Bundled Features"), JsonPropertyOrder(2)]
    public (decimal Price, (string Name, string Description)[] Features)[]? BundledFeatures { get; set; } =
    {
        (1.00m, new (string Name, string Description)[]
        {
            ("Identifier Renaming", "Obfucates All Identifiers"),
            ("Anti-Tamper", "Inserts random runtime checksums"),
        }),
        (1.20m, new (string Name, string Description)[]
        {
            ("Identifier Renaming", "Obfucates All Identifiers"),
            ("Anti-Tamper", "Inserts random runtime checksums"),
            ("SingularLine", "Trims code into 1 line"),
        }),
    };
}


public sealed record FeatureModel(decimal Price, (string Name, string Description) Feature);



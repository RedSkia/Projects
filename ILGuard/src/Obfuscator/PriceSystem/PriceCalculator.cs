using ILGuard.Obfuscator.PriceSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ILGuard.Obfuscator.PriceSystem;

public static class PriceCalculator
{
    static PriceCalculator()
	{
		var context = File.ReadAllText("./prices.json");
		var config = JsonSerializer.Deserialize<PriceConfiguration>(context);

		Console.WriteLine($"Line Price: {config.LinePricePerFeature}");
		foreach (var feature in config.Features)
		{
			Console.WriteLine($"Name: {feature.Feature.Name}");
			Console.WriteLine($"Description: {feature.Feature.Description}");
			Console.WriteLine($"Price: {feature.Price}");
        }
		Console.WriteLine("---------BUNDLES---------");
        foreach (var bundle in config.BundledFeatures)
        {
            Console.WriteLine($"Price: {bundle.Price}");
            foreach (var feature in bundle.Features)
			{
                Console.WriteLine($"Name: {feature.Name}");
                Console.WriteLine($"Description: {feature.Description}");
            }
        }
    }
}

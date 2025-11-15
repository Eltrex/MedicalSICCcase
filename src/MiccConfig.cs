using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace MedicalSICCcase;

public class MiccConfig
{
    public int Price { get; set; }
    public int Loyalty_lvl { get; set; }
    public int CellH { get; set; }
    public int CellV { get; set; }
    public int ItemWidth { get; set; }
    public int ItemHeight { get; set; }
    public bool AllowMedBarter { get; set; }
    public List<string> Containers { get; set; } = new();

    public static MiccConfig Load()
    {
        var asmDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var path = Path.Combine(asmDir, "config", "config.json");
        if (!File.Exists(path))
        {
            return new MiccConfig
            {
                Price = 1000000,
                Loyalty_lvl = 1,
                CellH = 5,
                CellV = 5,
                ItemWidth = 2,
                ItemHeight = 1,
                AllowMedBarter = true,
                Containers = new List<string>()
            };
        }
        var json = File.ReadAllText(path);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<MiccConfig>(json, options) ?? new MiccConfig();
    }
}

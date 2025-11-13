using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Services.Mod;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicalSICCcaseCS;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class MedicalSiccCaseAfterDbLoad(
    DatabaseServer databaseServer,
    CustomItemService customItemService) : IOnLoad
{
    private readonly MiccConfig _config = MiccConfig.Load();
    private readonly string _itemId = "674f974b8c797c96be0b096c"; // Medical SICC mongo id

    public Task OnLoad()
    {
        // Build minimal clone details (grid + prefab left as original SICC)
        var miccClone = new NewItemFromCloneDetails
        {
            ItemTplToClone = ItemTpl.CONTAINER_SICC,
            ParentId = "5795f317245977243854e041",
            NewId = _itemId,
            FleaPriceRoubles = _config.Price,
            HandbookPriceRoubles = _config.Price,
            HandbookParentId = "5b5f6fa186f77409407a7eb7",
            Locales = new Dictionary<string, LocaleDetails>
            {
                {"en", new LocaleDetails
                    {
                        Name = "Medical SICC case",
                        ShortName = "M I C C",
                        Description = "A SICC case for medical items."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                Name = "Medical SICC",
                ShortName = "M I C C",
                Description = "SICC case for medical items."
            }
        };

        customItemService.CreateItemFromClone(miccClone);
        var items = databaseServer.GetTables().Templates.Items;
        var mid = new MongoId(_itemId);
        if (items.ContainsKey(mid))
        {
            var templateObj = items[mid];

            if (templateObj is TemplateItem item)
            {
                try
                {
                    var grids = item.Properties.Grids.ToList();
                    if (grids.Count > 0)
                    {
                        grids[0].Properties.CellsH = _config.CellH;
                        grids[0].Properties.CellsV = _config.CellV;
                        item.Properties.Grids = grids;

                        System.Console.WriteLine($"[MedicalSICCcaseCS] Medical SICC internal grid set to {_config.CellH}x{_config.CellV}.");
                    }
                    else
                    {
                        System.Console.WriteLine("[MedicalSICCcaseCS] Clone OK but no grids found on item.");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine("[MedicalSICCcaseCS] Clone OK but grid resize failed: " + ex.Message);
                }
            }
        }
        else
        {
            System.Console.WriteLine("[MedicalSICCcaseCS] Medical SICC item clone FAILED.");
        }
        return Task.CompletedTask;
    }

    private static void SetInt(object target, string name, int value)
    {
        var prop = target.GetType().GetProperty(name);
        if (prop != null && prop.CanWrite && prop.PropertyType == typeof(int))
        {
            prop.SetValue(target, value);
            return;
        }
        var field = target.GetType().GetField(name);
        if (field != null && field.FieldType == typeof(int))
        {
            field.SetValue(target, value);
        }
    }
}

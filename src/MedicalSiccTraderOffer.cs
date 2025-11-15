using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalSICCcase;

// Run after item clone creation (which is PostDBModLoader +1); give higher priority
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 5)]
public class MedicalSiccTraderOffer(
    DatabaseService databaseService) : IOnLoad
{
    private static readonly MongoId TherapistId = new("54cb57776803fa99248b456e");
    private static readonly MongoId TemplateId = new("674f974b8c797c96be0b096c");
    private readonly MiccConfig _cfg = MiccConfig.Load();

    public Task OnLoad()
    {
        var tables = databaseService.GetTables();
        if (!tables.Traders.TryGetValue(TherapistId, out var therapist))
            return Task.CompletedTask;

        // Confirm item template exists
        var itemTemplates = tables.Templates.Items;
        if (!itemTemplates.ContainsKey(TemplateId))
            return Task.CompletedTask;

        // Prevent duplicate offer if already present
        if (therapist.Assort.Items.Any(i => i.Template == TemplateId))
            return Task.CompletedTask;

        // Create trader item (similar to reference WeebTraderService pattern)
        var traderItem = new Item
        {
            Id = new MongoId(), // unique offer root id
            Template = TemplateId,
            ParentId = "hideout",
            SlotId = "hideout",
            Upd = new Upd
            {
                UnlimitedCount = true,
                StackObjectsCount = 99999,
            }
        };
        therapist.Assort.Items.Add(traderItem);

        // RUB template id constant
        var cost = new BarterScheme
        {
            Count = _cfg.Price,
            Template = new MongoId("5449016a4bdc2d6f028b456f")
        };

        if (!therapist.Assort.BarterScheme.TryAdd(traderItem.Id, [[cost]]) )
        {
            therapist.Assort.Items.Remove(traderItem);
            return Task.CompletedTask;
        }

        if (!therapist.Assort.LoyalLevelItems.TryAdd(traderItem.Id, _cfg.Loyalty_lvl))
        {
            therapist.Assort.Items.Remove(traderItem);
            therapist.Assort.BarterScheme.Remove(traderItem.Id);
            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }

}

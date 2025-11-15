using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Spt.Server;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Logging;
using SPTarkov.Server.Core.Services.Mod;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPTarkov.Server.Core.Servers;
using JetBrains.Annotations;
using SPTarkov.Server.Core.Models.Utils;

namespace MedicalSICCcase;

[Injectable(TypePriority = OnLoadOrder.PostDBModLoader + 1)]
public class MedicalSiccCaseAfterDbLoad(
    DatabaseServer databaseServer,
    CustomItemService customItemService,
    ISptLogger<MedicalSiccCaseAfterDbLoad> logger) : IOnLoad
{
    private readonly MiccConfig _config = MiccConfig.Load();
    private readonly string _itemId = "674f974b8c797c96be0b096c"; // Medical SICC mongo id

    public Task OnLoad()
    {
        // Clone base SICC
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
                {
                    "en",
                    new LocaleDetails
                    {
                        Name = "Medical SICC case",
                        ShortName = "M I C C",
                        Description = "A SICC case for medical items."
                    }
                }
            },
            OverrideProperties = new TemplateItemProperties
            {
                Name = "item_container_micc",
                ShortName = "item_container_micc",
                Description = "item_container_micc",

                Prefab = new Prefab
                {
                    Path = "assets/content/items/barter/custom_micc/micc_case.bundle",
                }
            }
        };

        customItemService.CreateItemFromClone(miccClone);

        var items = databaseServer.GetTables().Templates.Items;
        var mid = new MongoId(_itemId);

        if (!items.TryGetValue(mid, out var templateObj) || templateObj is not TemplateItem item)
        {
            System.Console.WriteLine("[MedicalSICCcase] Medical SICC item clone FAILED.");
            return Task.CompletedTask;
        }

        try
        {
            // Resize internal grid and restrict to medical items
            var grids = item.Properties.Grids?.ToList() ?? [];
            if (grids.Count > 0)
            {
                grids[0].Properties.CellsH = _config.CellH;
                grids[0].Properties.CellsV = _config.CellV;

                    var existingFilters = grids[0].Properties.Filters;
                    if (existingFilters != null)
                    {
                        var filters = existingFilters.ToList();
                        if (filters.Count > 0)
                        {
                            filters[0].Filter = new HashSet<MongoId>
                            {
                                new("543be5664bdc2dd4348b4569"),
                                new("619cbf7d23893217ec30b689")
                            };

                            if (_config.AllowMedBarter)
                            {
                                filters[0].Filter.Add(new MongoId("57864c8c245977548867e7f1")); // Med Barter
                            }

                            grids[0].Properties.Filters = filters;
                        }
                    }

                item.Properties.Grids = grids;
            }

        if (_config.AllowMedBarter)
            {
            logger.Success(
                $"[MedicalSICCcase] Medical SICC internal grid set to {_config.CellH}x{_config.CellV} with med barter items allowed.");
            }
        else
            {
            logger.Success(
                $"[MedicalSICCcase] Medical SICC internal grid set to {_config.CellH}x{_config.CellV} without med barter items allowed.");
            }
        }
        catch (System.Exception ex)
        {
            logger.Error("[MedicalSICCcase] Clone OK but grid resize failed: " + ex.Message);
        }

        try
        {
            // Hardcode which containers can hold the Medical SICC case
            MongoId[] secureContainerIds =
            [
                new("544a11ac4bdc2d470e8b456a"), // Alpha
                new("5857a8b324597729ab0a0e7d"), // Beta
                new("5857a8bc2459772bad15db29"), // Gamma
                new("59db794186f77448bc595262"), // Epsilon
                new("665ee77ccf2d642e98220bca"), // Gamma Unheard
                new("5c093ca986f7740a1867ab12"), // Kappa
                new("676008db84e242067d0dc4c9"), // Kappa Cult
                new("5732ee6a24597719ae0c0281")  // Waist Pouch
            ];

                var caseId = new MongoId(_itemId);

            foreach (var parentId in secureContainerIds)
            {
                if (!items.TryGetValue(parentId, out var parentObj) || parentObj is not TemplateItem parent)
                {
                    continue;
                }

                var grids = parent.Properties.Grids?.ToList() ?? [];
                if (grids.Count == 0)
                {
                    continue;
                }

                var gridFilters = grids[0].Properties.Filters;
                if (gridFilters == null)
                {
                    continue;
                }

                var filterList = gridFilters.ToList();
                if (filterList.Count == 0)
                {
                    continue;
                }

                var allowed = filterList[0].Filter ?? new HashSet<MongoId>();
                allowed.Add(new MongoId(_itemId)); // allow Medical SICC in this secure container
                filterList[0].Filter = allowed;
                grids[0].Properties.Filters = filterList;

                parent.Properties.Grids = grids;
            }

        }
        catch (System.Exception ex)
        {
            logger.Error("[MedicalSICCcase] Failed to update container slot filters: " + ex.Message);
        }

        // Also allow Medical SICC in extra containers from config
        foreach (var parentId in _config.Containers.Select(id => new MongoId(id)))
        {
            if (!items.TryGetValue(parentId, out var parentObj) || parentObj is not TemplateItem parent)
            {
                continue;
            }

            var parentGrids = parent.Properties.Grids?.ToList() ?? [];
            if (parentGrids.Count == 0)
            {
                continue;
            }

            var gridFilters = parentGrids[0].Properties.Filters;
            if (gridFilters == null)
            {
                continue;
            }

            var filterList = gridFilters.ToList();
            if (filterList.Count == 0)
            {
                continue;
            }

            var allowed = filterList[0].Filter ?? new HashSet<MongoId>();
            allowed.Add(new MongoId(_itemId)); // allow Medical SICC as content
            filterList[0].Filter = allowed;
            parentGrids[0].Properties.Filters = filterList;

            parent.Properties.Grids = parentGrids;
        }

        return Task.CompletedTask;
    }
}

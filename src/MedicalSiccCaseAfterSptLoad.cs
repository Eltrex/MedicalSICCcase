using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using System.Threading.Tasks;

namespace MedicalSICCcase;

[Injectable(TypePriority = OnLoadOrder.PostSptModLoader + 1)]
public class MedicalSiccCaseAfterSptLoad : IOnLoad
{
    public Task OnLoad() => Task.CompletedTask;
}

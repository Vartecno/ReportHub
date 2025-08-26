using ReportHub.Objects.DTOs;

namespace ReportHub.Common.DataHelpers.IDataHelpers;

public interface ILookupHelper
{
    public Task<List<LookupsDTO>> GetLookupByName(string LookupName, bool? IsForSystem = true);
    public Task<List<LookupsDTO>> GetAllLookup(bool IsForSystem);
    public Task<List<LookupsDTO>> GetLookupsByNames(IEnumerable<string> lookupNames, bool? isForSystem = true);
}

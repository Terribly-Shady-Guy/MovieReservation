using DbInfrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure.DataSeeding
{
    internal class EnumSeedingHelper<TEnum, TLookup> : IDataSeedingHelper
        where TEnum : struct, Enum
        where TLookup : EnumLookupBase<TEnum>, new()
    {
        private readonly TLookup[] _lookups;
        private readonly DbContext _context;
        public EnumSeedingHelper(DbContext context)
        {
            _context = context;

           _lookups = Enum.GetValues<TEnum>()
                .Select(e => new TLookup { Id = e, Name = e.ToString()})
                .ToArray();
        }

        public void Add()
        {
            var storedLookups = _context.Set<TLookup>()
                   .ToList();

           AddEnums(storedLookups);
        }

        public async Task AddAsync(CancellationToken cancellationToken)
        {
            var storedLookups = await _context.Set<TLookup>()
                  .ToListAsync(cancellationToken);

            AddEnums(storedLookups);
        }

        private void AddEnums(List<TLookup> storedLookups)
        {
            foreach (var lookup in _lookups)
            {
                if (!storedLookups.Any(s => s.Id.Equals(lookup.Id)))
                {
                    _context.Add(lookup);
                }
            }
        }
    }
}

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
            var statuses = _context.Set<TLookup>()
                   .ToList();

            foreach (var lookup in _lookups)
            {
                if (!statuses.Any(s => s.Name == lookup.Name))
                {
                    _context.Add(lookup);
                }
            }
        }

        public async Task AddAsync(CancellationToken cancellationToken)
        {
            var statuses = await _context.Set<TLookup>()
                  .ToListAsync(cancellationToken);

            foreach (var lookup in _lookups)
            {
                if (!statuses.Any(s => s.Name == lookup.Name))
                {
                    await _context.AddAsync(lookup, cancellationToken);
                }
            }
        }
    }
}

using DbInfrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace DbInfrastructure.DataSeeding
{
    internal class EnumDataSeeder<TEnum, TLookup> : IDataSeeder
        where TEnum : struct, Enum
        where TLookup : EnumLookupBase<TEnum>, new()
    {
        private readonly TLookup[] _lookups;

        public EnumDataSeeder()
        {
           _lookups = Enum.GetValues<TEnum>()
                .Select(e => new TLookup { Id = e, Name = e.ToString() })
                .ToArray();
        }

        public void Add(DbContext context)
        {
            var storedLookups = context.Set<TLookup>()
                   .ToList();

           AddEnums(storedLookups, context);
        }

        public async Task AddAsync(DbContext context, CancellationToken cancellationToken)
        {
            var storedLookups = await context.Set<TLookup>()
                  .ToListAsync(cancellationToken);

            AddEnums(storedLookups, context);
        }

        private void AddEnums(IList<TLookup> storedLookups, DbContext context)
        {
            foreach (var lookup in _lookups)
            {
                if (!storedLookups.Any(s => s.Id.Equals(lookup.Id)))
                {
                    context.Add(lookup);
                }
            }
        }
    }
}

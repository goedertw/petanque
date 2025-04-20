using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using System.Linq;

namespace Petanque.Services.Services
{
    public class SpelverdelingService : ISpelverdelingService
    {
        private readonly Random _random = new();
        private readonly Id312896PetanqueContext _context;

        public SpelverdelingService(Id312896PetanqueContext context) => _context = context;

        public SpelverdelingResponseContract GetById(int id)
        {
            var entity = _context.Spelverdelings.Find(id);
            return entity is null ? null : MapToContract(entity);
        }

        public IEnumerable<SpelverdelingResponseContract> MaakVerdeling(IEnumerable<AanwezigheidResponseContract> aanwezigheden)
        {
            Console.WriteLine("Aanwezigheden:");
            foreach (var a in aanwezigheden)
            {
                Console.WriteLine($"SpelerVolgnr: {a.SpelerVolgnr}");
            }

            if (aanwezigheden == null || !aanwezigheden.Any())
                return Enumerable.Empty<SpelverdelingResponseContract>();

            const int spellenPerSpeler = 3;
            var spelverdelingen = new List<Spelverdeling>();

            var spelerTelling = new Dictionary<int, int>();

            foreach (var aanwezigheid in aanwezigheden)
            {
                if (!spelerTelling.ContainsKey(aanwezigheid.SpelerVolgnr))
                {
                    spelerTelling[aanwezigheid.SpelerVolgnr] = 0;
                }
            }

            Console.WriteLine("Initiale speler telling:");
            foreach (var kvp in spelerTelling)
            {
                Console.WriteLine($"Speler {kvp.Key} heeft {kvp.Value} spellen gespeeld.");
            }

            int spelId = 1;

            while (spelerTelling.Any(kvp => kvp.Value < spellenPerSpeler))
            {
                Console.WriteLine("Start nieuwe ronde...");

                var beschikbareSpelers = spelerTelling
                    .Where(kvp => kvp.Value < spellenPerSpeler)
                    .OrderBy(_ => _random.Next())
                    .ToList();

                Console.WriteLine("Beschikbare spelers voor deze ronde:");
                foreach (var speler in beschikbareSpelers)
                {
                    Console.WriteLine($"Speler {speler.Key} geselecteerd.");
                }

                if (beschikbareSpelers.Count < 4)
                {
                    Console.WriteLine("Niet genoeg spelers om een team te maken. De ronde stopt.");
                    break; 
                }

                var teamA = beschikbareSpelers.Take(2).ToList();
                var teamB = beschikbareSpelers.Skip(2).Take(2).ToList(); 

                int positie = 1;
                foreach (var speler in teamA)
                {
                    spelverdelingen.Add(new Spelverdeling
                    {
                        SpelId = spelId,
                        Team = "Team A",
                        SpelerPositie = $"P{positie++}",
                        SpelerVolgnr = speler.Key
                    });
                    spelerTelling[speler.Key]++;
                }

                positie = 1;
                foreach (var speler in teamB)
                {
                    spelverdelingen.Add(new Spelverdeling
                    {
                        SpelId = spelId,
                        Team = "Team B",
                        SpelerPositie = $"P{positie++}",
                        SpelerVolgnr = speler.Key
                    });
                    spelerTelling[speler.Key]++;
                }

                spelId++;
            }

            _context.Spelverdelings.AddRange(spelverdelingen);
            _context.SaveChanges();

            var responses = spelverdelingen.Select(MapToContract).ToList();
            return responses;
        }

        private static SpelverdelingResponseContract MapToContract(Spelverdeling entity)
        {
            return new SpelverdelingResponseContract
            {
                SpelverdelingsId = entity.SpelverdelingsId,
                SpelId = entity.SpelId,
                Team = entity.Team,
                SpelerPositie = entity.SpelerPositie,
                SpelerVolgnr = entity.SpelerVolgnr
            };
        }
    }
}

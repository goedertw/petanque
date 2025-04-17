using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;
using static Petanque.Contracts.Responses.SpelverdelingSpellenResponseContract;

namespace Petanque.Services.Services;

public class SpelverdelingService(Id312896PetanqueContext context) : ISpelverdelingService
{
    private readonly Random _random = new();

    public SpelverdelingResponseContract GetById(int id)
    {
        var entity = context.Spelverdelings.Find(id);
        return entity is null ? null : MapToContract(entity);
    }

    private static SpelverdelingResponseContract MapToContract(Spelverdeling entity)
    {
        return new SpelverdelingResponseContract
        {
            SpelverdelingsId = entity.SpelverdelingsId,
            SpelId = entity.SpelId,
            Team = entity.Team,
            SpelerPositie = entity.SpelerPositie,
            SpelerVolgnr = entity.SpelerVolgnr,
        };
    }

    public List<SpelverdelingSpellenResponseContract.SpelVerdelingRonde> GenereerVerdeling(List<string> spelers, int aantalRondes, int aantalSpeelterreinen)
    {
        var rondes = new List<SpelverdelingSpellenResponseContract.SpelVerdelingRonde>();
        var samenspeelTracker = spelers.ToDictionary(s => s, s => new HashSet<string>());

        for (int rondeNr = 1; rondeNr <= aantalRondes; rondeNr++)
        {
            var speelveldVerdelingen = new List<SpelverdelingSpellenResponseContract.SpeelveldVerdeling>();
            var shuffledSpelers = spelers.OrderBy(_ => _random.Next()).ToList();
            int totalSpelers = shuffledSpelers.Count;
            int spelersPerVeld = (int)Math.Floor(totalSpelers / (double)aantalSpeelterreinen);

            
            var fieldAssignments = new List<List<string>>();
            int index = 0;
            for (int i = 0; i < aantalSpeelterreinen; i++)
            {
                int count = spelersPerVeld + (i < totalSpelers % aantalSpeelterreinen ? 1 : 0);
                var subset = shuffledSpelers.Skip(index).Take(count).ToList();
                fieldAssignments.Add(subset);
                index += count;
            }

            for (int veldNr = 1; veldNr <= fieldAssignments.Count; veldNr++)
            {
                var subset = fieldAssignments[veldNr - 1];

                
                var teamSplits = GetTeamSplits(subset)
                    .OrderBy(s => Math.Abs(s.team1.Count - s.team2.Count)) 
                    .ThenBy(s => CalculateTeamScore(s.team1, samenspeelTracker) + CalculateTeamScore(s.team2, samenspeelTracker))
                    .ToList();

                if (!teamSplits.Any()) continue;

                var (bestTeam1, bestTeam2) = teamSplits.First();

                foreach (var speler in bestTeam1)
                    foreach (var teammate in bestTeam1.Where(x => x != speler))
                        samenspeelTracker[speler].Add(teammate);

                foreach (var speler in bestTeam2)
                    foreach (var teammate in bestTeam2.Where(x => x != speler))
                        samenspeelTracker[speler].Add(teammate);

                speelveldVerdelingen.Add(new SpelverdelingSpellenResponseContract.SpeelveldVerdeling
                {
                    SpeelveldNr = veldNr,
                    Team1 = bestTeam1,
                    Team2 = bestTeam2
                });
            }

            rondes.Add(new SpelverdelingSpellenResponseContract.SpelVerdelingRonde
            {
                RondeNr = rondeNr,
                Speelvelden = speelveldVerdelingen
            });
        }

        return rondes;
    }


    private static List<(List<string> team1, List<string> team2)> GetTeamSplits(List<string> spelers)
    {
        var result = new List<(List<string>, List<string>)>();
        int n = spelers.Count;

        if (n < 4) return result;

        for (int i = 1; i < n; i++)
        {
            var team1Combos = Combinations(spelers, i);
            foreach (var t1 in team1Combos)
            {
                var t2 = spelers.Except(t1).ToList();
                if (t1.Count >= 2 && t2.Count >= 2)
                {
                    result.Add((t1, t2));
                }
            }
        }

        return result;
    }

    private static int CalculateTeamScore(List<string> team, Dictionary<string, HashSet<string>> tracker)
    {
        int score = 0;
        foreach (var speler in team)
        {
            score += team.Where(teammate => teammate != speler && tracker[speler].Contains(teammate)).Count();
        }
        return score;
    }

    private static List<List<T>> Combinations<T>(List<T> list, int k)
    {
        var result = new List<List<T>>();

        void Combine(int start, List<T> current)
        {
            if (current.Count == k)
            {
                result.Add(new List<T>(current));
                return;
            }

            for (int i = start; i < list.Count; i++)
            {
                current.Add(list[i]);
                Combine(i + 1, current);
                current.RemoveAt(current.Count - 1);
            }
        }

        Combine(0, new List<T>());
        return result;
    }
}

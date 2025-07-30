using Microsoft.EntityFrameworkCore;

namespace Petanque.Services.Services;
using Petanque.Contracts.Requests;
using Petanque.Contracts.Responses;
using Petanque.Services.Interfaces;
using Petanque.Storage;

public class SeizoensKlassementService(Id312896PetanqueContext context) : ISeizoensKlassementService
{
    public IEnumerable<SeizoensKlassementResponseContract> CreateSeizoensKlassementen(int seizoenId)
    {
        
        var verwijderdeRecords = context.Seizoensklassements
            .Where(sk => sk.SeizoensId == seizoenId)
            .ExecuteDelete();
        // 1. Haal alle spelers op
        var alleSpelers = context.Spelers.ToList();

        // 2. Haal alle dagklassementen voor dit seizoen op
        var dagklassementen = context.Dagklassements
            .Include(d => d.Speeldag)
            .Where(d => d.Speeldag.SeizoensId == seizoenId)
            .ToList();

        // 3. Bereken totalen per speler (met Left Join voor spelers zonder klassementen)
        var seizoensKlassementen = alleSpelers
            .GroupJoin(
                dagklassementen,
                speler => speler.SpelerId,
                dagklassement => dagklassement.SpelerId,
                (speler, dagklassements) => new Seizoensklassement()
                {
                    SpelerId = speler.SpelerId,
                    SeizoensId = seizoenId,
                    Hoofdpunten = dagklassements.Sum(d => d?.Hoofdpunten ?? 0),
                    PlusMinPunten = dagklassements.Sum(d => d?.PlusMinPunten ?? 0)
                })
            .ToList();

        // 4. Verwijder bestaande en voeg nieuwe toe
        using var transaction = context.Database.BeginTransaction();
        try
        {
            context.Seizoensklassements
                .Where(sk => sk.SeizoensId == seizoenId)
                .ExecuteDelete();

            context.AddRange(seizoensKlassementen);
            context.SaveChanges();
            transaction.Commit();

            // 5. Map naar response
            return seizoensKlassementen.Select(s => new SeizoensKlassementResponseContract
            {
                SpelerId = s.SpelerId,
                SeizoensId = s.SeizoensId,
                Hoofdpunten = s.Hoofdpunten,
                PlusMinPunten = s.PlusMinPunten,
            });
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public IEnumerable<SeizoensKlassementResponseContract> GetById(int seizoenId)
    {
        var seizoensklassementen = context.Seizoensklassements
            .Where(sk => sk.SeizoensId == seizoenId)  // Filter op seizoenId in plaats van speeldagId
            // .Include(sk => sk.Speler)  // Optioneel: include spelerinfo als je die nodig hebt
            .ToList();

        return seizoensklassementen
            .Select(MapToContract)
            .ToList();  // Verwijderde de null-check aangezien dit niet nodig zou moeten zijn
    }

    private static SeizoensKlassementResponseContract MapToContract(Seizoensklassement entity) 
    {
        return new SeizoensKlassementResponseContract() 
        {
            SpelerId = entity.SpelerId,
            SeizoensId = entity.SeizoensId,
            Hoofdpunten = entity.Hoofdpunten,
            PlusMinPunten = entity.PlusMinPunten,
        };
    }
}

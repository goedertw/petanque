import React, { useEffect, useState } from "react";
const apiUrl = import.meta.env.VITE_API_URL;

// Interfaces
interface Speler {
    spelerId: number;
    voornaam: string;
    naam: string;
}

interface SpelResponseContract {
    spelId: number;
    speeldagId: number;
    terrein: string;
    spelerScores: any[]; // Wordt niet gebruikt
}

interface PlayerResponseContract extends Speler {}

interface SpelverdelingResponseContract {
    spelverdelingsId: number;
    spelId: number;
    team: string;
    spelerPositie: string;
    spelerVolgnr: number;
    speler: PlayerResponseContract;
    spel: SpelResponseContract;
}

interface Speeldag {
    speeldagId: number;
    datum: string;
}

interface Team {
    players: string[];
    points: number;
}

interface Game {
    spelId: number;
    terrein: string;
    teamA: Team;
    teamB: Team;
}

// Helpers
const createEmptyTeam = (): Team => ({
    players: [],
    points: 0,
});

const createEmptyGame = (): Game => ({
    spelId: 0,
    terrein: "",
    teamA: createEmptyTeam(),
    teamB: createEmptyTeam(),
});

const MatchScoreCard: React.FC = () => {
    const [games, setGames] = useState<Game[]>([createEmptyGame(), createEmptyGame(), createEmptyGame()]);
    const [terrein, setTerrein] = useState<number>(1);
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<number | null>(null);

    // Fetch speeldagen
    useEffect(() => {
        fetch(`${apiUrl}/speeldagen`)
            .then((res) => res.json())
            .then((data: Speeldag[]) => {
                setSpeeldagen(data);
                if (data.length > 0) {
                    setSelectedSpeeldag(data[0].speeldagId);
                }
            })
            .catch((err) => console.error("Fout bij laden van speeldagen:", err));
    }, []);

    // Fetch spelverdelingen voor het geselecteerde terrein
    useEffect(() => {
        if (selectedSpeeldag === null) return;

        // Haal spelverdelingen op, en filter op het geselecteerde terrein
        fetch(`${apiUrl}/spelverdelingen/${selectedSpeeldag}`)
            .then((res) => res.json())
            .then((data: SpelverdelingResponseContract[]) => {
                const spelMap: Record<number, Game> = {};
                // Loop door de spelverdelingen en groepeer spelers per team
                data.forEach((entry) => {
                    const terreinLabel = entry.spel.terrein;
                    const spelId = entry.spel.spelId;
                    const teamKey = entry.team === "Team A" ? "teamA" : "teamB"; // Toewijzen van team op basis van de naam
                    const spelerNaam = `${entry.speler.voornaam} ${entry.speler.naam}`;

                    // Controleer of het terrein overeenkomt met het geselecteerde terrein
                    if (terreinLabel.trim().toLowerCase() === `terrein ${terrein}`.toLowerCase()) {
                        if (!spelMap[spelId]) {
                            spelMap[spelId] = {
                                spelId,
                                terrein: terreinLabel,
                                teamA: createEmptyTeam(),
                                teamB: createEmptyTeam(),
                            };
                        }

                        // Voeg speler toe aan het juiste team
                        spelMap[spelId][teamKey].players.push(spelerNaam);
                    }
                });

                // Zet de gefilterde spellen in de state
                const mappedGames = Object.values(spelMap);
                setGames(mappedGames);
            })
            .catch((err) => console.error("Fout bij laden van spelverdeling:", err));
    }, [selectedSpeeldag, terrein]); // Alleen opnieuw ophalen bij verandering van speeldag of terrein

    const handleNameChange = (gameIndex: number, teamKey: "teamA" | "teamB", playerIndex: number, value: string) => {
        const updatedGames = [...games];
        updatedGames[gameIndex][teamKey].players[playerIndex] = value;
        setGames(updatedGames);
    };

    const handlePointsChange = (gameIndex: number, teamKey: "teamA" | "teamB", value: string) => {
        const updatedGames = [...games];
        const parsedValue = parseInt(value);
        updatedGames[gameIndex][teamKey].points = isNaN(parsedValue) ? 0 : Math.max(0, parsedValue);
        setGames(updatedGames);
    };

    /*dit is als je saved per player (in meeste gevallen dus een spel 4 keer opslaan door de 4 spelers)
    
    const handleSave = () => {
        if (!selectedSpeeldag) return;

        const spelResultaten: SpelRequestContract[] = [];

        games.forEach((game) => {
            ["teamA", "teamB"].forEach((teamKey) => {
                const team = game[teamKey as "teamA" | "teamB"];
                const scoreA = teamKey === "teamA" ? game.teamA.points : game.teamB.points;
                const scoreB = teamKey === "teamA" ? game.teamB.points : game.teamA.points;
                const terreinNaam = game.terrein;

                team.players.forEach((_, spelerIndex) => {
                    spelResultaten.push({
                        speeldagId: selectedSpeeldag,
                        terrein: terreinNaam,
                        spelerVolgnr: spelerIndex + 1,
                        scoreA,
                        scoreB
                    });
                });
            });
        });

        // Verstuur elke score apart naar de backend
        Promise.all(
            spelResultaten.map((spel) =>
                fetch(`${apiUrl}/scores`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(spel),
                }).then((res) => {
                    if (!res.ok) throw new Error("Fout bij opslaan van score");
                })
            )
        )
            .then(() => {
                alert("Alle scores succesvol opgeslagen!");
            })
            .catch((err) => {
                console.error("Fout bij opslaan:", err);
                alert("Er ging iets mis bij het opslaan.");
            });
    };*/

    const handleSave = () => {
        if (!selectedSpeeldag) return;

        Promise.all(




                 spelResultaten.map((spel) =>
                    fetch(`${apiUrl}/scores`/*/${game.spelId}`*/, {
                    method: "PUT",


                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify({
                        speeldagId: selectedSpeeldag,
                        terrein: spel.terrein,
                        spelerVolgnr: 1, // You can remove this if unused
                        scoreA: spel.teamA.points,
                        scoreB: spel.teamB.points
                    }),
                }).then((res) => {
                    if (!res.ok) throw new Error("Fout bij opslaan van score");
                })
            )
        )
            .then(() => {
                alert("Scores succesvol geüpdatet!");
            })
            .catch((err) => {
                console.error("Fout bij opslaan:", err);
                alert("Er ging iets mis bij het opslaan.");
            });
    };




    return (
        <div className="p-6 max-w-6xl mx-auto text-sm">
            <div className="mb-4">
                <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg">
                    VL@S - Wervik/Geluwe
                </h1>
                <div className="flex justify-between mt-2 text-sm items-center">
            <span className="flex items-center gap-2">
                <span className="bg-[#fbd46d] px-2 py-1 rounded">TERREIN</span>
                <input
                    type="number"
                    value={terrein}
                    min={1}
                    onChange={(e) => setTerrein(Math.max(1, parseInt(e.target.value) || 1))}
                    placeholder="Nr"
                    className="border border-[#74747c] p-2 rounded w-16"/>
            </span>
                    <span className="flex items-center gap-2">
                <label className="text-sm font-medium text-[#44444c]">Speeldag:</label>
                <select
                    value={selectedSpeeldag ?? ""}
                    onChange={(e) => setSelectedSpeeldag(parseInt(e.target.value))}
                    className="border rounded px-2 py-1 border-[#74747c]"
                >
                    {speeldagen.map((dag) => (
                        <option key={dag.speeldagId} value={dag.speeldagId}>
                            {new Date(dag.datum).toLocaleDateString()}
                        </option>
                    ))}
                </select>
            </span>
                </div>
            </div>

            {games.map((game, gameIndex) => (
                <div key={gameIndex} className="border rounded-xl p-6 mb-6 bg-white shadow-lg space-y-4">
                    <h2 className="text-lg font-semibold bg-[#ccac4c] text-white inline-block px-3 py-1 rounded-full">
                        SPEL {gameIndex + 1}
                    </h2>
                    <div className="flex justify-between items-center space-x-6">
                        <div className="w-full flex flex-col items-start">
                            <h3 className="font-medium text-lg text-left text-[#3c444c]">Team A</h3>
                            <div className="space-y-2 w-full">
                                {game.teamA.players.map((name, i) => (
                                    <input
                                        key={i}
                                        type="text"
                                        value={name}
                                        onChange={(e) => handleNameChange(gameIndex, "teamA", i, e.target.value)}
                                        placeholder={`Speler ${i + 1}`}
                                        className="border-b w-full px-2 py-1 mb-2 rounded-lg border-[#74747c]"
                                    />
                                ))}
                            </div>
                            <div className="mt-4">
                                <label className="block text-sm mb-1 text-[#44444c]">Aantal punten:</label>
                                <input
                                    type="number"
                                    value={game.teamA.points}
                                    min={0}
                                    max={13}
                                    onChange={(e) => handlePointsChange(gameIndex, "teamA", e.target.value)}
                                    className="border px-2 py-1 w-24 rounded-lg border-[#74747c]"
                                />
                                <div className="mt-2 text-sm font-medium text-[#44444c]">
                                    Aantal: <span className="font-bold">{game.teamA.points}</span>
                                    <span className="ml-4">
                                {game.teamA.points - game.teamB.points >= 0 ? "+" : ""}
                                        {game.teamA.points - game.teamB.points}
                            </span>
                                </div>
                            </div>
                        </div>

                        <div className="w-1 bg-gray-300 h-48 my-auto"></div> {/* Verticale scheidingslijn */}

                        <div className="w-full flex flex-col items-end">
                            <h3 className="font-medium text-lg text-right text-[#3c444c]">Team B</h3>
                            <div className="space-y-2 w-full">
                                {game.teamB.players.map((name, i) => (
                                    <input
                                        key={i}
                                        type="text"
                                        value={name}
                                        onChange={(e) => handleNameChange(gameIndex, "teamB", i, e.target.value)}
                                        placeholder={`Speler ${i + 1}`}
                                        className="border-b w-full px-2 py-1 mb-2 rounded-lg border-[#74747c]"
                                    />
                                ))}
                            </div>

                            <div className="mt-4">
                                <label className="block text-sm mb-1 text-[#44444c]">Aantal punten:</label>
                                <input
                                    type="number"
                                    value={game.teamB.points}
                                    min={0}
                                    max={13}
                                    onChange={(e) => handlePointsChange(gameIndex, "teamB", e.target.value)}
                                    className="border px-2 py-1 w-24 rounded-lg border-[#74747c]"
                                />
                                <div className="mt-2 text-sm font-medium text-[#44444c]">
                                    Aantal: <span className="font-bold">{game.teamB.points}</span>
                                    <span className="ml-4">
                                {game.teamB.points - game.teamA.points >= 0 ? "+" : ""}
                                        {game.teamB.points - game.teamA.points}
                            </span>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            ))}

            <div>
                <button
                    onClick={handleSave}
                    className="bg-[#3c444c] text-white px-4 py-2 rounded hover:bg-[#2f373f] transition cursor-pointer">
                    Opslaan
                </button>
            </div>
        </div>


    );
};

export default MatchScoreCard;

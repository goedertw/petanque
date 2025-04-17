import React, { useEffect, useState } from "react";

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
        // Fetch speeldagen from backend
        fetch("https://localhost:7241/api/speeldagen")
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
        fetch(`https://localhost:7241/api/spelverdelingen/${selectedSpeeldag}`)
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
                    if (terreinLabel === `Terrein ${terrein}`) {
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

    return (
        <div className="p-6 max-w-6xl mx-auto text-sm">
            <div className="mb-4">
                <h1 className="text-xl font-bold text-center">VL@S - Wervik/Geluwe</h1>
                <div className="flex justify-between mt-2 text-sm items-center">
                    <span className="flex items-center gap-2">
                        <span className="bg-orange-200 px-2 py-1 rounded">TERREIN</span>
                        <input
                            type="number"
                            value={terrein}
                            min={1}
                            onChange={(e) => setTerrein(Math.max(1, parseInt(e.target.value) || 1))}
                            placeholder="Nr"
                            className="border rounded px-2 py-1 w-16"
                        />
                    </span>
                    <span className="flex items-center gap-2">
                        <label className="text-sm font-medium">Speeldag:</label>
                        <select
                            value={selectedSpeeldag ?? ""}
                            onChange={(e) => setSelectedSpeeldag(parseInt(e.target.value))}
                            className="border rounded px-2 py-1"
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
                    <h2 className="text-lg font-semibold bg-yellow-300 inline-block px-3 py-1 rounded-full">
                        SPEL {gameIndex + 1}
                    </h2>
                    <div className="flex justify-between items-center space-x-6">
                        <div className="w-full flex flex-col items-start">
                            <h3 className="font-medium text-lg text-left text-blue-600">Team A</h3>
                            <div className="space-y-2 w-full">
                                {game.teamA.players.map((name, i) => (
                                    <input
                                        key={i}
                                        type="text"
                                        value={name}
                                        onChange={(e) => handleNameChange(gameIndex, "teamA", i, e.target.value)}
                                        placeholder={`Speler ${i + 1}`}
                                        className="border-b w-full px-2 py-1 mb-2 rounded-lg"
                                    />
                                ))}
                            </div>
                            {/*fixen da aanal punten niet boven 13 kunnen muss ook het plus min systeem integreren*/ }
                            <div className="mt-4">
                                <label className="block text-sm mb-1">Aantal punten:</label>
                                <input
                                    type="number"
                                    value={game.teamA.points}
                                    min={0}
                                    max={13}
                                    onChange={(e) => handlePointsChange(gameIndex, "teamA", e.target.value)}
                                    className="border px-2 py-1 w-24 rounded-lg"
                                />
                            </div>
                        </div>

                        <div className="w-1 bg-gray-300 h-48 my-auto"></div> {/* Verticale scheidingslijn */}

                        <div className="w-full flex flex-col items-end">
                            <h3 className="font-medium text-lg text-right text-red-600">Team B</h3>
                            <div className="space-y-2 w-full">
                                {game.teamB.players.map((name, i) => (
                                            <input
                                                key={i}
                                                type="text"
                                                value={name}
                                        onChange={(e) => handleNameChange(gameIndex, "teamB", i, e.target.value)}
                                                placeholder={`Speler ${i + 1}`}
                                        className="border-b w-full px-2 py-1 mb-2 rounded-lg"
                                            />
                                        ))}
                                    </div>

                            <div className="mt-4">
                                        <label className="block text-sm mb-1">Aantal punten:</label>
                                        <input
                                            type="number"
                                    value={game.teamB.points}
                                            min={0}
                                    onChange={(e) => handlePointsChange(gameIndex, "teamB", e.target.value)}
                                    className="border px-2 py-1 w-24 rounded-lg"
                                        />
                                    </div>

                                    <div className="mt-2 text-sm font-medium">
                                        Aantal (X): <span className="font-bold">{score}</span>
                                        <span className="ml-4">{difference >= 0 ? "+" : ""}{difference}</span>
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            ))}
        </div>
    );
};

export default MatchScoreCard;

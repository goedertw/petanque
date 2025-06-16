import { useEffect, useState } from "react";
import Kalender from "../Components/Kalender.tsx"; // ← importeer jouw eigen component!
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
    spelerScores: any[];
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

const formatDateToDutch = (dateString: string): string => {
    const date = new Date(dateString);
    return ` speeldag: ${date.getDate()} ${date.toLocaleDateString("nl-NL", { month: "long" })}`;
};

// Component
function Scorebladpagina() {
    const [gamesPerTerrein, setGamesPerTerrein] = useState<Record<string, Game[]>>({});
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
    const [showCalendar, setShowCalendar] = useState(false);

    // Fetch speeldagen
    useEffect(() => {
        fetch(`${apiUrl}/speeldagen`)
            .then((res) => res.json())
            .then((data: Speeldag[]) => {
                setSpeeldagen(data);

                if (data.length > 0) {
                    const opgeslagenSpeeldagId = localStorage.getItem("selectedSpeeldag");
                    let gekozenSpeeldagId = opgeslagenSpeeldagId ? parseInt(opgeslagenSpeeldagId) : data[0].speeldagId;

                    const speeldagBestaat = data.some((dag) => dag.speeldagId === gekozenSpeeldagId);

                    if (!speeldagBestaat) {
                        gekozenSpeeldagId = data[0].speeldagId;
                    }

                    const gekozenSpeeldag = data.find((dag) => dag.speeldagId === gekozenSpeeldagId) || data[0];
                    setSelectedSpeeldag(gekozenSpeeldag);
                }
            })
            .catch((err) => console.error("Fout bij laden van speeldagen:", err));
    }, []);

    // Fetch spelverdelingen per speeldag
    useEffect(() => {
        if (!selectedSpeeldag) return;

        fetch(`${apiUrl}/spelverdelingen/${selectedSpeeldag.speeldagId}`)
            .then((res) => res.json())
            .then((data: SpelverdelingResponseContract[]) => {
                const terreinMap: Record<string, Record<number, Game>> = {};

                data.forEach((entry) => {
                    const terreinLabel = entry.spel.terrein.trim();
                    const spelId = entry.spel.spelId;
                    const teamKey = entry.team === "Team A" ? "teamA" : "teamB";
                    const spelerNaam = `${entry.speler.spelerId} ${entry.speler.voornaam} ${entry.speler.naam}`;

                    if (!terreinMap[terreinLabel]) {
                        terreinMap[terreinLabel] = {};
                    }

                    if (!terreinMap[terreinLabel][spelId]) {
                        terreinMap[terreinLabel][spelId] = {
                            spelId,
                            terrein: terreinLabel,
                            teamA: createEmptyTeam(),
                            teamB: createEmptyTeam(),
                        };
                    }

                    terreinMap[terreinLabel][spelId][teamKey].players.push(spelerNaam);
                });

                const restoredGames: Record<string, Game[]> = {};

                for (const terrein in terreinMap) {
                    const key = `punten_${selectedSpeeldag.speeldagId}_terrein_${terrein}`;
                    const opgeslagen = localStorage.getItem(key);
                    const games = Object.values(terreinMap[terrein]);

                    if (opgeslagen) {
                        const parsed = JSON.parse(opgeslagen) as Game[];
                        restoredGames[terrein] = games.map((game) => {
                            const match = parsed.find((g) => g.spelId === game.spelId);
                            return match
                                ? {
                                    ...game,
                                    teamA: { ...game.teamA, points: match.teamA.points },
                                    teamB: { ...game.teamB, points: match.teamB.points },
                                }
                                : game;
                        });
                    } else {
                        restoredGames[terrein] = games;
                    }
                }

                setGamesPerTerrein(restoredGames);
            })
            .catch((err) => console.error("Fout bij laden van spelverdeling:", err));
    }, [selectedSpeeldag]);

    const handlePointsChange = (terrein: string, gameIndex: number, teamKey: "teamA" | "teamB", value: string) => {
        const updated = { ...gamesPerTerrein };
        const parsedValue = parseInt(value);
        updated[terrein][gameIndex][teamKey].points = isNaN(parsedValue) ? 0 : Math.max(0, parsedValue);
        setGamesPerTerrein(updated);

        if (selectedSpeeldag) {
            const key = `punten_${selectedSpeeldag.speeldagId}_terrein_${terrein}`;
            localStorage.setItem(key, JSON.stringify(updated[terrein]));
        }
    };

    const handleSave = async () => {
        if (!selectedSpeeldag) return;

        const allGames = Object.entries(gamesPerTerrein).flatMap(([_, games]) => games);
        const invalidGames = allGames.filter(
            (game) => game.teamA.points !== 13 && game.teamB.points !== 13
        );

        if (invalidGames.length > 0) {
            alert("Fout: Bij elk spel moet één team exact 13 punten hebben.");
            return;
        }

        try {
            await Promise.all(
                allGames.map((game) =>
                    fetch(`${apiUrl}/scores/${game.spelId}`, {
                        method: "PUT",
                        headers: { "Content-Type": "application/json" },
                        body: JSON.stringify({
                            speeldagId: selectedSpeeldag.speeldagId,
                            terrein: game.terrein,
                            scoreA: game.teamA.points,
                            scoreB: game.teamB.points,
                        }),
                    }).then((res) => {
                        if (!res.ok) throw new Error("Fout bij opslaan van score");
                    })
                )
            );

            // 🔁 Na scores succesvol opgeslagen → klassement genereren
            const klassementResp = await fetch(`${apiUrl}/dagklassementen/${selectedSpeeldag.speeldagId}`, {
                method: "POST",
            });

            if (!klassementResp.ok) {
                throw new Error("Fout bij genereren dagklassement");
            }

            alert("Scores én dagklassement succesvol opgeslagen!");
        } catch (error) {
            console.error("Fout bij opslaan of klassement:", error);
            alert("Er ging iets mis bij het opslaan of genereren van het klassement.");
        }
    };


    return (
        <div className="p-6 max-w-6xl mx-auto text-sm">
            <div className="mb-4">
                <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg">
                    VL@S - Scores
                </h1>

                {selectedSpeeldag !== null && (
                    <div className="text-black-800 text-xl font-bold italic mt-4 text-center">
                        {formatDateToDutch(selectedSpeeldag.datum)}
                    </div>
                )}

                <Kalender
                    speeldagen={speeldagen}
                    selectedSpeeldag={selectedSpeeldag}
                    onSelectSpeeldag={(speeldag) => {
                        setSelectedSpeeldag(speeldag);
                        localStorage.setItem("selectedSpeeldag", speeldag.speeldagId.toString());
                        setShowCalendar(false);
                    }}
                    showCalendar={showCalendar}
                    onToggleCalendar={() => setShowCalendar(!showCalendar)}
                />
            </div>

            {Object.entries(gamesPerTerrein).map(([terrein, games]) => (
                <div key={terrein} className="mb-10">
                    <h2 className="text-lg font-bold mb-2 bg-[#fbd46d] text-[#3c444c] px-4 py-2 rounded-lg shadow">
                        {terrein.toUpperCase()}
                    </h2>

                    {games.map((game, gameIndex) => (
                        <div key={gameIndex} className="border rounded-xl p-6 mb-6 bg-white shadow-lg space-y-4">
                            <h3 className="text-md font-semibold bg-[#ccac4c] text-white inline-block px-3 py-1 rounded-full">
                                SPEL {gameIndex + 1}
                            </h3>
                            <div className="flex justify-between items-center space-x-6">
                                <div className="w-full flex flex-col items-start">
                                    <h4 className="font-medium text-lg text-left text-[#3c444c]">Team A</h4>
                                    <div className="space-y-2 w-full">
                                        {game.teamA.players.map((name, i) => (
                                            <p key={i} className="border-b w-full px-2 py-1 mb-2 rounded-lg border-[#74747c]">
                                                {name}
                                            </p>
                                        ))}
                                    </div>
                                    <div className="mt-4">
                                        <label className="block text-sm mb-1 text-[#44444c]">Aantal punten:</label>
                                        <input
                                            type="number"
                                            value={game.teamA.points}
                                            min={0}
                                            max={13}
                                            onChange={(e) => handlePointsChange(terrein, gameIndex, "teamA", e.target.value)}
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

                                <div className="w-1 bg-gray-300 h-48 my-auto"></div>

                                <div className="w-full flex flex-col items-end">
                                    <h4 className="font-medium text-lg text-right text-[#3c444c]">Team B</h4>
                                    <div className="space-y-2 w-full">
                                        {game.teamB.players.map((name, i) => (
                                            <p key={i} className="border-b w-full px-2 py-1 mb-2 rounded-lg border-[#74747c]">
                                                {name}
                                            </p>
                                        ))}
                                    </div>
                                    <div className="mt-4">
                                        <label className="block text-sm mb-1 text-[#44444c]">Aantal punten:</label>
                                        <input
                                            type="number"
                                            value={game.teamB.points}
                                            min={0}
                                            max={13}
                                            onChange={(e) => handlePointsChange(terrein, gameIndex, "teamB", e.target.value)}
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
                </div>
            ))}

            <button
                onClick={handleSave}
                className="bg-[#3c444c] text-white px-4 py-2 rounded hover:bg-[#2f373f] transition cursor-pointer mt-6"
            >
                Opslaan
            </button>
        </div>
    );
}

export default Scorebladpagina;
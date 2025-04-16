import { useEffect, useState } from "react";
import '../../index.css';

interface Speler {
    spelerId: number;
    voornaam: string;
    naam: string;
}

interface Speeldag {
    speeldagId: number;
    datum: string;
}

interface Aanwezigheid {
    spelerId: number;
    speeldagId: number;
    aanwezig: boolean;
}

function Aanwezigheidspagina() {
    const [spelers, setSpelers] = useState<Speler[]>([]);
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [geselecteerdeSpeeldag, setGeselecteerdeSpeeldag] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [aanwezigheden, setAanwezigheden] = useState<Aanwezigheid[]>([]);

    // Functie om de aanwezigheden op te halen voor een specifieke speeldag
    const loadAanwezigheden = (speeldagId: number) => {
        setLoading(true); // Zet loading weer aan als we de data ophalen
        fetch(`https://localhost:7241/api/aanwezigheden?speeldagId=${speeldagId}`)
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij ophalen aanwezigheden");
                return res.json();
            })
            .then((data: Aanwezigheid[]) => {
                setAanwezigheden(data);
                setLoading(false); // Zet loading uit als de data binnen is
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    };

    // Fetch spelers
    useEffect(() => {
        fetch("https://localhost:7241/api/players")
            .then((res) => {
                if (!res.ok) throw new Error("Netwerkfout bij spelers");
                return res.json();
            })
            .then((data: Speler[]) => {
                setSpelers(data);
            })
            .catch((err) => {
                setError(err.message);
            });
    }, []);

    // Fetch speeldagen
    useEffect(() => {
        fetch("https://localhost:7241/api/speeldagen")
            .then((res) => {
                if (!res.ok) throw new Error("Netwerkfout bij speeldagen");
                return res.json();
            })
            .then((data: Speeldag[]) => {
                setSpeeldagen(data);
                if (data.length > 0) {
                    const eersteSpeeldagId = data[0].speeldagId;
                    setGeselecteerdeSpeeldag(eersteSpeeldagId); // Zet de eerste speeldag als standaard
                    loadAanwezigheden(eersteSpeeldagId); // Haal de aanwezigheden op voor de eerste speeldag
                }
            })
            .catch((err) => setError(err.message));
    }, []);

    // Als de geselecteerde speeldag verandert, laad de aanwezigheden opnieuw
    useEffect(() => {
        if (geselecteerdeSpeeldag) {
            loadAanwezigheden(geselecteerdeSpeeldag);
        }
    }, [geselecteerdeSpeeldag]); // Trigger opnieuw wanneer de speeldag verandert

    const bevestigAanwezigheid = (spelerId: number) => {
        if (!geselecteerdeSpeeldag) {
            setError("Geen speeldag gekozen");
            return;
        }

        const spelerVolgnr = 1;

        fetch("https://localhost:7241/api/aanwezigheden", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                SpeeldagId: geselecteerdeSpeeldag,
                SpelerId: spelerId,
                SpelerVolgnr: spelerVolgnr,
            }),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij bevestigen");
                return res.json();
            })
            .then(() => {
                setAanwezigheden((prev) => [
                    ...prev,
                    { spelerId, speeldagId: geselecteerdeSpeeldag, aanwezig: true },
                ]);
            })
            .catch(() => {
                setError("Fout bij bevestigen");
            });
    };

    if (loading) return <p className="text-center mt-10">Bezig met laden...</p>;
    if (error) return <p className="text-center text-red-600 mt-10">Fout: {error}</p>;

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold text-white bg-blue-600 p-4 rounded-xl text-center mb-8 shadow-lg">
                Aanwezigheid Overzicht
            </h1>

            {/* Speeldag Dropdown */}
            <div className="mb-8 flex flex-col sm:flex-row items-center gap-4">
                <label className="text-lg font-medium">Kies een speeldag:</label>
                <select
                    className="px-4 py-2 rounded-xl border border-gray-300 shadow"
                    value={geselecteerdeSpeeldag ?? ""}
                    onChange={(e) => setGeselecteerdeSpeeldag(parseInt(e.target.value))}
                >
                    {speeldagen.map((dag) => (
                        <option key={dag.speeldagId} value={dag.speeldagId}>
                            {new Date(dag.datum).toLocaleDateString()}
                        </option>
                    ))}
                </select>
            </div>

            {/* Spelers Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 w-full">
                {spelers.map((speler) => {
                    const spelerAanwezig = aanwezigheden.find(
                        (aanwezigheid) => aanwezigheid.spelerId === speler.spelerId
                    );

                    return (
                        <div
                            key={speler.spelerId}
                            className="bg-white rounded-2xl shadow-md p-6 flex flex-col justify-between w-full"
                        >
                            <div>
                                <h2 className="text-xl font-semibold text-gray-800 mb-1">
                                    {speler.voornaam} {speler.naam}
                                </h2>
                                <ul className="text-sm text-gray-700 mb-4"></ul>
                            </div>
                            <div className="mt-auto">
                                {spelerAanwezig ? (
                                    <p className="text-green-500 font-semibold">Aanwezig</p>
                                ) : (
                                    <button
                                        onClick={() => bevestigAanwezigheid(speler.spelerId)}
                                        className="bg-green-500 hover:bg-green-600 text-white px-4 py-2 rounded-xl w-full transition"
                                    >
                                        Bevestig aanwezigheid
                                    </button>
                                )}
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}

export default Aanwezigheidspagina;

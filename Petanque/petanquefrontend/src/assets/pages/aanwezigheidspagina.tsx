import { useEffect, useState } from "react";
import '../../index.css';
const apiUrl = import.meta.env.VITE_API_URL;
import Kalender from '../Components/Kalender.tsx';

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
    aanwezigheidId: number;
    speeldagId: number;
    spelerId: number;
    spelerVolgnr: number;
}

function Aanwezigheidspagina() {
    const [spelers, setSpelers] = useState<Speler[]>([]);
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [aanwezigheden, setAanwezigheden] = useState<Aanwezigheid[]>([]);
    const [showCalendar, setShowCalendar] = useState(false);
    const [sortField, setSortField] = useState<'voornaam' | 'naam'>('voornaam');

    const sortedSpelers = [...spelers].sort((a, b) => {
        if (sortField === 'voornaam') {
            return a.voornaam.localeCompare(b.voornaam);
        } else {
            return a.naam.localeCompare(b.naam);
        }
    });

    const loadAanwezigheden = () => {
        setLoading(true);
        fetch(`${apiUrl}/aanwezigheden`)
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij ophalen aanwezigheden");
                return res.json();
            })
            .then((data: Aanwezigheid[]) => {
                setAanwezigheden(data);
                setLoading(false);
            })
            .catch((err) => {
                setError(err.message);
                setLoading(false);
            });
    };

    useEffect(() => {
        fetch(`${apiUrl}/players`)
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

    useEffect(() => {
        const fetchSpeeldagen = async () => {
            try {
                const response = await fetch(`${apiUrl}/speeldagen`);
                if (!response.ok) throw new Error("Netwerkfout bij speeldagen");
                const data: Speeldag[] = await response.json();

                // Forceer datum naar YYYY-MM-DD → zelfde als in Spelverdeling
                const processedData = data.map(dag => ({
                    ...dag,
                    datum: new Date(dag.datum).toISOString(), // volledige string
                }));
                setSpeeldagen(processedData);

                setSpeeldagen(processedData);

                const savedSpeeldagId = localStorage.getItem('geselecteerdeSpeeldag');
                if (savedSpeeldagId) {
                    const foundSpeeldag = processedData.find(dag => dag.speeldagId.toString() === savedSpeeldagId);
                    setSelectedSpeeldag(foundSpeeldag || processedData[0]);
                } else {
                    setSelectedSpeeldag(processedData[0]);
                }

                loadAanwezigheden();
            } catch (err: any) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchSpeeldagen();
    }, []);

    useEffect(() => {
        if (selectedSpeeldag) {
            localStorage.setItem('geselecteerdeSpeeldag', selectedSpeeldag.speeldagId.toString());
            loadAanwezigheden();
        }
    }, [selectedSpeeldag]);

    const bevestigAanwezigheid = (spelerId: number) => {
        if (!selectedSpeeldag) {
            setError("Geen speeldag gekozen");
            return;
        }

        const spelerAanwezig = aanwezigheden.find(
            (aanwezigheid) => aanwezigheid.spelerId === spelerId && aanwezigheid.speeldagId === selectedSpeeldag.speeldagId
        );

        if (spelerAanwezig) return;

        const spelerVolgnr =
            aanwezigheden.filter((a) => a.speeldagId === selectedSpeeldag.speeldagId).length + 1;

        const nieuweAanwezigheid: Omit<Aanwezigheid, "aanwezigheidId"> = {
            speeldagId: selectedSpeeldag.speeldagId,
            spelerId,
            spelerVolgnr,
        };

        fetch(`${apiUrl}/aanwezigheden`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(nieuweAanwezigheid),
        })
            .then((res) => {
                if (!res.ok) throw new Error("Fout bij versturen van aanwezigheid");
                return res.json();
            })
            .then((data: Aanwezigheid) => {
                setAanwezigheden((prev) => [...prev, data]);
            })
            .catch((err) => {
                setError(err.message);
            });
    };

    const verwijderAanwezigheid = (aanwezigheidId: number) => {
        fetch(`${apiUrl}/aanwezigheden/${aanwezigheidId}`, {
            method: 'DELETE',
        })
            .then((res) => {
                if (!res.ok) throw new Error('Fout bij verwijderen van aanwezigheid');
                // Verwijderen uit de lokale state:
                setAanwezigheden(prev => prev.filter(a => a.aanwezigheidId !== aanwezigheidId));
            })
            .catch((err) => {
                setError(err.message);
            });
    };


    // const isSpeeldagInVerleden = (speeldagDatum: string) => {
    //     const speeldagDate = new Date(speeldagDatum);
    //     const today = new Date();
    //     return speeldagDate < today;
    // };
    //
    // const isSpeeldagVandaag = (speeldagDatum: string) => {
    //     const speeldagDate = new Date(speeldagDatum);
    //     const today = new Date();
    //     return (
    //         speeldagDate.getFullYear() === today.getFullYear() &&
    //         speeldagDate.getMonth() === today.getMonth() &&
    //         speeldagDate.getDate() === today.getDate()
    //     );
    // };

    const handleSelectSpeeldag = (speeldag: Speeldag) => {
        setSelectedSpeeldag(speeldag);
        localStorage.setItem('geselecteerdeSpeeldag', speeldag.speeldagId.toString());
        setShowCalendar(false);
    };

    const handleToggleCalendar = () => {
        setShowCalendar(!showCalendar);
    };

    if (loading) return <p className="text-center mt-10">Bezig met laden...</p>;
    if (error) return <p className="text-center text-red-600 mt-10">Fout: {error}</p>;

    const filteredAanwezigheden = selectedSpeeldag
        ? aanwezigheden.filter(a => a.speeldagId === selectedSpeeldag.speeldagId)
        : [];

    const formatDateToDutch = (dateString: string): string => {
        const date = new Date(dateString);
        return ` speeldag: ${date.getDate()} ${date.toLocaleDateString("nl-NL", { month: "long" })}`;
    };

    return (
        <div className="p-6">
            <h1 className="text-3xl font-bold text-white bg-[#3c444c] p-4 rounded-2xl shadow mb-6 text-center">
                Overzicht Aanwezigheden
            </h1>

            {speeldagen.length > 0 && selectedSpeeldag && (
                <Kalender
                    speeldagen={speeldagen}
                    selectedSpeeldag={selectedSpeeldag}
                    onSelectSpeeldag={handleSelectSpeeldag}
                    showCalendar={showCalendar}
                    onToggleCalendar={handleToggleCalendar}
                />
            )}

            {selectedSpeeldag !== null && (
                <div className="text-black-800 text-xl font-bold italic mt-1 text-center">
                    {
                        formatDateToDutch(
                            speeldagen.find((dag) => dag.speeldagId === selectedSpeeldag.speeldagId)?.datum ?? ""

                        )
                    }
                </div>
            )}

            <div className="overflow-x-auto max-h-[600px] overflow-y-auto border rounded-2xl shadow-md">
                <table className="min-w-full text-left text-sm">
                    <thead className="bg-[#3c444c] text-white sticky top-0">
                    <tr>
                        <th
                            className="cursor-pointer px-6 py-3"
                            onClick={() => setSortField('voornaam')}
                        >
                            Voornaam {sortField === 'voornaam' ? '▲' : ''}
                        </th>
                        <th
                            className="cursor-pointer px-6 py-3"
                            onClick={() => setSortField('naam')}
                        >
                            Achternaam {sortField === 'naam' ? '▲' : ''}
                        </th>
                        <th className="px-6 py-3">Aanwezig?</th>
                    </tr>
                    </thead>
                    <tbody>
                    {sortedSpelers.map((speler) => {
                        const spelerAanwezig = filteredAanwezigheden.find(
                            (aanwezigheid) => aanwezigheid.spelerId === speler.spelerId
                        );

                        const isAanwezig = !!spelerAanwezig;

                        return (
                            <tr
                                key={speler.spelerId}
                                className={
                                    isAanwezig
                                        ? 'bg-green-200 border-t-2 border-b-2 border-black'
                                        : 'bg-red-200 border-t-2 border-b-2 border-black'
                                }
                            >
                                <td className="px-6 py-4 border-b border-gray-200">{speler.voornaam}</td>
                                <td className="px-6 py-4 border-b border-gray-200">{speler.naam}</td>
                                <td className="px-6 py-4 border-b border-gray-200 text-center">
                                    <div className="flex items-center justify-center space-x-2">
                                        <input
                                            type="checkbox"
                                            checked={isAanwezig}
                                            onChange={() => {
                                                if (isAanwezig && spelerAanwezig) {
                                                    verwijderAanwezigheid(spelerAanwezig.aanwezigheidId);
                                                } else {
                                                    bevestigAanwezigheid(speler.spelerId);
                                                }
                                            }}

                                            className="h-5 w-5 cursor-pointer"
                                        />
                                        <div
                                            className={`inline-block px-3 py-1 rounded-full text-white text-xs font-semibold
                                ${isAanwezig ? 'bg-green-500' : 'bg-red-500'}`}
                                        >
                                            {isAanwezig ? 'Aanwezig' : 'Afwezig'}
                                        </div>
                                    </div>
                                </td>
                            </tr>
                        );
                    })}
                    </tbody>


                </table>
            </div>

        </div>
    );
}

export default Aanwezigheidspagina;

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

    const isSpeeldagInVerleden = (speeldagDatum: string) => {
        const speeldagDate = new Date(speeldagDatum);
        const today = new Date();
        return speeldagDate < today;
    };

    const isSpeeldagVandaag = (speeldagDatum: string) => {
        const speeldagDate = new Date(speeldagDatum);
        const today = new Date();
        return (
            speeldagDate.getFullYear() === today.getFullYear() &&
            speeldagDate.getMonth() === today.getMonth() &&
            speeldagDate.getDate() === today.getDate()
        );
    };

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

            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 w-full">
                {spelers.map((speler) => {
                    const spelerAanwezig = filteredAanwezigheden.find(
                        (aanwezigheid) => aanwezigheid.spelerId === speler.spelerId
                    );
                    const speeldagDatum = selectedSpeeldag?.datum || '';

                    const speeldagInVerleden = isSpeeldagInVerleden(speeldagDatum);
                    const speeldagIsVandaag = isSpeeldagVandaag(speeldagDatum);

                    return (
                        <div
                            key={speler.spelerId}
                            className="bg-white rounded-2xl shadow-md p-6 flex flex-col justify-between w-full"
                        >
                            <div>
                                <h2 className="text-xl font-semibold text-[#44444c] mb-1">
                                    {speler.voornaam} {speler.naam}
                                </h2>
                            </div>
                            <div className="mt-auto">
                                {speeldagIsVandaag ? (
                                    !spelerAanwezig ? (
                                        <button
                                            onClick={() => bevestigAanwezigheid(speler.spelerId)}
                                            className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold px-4 py-2 rounded-xl w-full transition cursor-pointer"
                                        >
                                            Bevestig aanwezigheid
                                        </button>
                                    ) : (
                                        <p className="bg-[#fbd46d] text-[#44444c] font-semibold px-2 py-1 rounded-full inline-block">
                                            Aanwezig
                                        </p>
                                    )
                                ) : speeldagInVerleden ? (
                                    spelerAanwezig ? (
                                        <p className="text-sm font-medium text-[#ccac4c]">Was aanwezig</p>
                                    ) : (
                                        <p className="text-sm font-medium text-[#74747c]">Was niet aanwezig</p>
                                    )
                                ) : (
                                    <button
                                        onClick={() => bevestigAanwezigheid(speler.spelerId)}
                                        className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold px-4 py-2 rounded-xl w-full transition cursor-pointer"
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

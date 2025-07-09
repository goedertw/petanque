import { useEffect, useState } from "react";
import Kalender from '../Components/Kalender.tsx';

const apiUrl = import.meta.env.VITE_API_URL;

interface Speeldag {
    speeldagId: number;
    datum: string;
}

function Spelverdeling() {
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
    const [pdfBlobUrl, setPdfBlobUrl] = useState<string | null>(null);
    const [showCalendar, setShowCalendar] = useState(false);

    useEffect(() => {
        const fetchSpeeldagen = async () => {
            try {
                const response = await fetch(`${apiUrl}/speeldagen`);
                if (!response.ok) throw new Error("Fout bij ophalen van speeldagen");
                const data: Speeldag[] = await response.json();
                setSpeeldagen(data);

                const savedSpeeldagId = localStorage.getItem('speeldagId');
                if (savedSpeeldagId) {
                    const foundSpeeldag = data.find((dag) => dag.speeldagId.toString() === savedSpeeldagId);
                    if (foundSpeeldag) {
                        setSelectedSpeeldag(foundSpeeldag);
                    }
                }
            } catch (error) {
                console.error("Fout bij laden van speeldagen:", error);
                alert("Kon speeldagen niet laden.");
            }
        };

        fetchSpeeldagen();
    }, []);

    useEffect(() => {
        if (selectedSpeeldag) {
            fetchPdf(selectedSpeeldag.speeldagId);
        }
    }, [selectedSpeeldag]);

    const fetchPdf = async (speeldagId: number) => {
        try {
            const response = await fetch(`${apiUrl}/pdfspelverdelingen/${speeldagId}`, {
                method: "POST",
                headers: {
                    Accept: "application/pdf",
                },
            });

            if (!response.ok) throw new Error("Fout bij ophalen van PDF");

            const blob = await response.blob();
            const url = URL.createObjectURL(blob);
            setPdfBlobUrl(url);
        } catch (error) {
            console.error("Fout bij ophalen van PDF:", error);
            alert("Kon PDF niet ophalen.");
        }
    };

    const handleSelectSpeeldag = async (speeldag: Speeldag) => {
        setSelectedSpeeldag(speeldag);
        localStorage.setItem('speeldagId', speeldag.speeldagId.toString());
        setShowCalendar(false);
    };

    const maakSpelverdeling = async () => {
        if (!selectedSpeeldag) {
            alert("Selecteer eerst een speeldag");
        }
        setPdfBlobUrl(null); // reset tonen oude PDF

        try {
            const response = await fetch(`${apiUrl}/spelverdelingen/${selectedSpeeldag.speeldagId}`, {
                method: "POST",
            });
            if (!response.ok) {
                const errMsg = await response.text();
                throw new Error(errMsg);
            }
            //alert("Spelverdeling aangemaakt!");
            fetchPdf(selectedSpeeldag.speeldagId);
        } catch (error) {
            console.error("Fout bij aanmaken van spelverdeling:", error.message);
            alert(error.message);
        }
    };

    const handleToggleCalendar = () => {
        setShowCalendar(!showCalendar);
    };

    const formatDate = (isoDate: string) => {
        const date = new Date(isoDate);
        return new Intl.DateTimeFormat("nl-NL", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
        }).format(date);
    };

    const formatDateToDutch = (dateString: string): string => {
        const date = new Date(dateString);
        return ` speeldag: ${date.getDate()} ${date.toLocaleDateString("nl-NL", { month: "long" })}`;
    };

    return (
        <div className="grid p-0">
            <h2 className="text-3xl font-bold text-white bg-[#3c444c] p-2 rounded-2xl shadow mb-4 text-center">
                Spelverdeling
            </h2>

            <Kalender
                speeldagen={speeldagen}
                selectedSpeeldag={selectedSpeeldag}
                onSelectSpeeldag={handleSelectSpeeldag}
                showCalendar={showCalendar}
                onToggleCalendar={handleToggleCalendar}
            />

            <button
                onClick={maakSpelverdeling}
                className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold px-6 py-3 rounded-xl transition cursor-pointer mb-2 place-self-center"
            >
                (her)genereer Spelverdeling
            </button>

            {pdfBlobUrl && selectedSpeeldag && (
                <div className="m-0 p-0">
                    <iframe
                        src={pdfBlobUrl}
                        width="100%"
                        height="1000px"
                        title="Spelverdeling PDF"
                        className="border rounded mb-4"
                    ></iframe>
                </div>
            )}
        </div>
    );
}

export default Spelverdeling;

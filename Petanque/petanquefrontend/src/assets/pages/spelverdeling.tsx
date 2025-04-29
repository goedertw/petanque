import { useEffect, useState } from "react";
const apiUrl = import.meta.env.VITE_API_URL;
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';

interface Speeldag {
    speeldagId: number;
    datum: string;
}

function SpeeldagenDropdown() {
    const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
    const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
    const [terrein, setTerrein] = useState<string>(() => localStorage.getItem('terrein') || "");
    const [pdfUrl, setPdfUrl] = useState<string | null>(() => localStorage.getItem('pdfUrl') || null);

    useEffect(() => {
        const fetchSpeeldagen = async () => {
            try {
                const response = await fetch(`${apiUrl}/speeldagen`);
                if (!response.ok) throw new Error("Fout bij ophalen van speeldagen");
                const data: Speeldag[] = await response.json();
                setSpeeldagen(data);

                // Check for the speeldagId saved in localStorage and select the speeldag
                const savedSpeeldagId = localStorage.getItem('speeldagId');
                if (savedSpeeldagId) {
                    const foundSpeeldag = data.find((dag) => dag.speeldagId.toString() === savedSpeeldagId);
                    setSelectedSpeeldag(foundSpeeldag || null);
                }
            } catch (error) {
                console.error("Fout bij laden van speeldagen:", error);
                alert("Kon speeldagen niet laden.");
            }
        };

        fetchSpeeldagen();
    }, []);

    useEffect(() => {
        if (selectedSpeeldag && terrein.trim()) {
            fetchPdf(selectedSpeeldag.speeldagId, terrein);
        }
    }, [selectedSpeeldag, terrein]);

    const formatDate = (isoDate: string) => {
        const date = new Date(isoDate);
        return new Intl.DateTimeFormat("nl-NL", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
        }).format(date);
    };

    const fetchPdf = async (speeldagId: number, terrein: string) => {
        try {
            const response = await fetch(
                `${apiUrl}/pdfspelverdelingen/${speeldagId}/${encodeURIComponent(terrein)}`,
                {
                    method: "POST",
                    headers: {
                        Accept: "application/pdf",
                    },
                }
            );

            if (!response.ok) throw new Error("Fout bij ophalen van PDF");

            const blob = await response.blob();
            const reader = new FileReader();
            reader.readAsDataURL(blob);
            reader.onloadend = () => {
                const base64data = reader.result as string;
                setPdfUrl(base64data);
                localStorage.setItem('pdfUrl', base64data);
            };
        } catch (error) {
            console.error("Fout bij ophalen van PDF:", error);
            alert("Kon PDF niet ophalen.");
        }
    };

    return (
        <div className="p-4 max-w-3xl mx-auto flex flex-col items-center">
            <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg mb-6 w-full">
                Spelverdelingen
            </h1>
            <h2 className="text-center w-full mb-4">Selecteer een speeldag</h2>

            <Calendar
                onClickDay={(value) => {
                    const clickedDate = new Date(value);
                    const matchingSpeeldag = speeldagen.find((dag) => {
                        const dagDate = new Date(dag.datum);
                        return (
                            dagDate.getFullYear() === clickedDate.getFullYear() &&
                            dagDate.getMonth() === clickedDate.getMonth() &&
                            dagDate.getDate() === clickedDate.getDate()
                        );
                    });

                    if (matchingSpeeldag) {
                        setSelectedSpeeldag(matchingSpeeldag);
                        localStorage.setItem('speeldagId', matchingSpeeldag.speeldagId.toString());
                    }
                }}
                value={selectedSpeeldag ? new Date(selectedSpeeldag.datum) : null}
                tileContent={({ date, view }) => {
                    if (view === 'month') {
                        const match = speeldagen.find((dag) => {
                            const dagDate = new Date(dag.datum);
                            return (
                                dagDate.getFullYear() === date.getFullYear() &&
                                dagDate.getMonth() === date.getMonth() &&
                                dagDate.getDate() === date.getDate()
                            );
                        });

                        return match ? (
                            <div className="flex justify-center items-center mt-1">
                                <div className="h-2 w-2 rounded-full bg-[#ccac4c]"></div>
                            </div>
                        ) : null;
                    }
                }}
                className="p-4 bg-white rounded-2xl shadow-md text-[#44444c] mb-4"
                tileClassName={({ date, view }) => {
                    if (view === 'month') {
                        const speeldag = speeldagen.find((dag) => {
                            const dagDate = new Date(dag.datum);
                            return (
                                dagDate.getFullYear() === date.getFullYear() &&
                                dagDate.getMonth() === date.getMonth() &&
                                dagDate.getDate() === date.getDate()
                            );
                        });

                        if (speeldag && speeldag.speeldagId === selectedSpeeldag?.speeldagId) {
                            return 'bg-[#ccac4c] text-white rounded-full'; // ✨ Highlight
                        }
                    }
                    return null;
                }}
            />

            <h2 className="text-center w-full mb-4">Voer een terrein in</h2>

            <input
                type="number"
                placeholder="Voer terrein in"
                value={terrein}
                onChange={(e) => {
                    setTerrein(e.target.value);
                    localStorage.setItem('terrein', e.target.value);
                    setPdfUrl(null);
                    localStorage.removeItem('pdfUrl');
                }}
                className="border p-2 rounded w-full mb-4"
            />

            {pdfUrl && selectedSpeeldag && (
                <div className="mt-6 w-full">
                    <h2 className="text-lg font-medium mb-2 text-center">
                        Spelverdeling voor {formatDate(selectedSpeeldag.datum)} – Terrein: {terrein}
                    </h2>

                    <iframe
                        src={pdfUrl}
                        width="100%"
                        height="600px"
                        title="Spelverdeling PDF"
                        className="border rounded mb-4"
                    ></iframe>

                    <a
                        href={pdfUrl}
                        download={`spelverdeling-speeldag-${selectedSpeeldag.speeldagId}-${terrein}.pdf`}
                        className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer block mx-auto w-auto"
                    >
                        Download PDF
                    </a>
                </div>
            )}
        </div>
    );

}

export default SpeeldagenDropdown;

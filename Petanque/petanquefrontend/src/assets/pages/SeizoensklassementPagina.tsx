import { useEffect, useState } from "react";
const apiUrl = import.meta.env.VITE_API_URL;

interface Seizoen {
    seizoensId: number;
    startdatum: string;
    einddatum: string;
}

function Seizoensklassementpagina() {
    const [seizoenen, setSeizoenen] = useState<Seizoen[]>([]);
    const [seizoenId, setSeizoenId] = useState<string>(() => localStorage.getItem('seizoensklassementSeizoenId') || "");
    const [pdfUrl, setPdfUrl] = useState<string | null>(() => localStorage.getItem('seizoensklassementPdfUrl') || null);

    useEffect(() => {
        const fetchSeizoenen = async () => {
            try {
                const response = await fetch(`${apiUrl}/seizoenen`);
                if (!response.ok) throw new Error("Fout bij ophalen van seizoenen");
                const data: Seizoen[] = await response.json();
                setSeizoenen(data);
            } catch (error) {
                console.error("Fout bij laden van seizoenen:", error);
                alert("Kon seizoenen niet laden.");
            }
        };
        fetchSeizoenen();
    }, []);

    const fetchPdf = async () => {
        if (!seizoenId) return;

        localStorage.setItem('seizoensklassementSeizoenId', seizoenId);

        try {
            const response = await fetch(`${apiUrl}/pdfseizoensklassementen/${seizoenId}`, {
                method: "POST",
                headers: { Accept: "application/pdf" },
            });
            if (!response.ok) throw new Error("Fout bij ophalen van PDF");

            const blob = await response.blob();
            const reader = new FileReader();
            reader.readAsDataURL(blob);
            reader.onloadend = () => {
                const base64data = reader.result as string;
                setPdfUrl(base64data);
                localStorage.setItem('seizoensklassementPdfUrl', base64data);
            };
        } catch (error) {
            console.error("Fout bij ophalen van PDF:", error);
            alert("Kon PDF niet laden.");
        }
    };

    return (
        <div className="p-4 max-w-3xl mx-auto">
            <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg mb-6">
                Seizoensklassementen
            </h1>

            <select
                value={seizoenId}
                onChange={(e) => {
                    setSeizoenId(e.target.value);
                    localStorage.setItem('seizoensklassementSeizoenId', e.target.value);
                    setPdfUrl(null);
                    localStorage.removeItem('seizoensklassementPdfUrl');
                }}
                className="border p-2 rounded w-full mb-4"
            >
                <option value="">Selecteer een seizoen</option>
                {[...seizoenen]
                    .sort((a, b) => new Date(a.startdatum).getTime() - new Date(b.startdatum).getTime())
                    .map((seizoen) => (
                        <option key={seizoen.seizoensId} value={seizoen.seizoensId.toString()}>
                            Seizoen {new Date(seizoen.startdatum).getFullYear()}:{" "}
                            {new Date(seizoen.startdatum).toLocaleDateString("nl-BE")} -{" "}
                            {new Date(seizoen.einddatum).toLocaleDateString("nl-BE")}
                        </option>
                    ))}
            </select>

            <button
                onClick={fetchPdf}
                className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
            >
                (her)genereer PDF
            </button>

            {pdfUrl && (
                <div className="mt-6">
                    <iframe
                        src={pdfUrl}
                        width="100%"
                        height="600px"
                        className="border rounded mb-4"
                    ></iframe>

                    <a
                        href={pdfUrl}
                        download={`seizoensklassement-${seizoenId}.pdf`}
                        className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
                    >
                        Download PDF
                    </a>
                </div>
            )}
        </div>
    );
}

export default Seizoensklassementpagina;

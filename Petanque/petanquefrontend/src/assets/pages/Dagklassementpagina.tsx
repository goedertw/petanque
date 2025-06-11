import { useEffect, useState } from "react";
const apiUrl = import.meta.env.VITE_API_URL;
import Kalender from '../Components/Kalender.tsx';  // jouw herbruikbare Kalender component

interface Speeldag {
  speeldagId: number;
  datum: string;
}

function Dagklassementpagina() {
  const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
  const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
  const [pdfUrl, setPdfUrl] = useState<string | null>(() => localStorage.getItem('dagklassementPdfUrl') || null);
  const [showCalendar, setShowCalendar] = useState(false);

  useEffect(() => {
    const fetchSpeeldagen = async () => {
      try {
        const response = await fetch(`${apiUrl}/speeldagen`);
        if (!response.ok) throw new Error("Fout bij ophalen van speeldagen");
        const data: Speeldag[] = await response.json();
        setSpeeldagen(data);

        const savedSpeeldagId = localStorage.getItem('dagklassementSpeeldagId');
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

  const formatDate = (isoDate: string) => {
    const date = new Date(isoDate);
    return new Intl.DateTimeFormat("nl-NL", {
      weekday: "long",
      year: "numeric",
      month: "long",
      day: "numeric",
    }).format(date);
  };

  const fetchPdf = async () => {
    if (!selectedSpeeldag) {
      alert("Selecteer een speeldag.");
      return;
    }

    localStorage.setItem('dagklassementSpeeldagId', selectedSpeeldag.speeldagId.toString());

    try {
      const response = await fetch(
          `${apiUrl}/pdfdagklassementen/${selectedSpeeldag.speeldagId}`,
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
        localStorage.setItem('dagklassementPdfUrl', base64data);
      };
    } catch (error) {
      console.error("Fout bij ophalen van PDF:", error);
      alert("Kon PDF niet ophalen.");
    }
  };

  const handleSelectSpeeldag = (speeldag: Speeldag) => {
    setSelectedSpeeldag(speeldag);
    localStorage.setItem('dagklassementSpeeldagId', speeldag.speeldagId.toString());
    setShowCalendar(false);
  };

  const handleToggleCalendar = () => {
    setShowCalendar(!showCalendar);
  };

  return (
      <div className="p-4 max-w-3xl mx-auto">
        <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg mb-6">
          Dagklassementen
        </h1>

        {speeldagen.length > 0 && (
            <Kalender
                speeldagen={speeldagen}
                selectedSpeeldag={selectedSpeeldag}
                onSelectSpeeldag={handleSelectSpeeldag}
                showCalendar={showCalendar}
                onToggleCalendar={handleToggleCalendar}
            />
        )}

        <button
            onClick={fetchPdf}
            className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer mt-4 block mx-auto"
        >
          Toon PDF
        </button>

        {pdfUrl && selectedSpeeldag && (
            <div className="mt-6">
              <h2 className="text-lg font-medium mb-2 text-center">
                Dagklassement voor {formatDate(selectedSpeeldag.datum)}
              </h2>

              <iframe
                  src={pdfUrl}
                  width="100%"
                  height="600px"
                  title="Dagklassement PDF"
                  className="border rounded mb-4"
              ></iframe>

              <a
                  href={pdfUrl}
                  download={`dagklassement-speeldag-${selectedSpeeldag.speeldagId}-${selectedSpeeldag.datum}.pdf`}
                  className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer block mx-auto"
              >
                Download PDF
              </a>
            </div>
        )}
      </div>
  );
}

export default Dagklassementpagina;

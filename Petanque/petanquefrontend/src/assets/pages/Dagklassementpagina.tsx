import { useEffect, useState } from "react";
const apiUrl = import.meta.env.VITE_API_URL;
import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';

interface Speeldag {
  speeldagId: number;
  datum: string;
}

function Dagklassementpagina() {
  const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
  const [speeldagId, setSpeeldagId] = useState<string>(() => localStorage.getItem('dagklassementSpeeldagId') || "");
  const [pdfUrl, setPdfUrl] = useState<string | null>(() => localStorage.getItem('dagklassementPdfUrl') || null);
  const [selectedDag, setSelectedDag] = useState<Speeldag | null>(null);

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
          setSelectedDag(foundSpeeldag || null);
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
    if (!speeldagId) {
      alert("Selecteer een speeldag.");
      return;
    }

    const selected = speeldagen.find((dag) => dag.speeldagId.toString() === speeldagId);
    setSelectedDag(selected || null);
    localStorage.setItem('dagklassementSpeeldagId', speeldagId);

    try {
      const response = await fetch(
          `${apiUrl}/pdfdagklassementen/${speeldagId}`,
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

  return (
      <div className="p-4 max-w-3xl mx-auto">
        <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg mb-6">
          Dagklassementen
        </h1>
          <h2 className="text-center">Selecteer een speeldag:</h2>

          <div className="flex justify-center mt-4 mb-4">
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
                          setSelectedDag(matchingSpeeldag);
                          setSpeeldagId(matchingSpeeldag.speeldagId.toString());
                          localStorage.setItem('dagklassementSpeeldagId', matchingSpeeldag.speeldagId.toString());
                          setPdfUrl(null);
                          localStorage.removeItem('dagklassementPdfUrl');
                      }
                  }}
                  value={selectedDag ? new Date(selectedDag.datum) : null}
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
                  tileClassName={({ date, view }) => {
                      if (view === 'month') {
                          const match = speeldagen.find((dag) => {
                              const dagDate = new Date(dag.datum);
                              return (
                                  dagDate.getFullYear() === date.getFullYear() &&
                                  dagDate.getMonth() === date.getMonth() &&
                                  dagDate.getDate() === date.getDate()
                              );
                          });

                          if (match && selectedDag && match.speeldagId === selectedDag.speeldagId) {
                              return 'bg-[#ccac4c] text-white rounded-full';
                          }
                      }
                      return null;
                  }}
                  className="p-4 bg-white rounded-2xl shadow-md text-[#44444c]"
              />
          </div>


          <button
            onClick={fetchPdf}
            className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
        >
          Toon PDF
        </button>

        {pdfUrl && selectedDag && (
            <div className="mt-6">
              <h2 className="text-lg font-medium mb-2">
                Dagklassement voor {formatDate(selectedDag.datum)}
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
                  download={`dagklassement-speeldag-${selectedDag.speeldagId}-${selectedDag.datum}.pdf`}
                  className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
              >
                Download PDF
              </a>
            </div>
        )}
      </div>
  );
}

export default Dagklassementpagina;

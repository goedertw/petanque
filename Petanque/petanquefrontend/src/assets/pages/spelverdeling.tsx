import React, { useEffect, useState } from "react";
const apiUrl = import.meta.env.VITE_API_URL;

interface Speeldag {
  speeldagId: number;
  datum: string;
}

function SpeeldagenDropdown() {
  const [speeldagen, setSpeeldagen] = useState<Speeldag[]>([]);
  const [speeldagId, setSpeeldagId] = useState<string>("");
  const [selectedSpeeldag, setSelectedSpeeldag] = useState<Speeldag | null>(null);
  const [terrein, setTerrein] = useState<string>("");
  const [pdfUrl, setPdfUrl] = useState<string | null>(null);

  useEffect(() => {
    const fetchSpeeldagen = async () => {
      try {
        const response = await fetch(`${apiUrl}/speeldagen`);
        if (!response.ok) throw new Error("Fout bij ophalen van speeldagen");
        const data: Speeldag[] = await response.json();
        setSpeeldagen(data);
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
    if (!speeldagId || !terrein.trim()) {
      alert("Gelieve zowel een speeldag als een terrein in te geven.");
      return;
    }

    const selected = speeldagen.find((dag) => dag.speeldagId.toString() === speeldagId);
    setSelectedSpeeldag(selected || null);

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
      const url = URL.createObjectURL(blob);
      setPdfUrl(url);
    } catch (error) {
      console.error("Fout bij ophalen van PDF:", error);
      alert("Kon PDF niet ophalen.");
    }
  };

  return (
    <div className="p-4 max-w-3xl mx-auto">
      <h1 className="text-xl font-bold text-center text-[#f7f7f7] bg-[#3c444c] p-4 rounded-2xl shadow-lg mb-6">
        Spelverdelingen
      </h1>

      <select
        value={speeldagId}
        onChange={(e) => {
          setSpeeldagId(e.target.value);
          setPdfUrl(null); // reset bij nieuwe selectie
        }}
        className="border p-2 rounded w-full mb-4"
      >
        <option value="">Selecteer een speeldag</option>
        {speeldagen.map((dag) => (
          <option key={dag.speeldagId} value={dag.speeldagId.toString()}>
            Speeldag {dag.speeldagId} – {formatDate(dag.datum)}
          </option>
        ))}
      </select>

      <input
        type="text"
        placeholder="Voer terrein in"
        value={terrein}
        onChange={(e) => setTerrein(e.target.value)}
        className="border p-2 rounded w-full mb-4"
      />

      <button
        onClick={fetchPdf}
        className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer w-full"
      >
        Genereer PDF
      </button>

      {pdfUrl && selectedSpeeldag && (
        <div className="mt-6">
          <h2 className="text-lg font-medium mb-2">
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
            className="bg-[#fbd46d] text-[#3c444c] font-bold py-2 px-4 rounded hover:bg-[#f7c84c] transition cursor-pointer"
          >
            Download PDF
          </a>
        </div>
      )}
    </div>
  );
}

export default SpeeldagenDropdown;

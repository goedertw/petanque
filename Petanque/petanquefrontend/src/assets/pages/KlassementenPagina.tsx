import { useEffect, useState } from "react";
import Dagklassementpagina from "./Dagklassementpagina";
import Seizoensklassementpagina from "./SeizoensklassementPagina";

type KlassementType = "dag" | "seizoen";

function KlassementenPagina() {
    const [activeTab, setActiveTab] = useState<KlassementType>(() => {
        return (localStorage.getItem("activeTab") as KlassementType) || "dag";
    });

    useEffect(() => {
        localStorage.setItem("activeTab", activeTab);
    }, [activeTab]);

    return (
        <div className="p-4 max-w-3xl mx-auto">
            <div className="flex space-x-4 mb-6">
                <button
                    onClick={() => setActiveTab("dag")}
                    className={`py-2 px-4 rounded-t-lg font-medium ${
                        activeTab === "dag"
                            ? "bg-[#fbd46d] text-[#3c444c]"
                            : "bg-gray-200 text-gray-700"
                    }`}
                >
                    Dagklassementen
                </button>
                <button
                    onClick={() => setActiveTab("seizoen")}
                    className={`py-2 px-4 rounded-t-lg font-medium ${
                        activeTab === "seizoen"
                            ? "bg-[#fbd46d] text-[#3c444c]"
                            : "bg-gray-200 text-gray-700"
                    }`}
                >
                    Seizoensklassementen
                </button>
            </div>

            {activeTab === "dag" ? <Dagklassementpagina /> : <Seizoensklassementpagina />}
        </div>
    );
}

export default KlassementenPagina;

function HomePagina() {
    return (
        <div className="min-h-screen bg-gray-100 flex flex-col items-center pt-16 text-gray-800 px-4">
            <div className="max-w-xl w-full text-center bg-white border border-[#fbd46d] shadow-lg rounded-2xl p-10">
                <h1 className="text-4xl font-bold text-[#3c444c] mb-6">
                    Welkom bij de <span className="text-[#fbd46d]">VL@S Petanque</span> App
                </h1>
                <p className="text-lg leading-relaxed">
                    Gebruik het menu hierboven om spellen te starten, scores in te voeren, klassementen te bekijken, aanwezigheden op te nemen en meer.
                </p>
            </div>
        </div>
    );
}

export default HomePagina;

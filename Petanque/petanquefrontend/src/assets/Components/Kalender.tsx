import Calendar from 'react-calendar';
import 'react-calendar/dist/Calendar.css';

interface Speeldag {
    speeldagId: number;
    datum: string;
}

interface KalenderProps {
    speeldagen: Speeldag[];
    selectedSpeeldag: Speeldag | null;
    onSelectSpeeldag: (speeldag: Speeldag) => void;
    onClickOnNewDate?: (date: Date) => void;
    showCalendar: boolean;
    onToggleCalendar: () => void;
}

const formatDateToDutch = (dateString: string): string => {
    const date = new Date(dateString);
    return `${date.toLocaleDateString("nl-NL", { weekday: "short", day: "numeric", month: "long", year: "numeric" })}`;
};

function Kalender({ speeldagen, selectedSpeeldag, onSelectSpeeldag, showCalendar, onToggleCalendar, onClickOnNewDate }: KalenderProps) {
    return (
        <div className="flex">
            <div className="mb-8 w-full">
                <span className="justify-center item-center">Speeldag:&nbsp;
                    <b>{formatDateToDutch(speeldagen.find((dag) => dag.speeldagId === selectedSpeeldag?.speeldagId)?.datum ?? "")}</b>
                </span>&nbsp;&nbsp;&nbsp;
                <button
                    onClick={onToggleCalendar}
                    className="bg-[#ccac4c] hover:bg-[#b8953d] text-white font-bold px-6 py-3 rounded-xl transition cursor-pointer"
                >
                    {showCalendar ? 'Verberg kalender' : 'Wijzig speeldag'}
                </button>

                {showCalendar && (
                    <div className="flex justify-center w-full">
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
                                    onSelectSpeeldag(matchingSpeeldag);
                                } else {
                                    if (onClickOnNewDate) {
                                        onClickOnNewDate(clickedDate);
                                    }
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
                            className="p-4 bg-white rounded-2xl shadow-md text-[#44444c] mb-4 w-full"
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
                                        return 'bg-[#ccac4c] text-white rounded-full';
                                    }
                                }
                                return null;
                            }}
                        />
                    </div>
                )}
            </div>
        </div>
    );
}

export default Kalender;
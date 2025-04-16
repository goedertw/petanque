import { Routes, Route, useNavigate } from 'react-router-dom'
import SecondPage from './pages/SecondPage'
import { Routes, Route, useNavigate } from 'react-router-dom'
import SecondPage from './assets/pages/aanwezigheidspagina.tsx'

function App() {
    const navigate = useNavigate()

    return (
        <>
            <Routes>
                <Route path="/second" element={<SecondPage />} />
            </Routes>

            <button onClick={() => navigate('/second')}>Aanwezigheden Opnemen</button>
        </>
    )
}

export default App
export default App

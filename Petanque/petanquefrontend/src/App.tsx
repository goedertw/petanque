import { Routes, Route, useNavigate } from 'react-router-dom'
<<<<<<< HEAD
import SecondPage from './assets/pages/aanwezigheidspagina.tsx'
=======
import SecondPage from './pages/SecondPage'
>>>>>>> origin/master

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

<<<<<<< HEAD
export default App
=======
export default App
>>>>>>> origin/master

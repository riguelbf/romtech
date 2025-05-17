import { Home } from './pages/Home'
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function App() {
 

  return (
    <>
        <Home />
        <ToastContainer aria-label="Toast notifications" />
    </>
  )
}

export default App

import { useState, useEffect } from 'react';
import ReactDOM from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import './App.css';

import Home from './Pages/Home.jsx';
import Layout from './Pages/Layout/Layout.jsx';
import ErrorPage from './Pages/ErrorPage.jsx';
import CityTable from './Pages/CityTable.jsx';
import SunsetSunriseTable from './Pages/SunsetSunriseTable.jsx';
import Login from './Pages/Login.jsx';
import Register from './Pages/Register.jsx';
import SunsetSunrise from './Pages/SunsetSunrise.jsx';

function App() {
  const url = "http://localhost:5062/";

  const [loading, setLoading] = useState(false);
  const [userRole, setUserRole] = useState("guest");

  async function fetchRole() {
    try{
      const res = await fetch(`${url}SolarWatch/GetRole`, {
        method: "GET",
        credentials: 'include',
      });

      if (!res.ok) {
        throw new Error(`HTTP error! status: ${res.status}`);
      }

      return await res.text();
    }
    catch(error){
      throw error;
    }
  };

  function handleUserRole(role) {
    setUserRole(role);
  }

  useEffect(() => {
    setLoading(true);
    fetchRole()
        .then(role => setUserRole(role), setLoading(false));
  }, []);

  const router = createBrowserRouter([
    {
      path: "/",
      element: <Layout url={url} userRole={userRole} handleUserRole={handleUserRole} />,
      errorElement: <ErrorPage />,
      children: [
        {
          path: "/",
          element: <Home />,
        },
        {
          path: "/SunsetSunriseTable",
          element: <SunsetSunriseTable />,
        },
        {
          path: "/CityTable",
          element: <CityTable />,
        },
        {
          path: "/Register",
          element: <Register url={url} />,
        },
        {
          path: "/Login",
          element: <Login url={url} handleUserRole={handleUserRole} />,
        }
      ],
    },
  ]);

  return (
    <RouterProvider router={router} />
  )
}

export default App

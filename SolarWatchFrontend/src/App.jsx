import { useState } from 'react';
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

  const router = createBrowserRouter([
    {
      path: "/",
      element: <Layout />,
      errorElement: <ErrorPage />,
      children: [
        {
          path: "/",
          element: <Home />,
        },
        {
          path: "/SunsetSunrise",
          element: <SunsetSunrise />,
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
          element: <Login url={url} />,
        }
      ],
    },
  ]);

  return (
    <RouterProvider router={router} />
  )
}

export default App

import React from 'react';
import ReactDOM from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';
import App from './App.jsx';
import './index.css';

import Home from './Pages/Home.jsx';
import Layout from './Pages/Layout/Layout.jsx';
import ErrorPage from './Pages/ErrorPage.jsx';
import CityTable from './Pages/CityTable.jsx';
import SunsetSunriseTable from './Pages/SunsetSunriseTable.jsx';
import Login from './Pages/Login.jsx';
import Register from './Pages/Register.jsx';
import SunsetSunrise from './Pages/SunsetSunrise.jsx';

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
        element: <Register />,
      },
      {
        path: "/Login",
        element: <Login />,
      }
    ],
  },
]);

ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>,
)

import React from 'react';
import { Outlet, Link } from "react-router-dom";

import "./Layout.css";

function Layout() {
  return (
    <div className="Layout">
    <nav>
      <ul>
        <li className="grow">
          <Link to="/">Home</Link>
        </li>
        <li className="grow">
          <Link to="/SunsetSunrise">SunsetSunrise</Link>
        </li>
        <li className="grow">
          <Link to="/Login">Login</Link>
        </li>
        <li className="grow">
          <Link to="/Register">Register</Link>
        </li>
        <li className="grow">
          <Link to="/CityTable">CityTable</Link>
        </li>
        <li className="grow">
          <Link to="/SunsetSunriseTable">SunsetSunriseTable</Link>
        </li>
      </ul>
    </nav>
    <Outlet />
  </div>
  )
}

export default Layout
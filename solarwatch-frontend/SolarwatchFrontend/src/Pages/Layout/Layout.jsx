import React from 'react'
import { Outlet, Link } from "react-router-dom";

import "./Layout.css";

const Layout = () => (
  <div className="Layout">
    <nav>
      <ul>
        <li>
          <Link to="/">
            <button type="button">Home</button>
          </Link>
        </li>
        <li>
          <Link to="/login">
            <button type="button">Login</button>
          </Link>
        </li>
        <li>
          <Link to="/registration">
            <button type="button">Register</button>
          </Link>
        </li>
        <li>
          <Link to="/solar-watch">
            <button type="button">Solar Watch</button>
          </Link>
        </li>
      </ul>
    </nav>
    <Outlet />
  </div>
);

export default Layout;
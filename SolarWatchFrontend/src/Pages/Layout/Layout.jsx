import React from 'react';
import { Outlet, Link } from "react-router-dom";
import { useState, useEffect } from 'react';
import ReactDOM from 'react-dom/client';

import "./Layout.css";

function Layout({url, userRole, handleUserRole}) {

  async function fetchLogout() {
    try{
      const res = await fetch(`${url}Auth/Logout`, {
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
  }

  function handleLogout(e) {
    e.preventDefault();
    fetchLogout();
    handleUserRole("guest");
  }

  return (
  <div className="Layout">
    <nav>
      <ul>
        <li className="grow">
          <Link to="/">Home</Link>
        </li>
        {userRole !== "guest" ?
          <li className="grow" onClick={e => handleLogout(e)} id="Logout">Logout</li> 
          :
          <>
            <li className="grow">
              <Link to="/Login">Login</Link>
            </li>
            <li className="grow">
              <Link to="/Register">Register</Link>
            </li>
          </>
        }
        {userRole === "Admin" ?
        <>
          <li className="grow">
            <Link to="/CityTable">CityTable</Link>
          </li>
          <li className="grow">
            <Link to="/SunsetSunriseTable">SunsetSunriseTable</Link>
          </li>
        </>
        : <></>
        }
      </ul>
    </nav>
    <Outlet />
  </div>
  )
}

export default Layout
import React from 'react';
import { Outlet, Link } from "react-router-dom";
import { useState, useEffect } from 'react';
import ReactDOM from 'react-dom/client';

import "./Layout.css";

function getCookie(cname) {
  let name = cname + "=";
  let decodedCookie = decodeURIComponent(document.cookie);
  let ca = decodedCookie.split(';');
  for (let i = 0; i < ca.length; i++) {
      let c = ca[i];
      while (c.charAt(0) == ' ') {
          c = c.substring(1);
      }
      if (c.indexOf(name) == 0) {
          return c.substring(name.length, c.length);
      }
  }
  return "";
}

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
          <Link to="/">Home {userRole}</Link>
        </li>
        {userRole !== "guest" ?
          <li className="grow" onClick={e => handleLogout(e)}>Logout</li> 
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
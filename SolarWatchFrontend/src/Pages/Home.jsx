import React from 'react';
import { useState } from "react";

function Home({url, userRole}) {
  const [cityName, setCityName] = useState("Delhi");
  const [year, setYear] = useState(2022);
  const [month, setMonth] = useState(2);
  const [day, setDay] = useState(2);
  const [sunriseSunset, setSunriseSunset] = useState(undefined);
  const [loading, setLoading] = useState(false);

  async function fetchSuriseSunset() {
    try {
      const res = await fetch(`${url}SolarWatch/GetSunriseSunset?cityName=${cityName}&year=${year}&month=${month}&day=${day}`, {
        method: "GET",
        credentials: 'include'
      });

      if (!res.ok) {
        throw new Error(`HTTP error! status: ${res.status}`);
      }

      const resJson = await res.json();
      return resJson;
    }
    catch (error) {
      throw error;
    }
  }

  async function onSubmit(e) {
    e.preventDefault();
    setLoading(true);
    setSunriseSunset(await fetchSuriseSunset());
    setLoading(false);
    console.log("sunriseSunset: ");
    console.log(sunriseSunset);
  }

  return (
    <div>
        <h1>Welcome to SolarWatch!</h1>
        {userRole === "guest" ?
          <h2>To use the site log in.</h2>
          :
          <>
          <form className="EmployeeForm" onSubmit={e => onSubmit(e)}>
            <div className="control">
              <label htmlFor="cityName">City name:</label>
              <input
                value={cityName}
                onChange={(e) => setCityName(e.target.value)}
                name="cityName"
                id="cityName"
              />
            </div>
  
            <div className="control">
              <label htmlFor="year">Year:</label>
              <input
                value={year}
                onChange={(e) => setYear(e.target.value)}
                name="year"
                id="year"
              />
            </div>
  
            <div className="control">
              <label htmlFor="month">Month:</label>
              <input
                value={month}
                onChange={(e) => setMonth(e.target.value)}
                name="month"
                id="month"
              />
            </div>
  
            <div className="control">
              <label htmlFor="day">Day:</label>
              <input
                value={day}
                onChange={(e) => setDay(e.target.value)}
                name="day"
                id="day"
              />
            </div>
  
            <div className="buttons">
              <button type="submit" disabled={loading}>
                "Submit"
              </button>
            </div>
          </form>
          <div className='sunriseSunsetContainer'>
          {loading ? 
            <h2>"Loading"</h2>
            :
            <>
            {sunriseSunset !== undefined ? 
              <>
                <h2>{sunriseSunset.sunrise}</h2>
                <h2>{sunriseSunset.sunset}</h2>
                <h2>{sunriseSunset.dayLength}</h2>
              </>
              :
              <></>
            }
            </>
          }
          </div>
          </>
        }

        
    </div>
  )
}

export default Home
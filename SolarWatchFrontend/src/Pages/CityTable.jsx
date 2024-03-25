import React from 'react';
import CityForm from '../Components/CityForm';
import { useState, useEffect } from 'react';
import { useNavigate } from "react-router-dom";

function CityTable({url, userRole}) {
  const navigate = useNavigate();
  
  if (userRole !== "Admin") {
    navigate('/');
  }

  const [cities, setCities] = useState(undefined);
  const [loading, setLoading] = useState(true);
  const [pickedCity, setPickedCity] = useState(undefined);

  async function fetchCities() {
    try {
      const res = await fetch(`${url}SolarWatch/GetCities`, {
        method: "GET",
        credentials: 'include'
      });

      if (!res.ok) {
        throw new Error(`HTTP error! status: ${res.status}`);
      }

      var resJson = await res.json();
      return resJson;
    } 
    catch (error) {
      setLoading(false);
      throw error
    }
  }

  function onClickPick(name, latitude, longitude, state, country) {
    setPickedCity({
      name: name,
      latitude: latitude,
      longitude: longitude,
      state: state,
      country: country
    });
  }

  useEffect(() => {
    setLoading(true);
    fetchCities()
      .then(citiesJson => setCities(citiesJson), setLoading(false));
  }, [])
  

  return (
    <div>
      <CityForm url={url} pickedCity={pickedCity} />;
      {
        cities === undefined ?
          <></>
          :
          <div className="EmployeeTable">
            <table>
              <thead>
                <tr>
                  <th>Name</th>
                  <th>Latitude</th>
                  <th>Longitude</th>
                  <th>State</th>
                  <th>Country</th>
                  <th />
                </tr>
              </thead>
              <tbody>
                {cities.map((city) => (
                  <tr key={city.name}>
                    <td>{city.name}</td>
                    <td>{city.latitude}</td>
                    <td>{city.longitude}</td>
                    <td>{city.state}</td>
                    <td>{city.country}</td>

                    <td>
                      <button type="button" onClick={() => onClickPick(city.name, city.latitude, city.longitude, city.state, city.country)}>Pick</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
      }
    </div>
  )
}

export default CityTable
import React from 'react';
import { useState } from 'react';

function CityForm({url, city}) {
  const [name, setName] = useState(city?.name ?? "");
  const [latitude, setLatitude] = useState(city?.latitude ?? "");
  const [longitude, setLongitude] = useState(city?.longitude ?? "");
  const [state, setState] = useState(city?.state ?? "");
  const [country, setCountry] = useState(city?.country ?? "");
  const [loading, setLoading] = useState(false);

  async function fetchPostCity() {
    try {
      const res = await fetch(`${url}SolarWatch/PostCity?name=${name}&latitude=${latitude}&longitude=${longitude}&state=${state}&country=${country}`, {
        method: "POST",
        credentials: 'include',
        headers: { 
          "Content-Type": "application/json",
           }
      });

      if (!res.ok) {
        throw new Error(`HTTP error! status: ${res.status}`);
      }
    } 
    catch (error) {
      throw error
    }
  }

  async function onSubmit(e) {
    setLoading(true);
    e.preventDefault();
    await fetchPostCity();
    setLoading(false);
  }

  async function onCancel(e) {
    e.preventDefault();
    setName("");
    setLatitude("");
    setLongitude("");
    setState("");
    setCountry("");
  }

  return (
    <div>
      <form className="EmployeeForm" onSubmit={e => onSubmit(e)}>
        <div className="control">
          <label htmlFor="name">Name:</label>
          <input
            value={name}
            onChange={(e) => setName(e.target.value)}
            name="name"
            id="name"
          />
        </div>

        <div className="control">
          <label htmlFor="latitude">Latitude:</label>
          <input
            value={latitude}
            onChange={(e) => setLatitude(e.target.value)}
            name="latitude"
            id="latitude"
          />
        </div>

        <div className="control">
          <label htmlFor="longitude">Longitude:</label>
          <input
            value={longitude}
            onChange={(e) => setLongitude(e.target.value)}
            name="longitude"
            id="longitude"
          />
        </div>

        <div className="control">
          <label htmlFor="state">State:</label>
          <input
            value={state}
            onChange={(e) => setState(e.target.value)}
            name="state"
            id="state"
          />
        </div>

        <div className="control">
          <label htmlFor="country">Country:</label>
          <input
            value={country}
            onChange={(e) => setCountry(e.target.value)}
            name="country"
            id="country"
          />
        </div>

        <div className="buttons">
          <button type="submit" disabled={loading}>
            Add
          </button>

          <button type="button" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}

export default CityForm
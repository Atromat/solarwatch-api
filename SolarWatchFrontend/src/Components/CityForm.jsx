import React from 'react';
import { useState, useEffect } from 'react';

function CityForm({url, pickedCity}) {
  const [name, setName] = useState("");
  const [latitude, setLatitude] = useState("");
  const [longitude, setLongitude] = useState("");
  const [state, setState] = useState("");
  const [country, setCountry] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (pickedCity !== undefined) {
      setName(pickedCity.name ?? "");
      setLatitude(pickedCity.latitude ?? "");
      setLongitude(pickedCity.longitude ?? "");
      setState(pickedCity.state ?? "");
      setCountry(pickedCity.country ?? "");
    }
  }, [pickedCity])
  

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
      setLoading(false);
      throw error
    }
  }

  async function fetchUpdateCity() {
    try {
      const res = await fetch(`${url}SolarWatch/UpdateCity?name=${name}&latitude=${latitude}&longitude=${longitude}&state=${state}&country=${country}`, {
        method: "PATCH",
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
      setLoading(false);
      throw error
    }
  }

  async function fetchDeleteCity() {
    try {
      const res = await fetch(`${url}SolarWatch/DeleteCity?name=${name}`, {
        method: "DELETE",
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
      setLoading(false);
      throw error
    }
  }

  async function onSubmit(e) {
    e.preventDefault();

    setLoading(true);

    if (e.nativeEvent.submitter.name === "AddCity") {
      await fetchPostCity();
    } else if (e.nativeEvent.submitter.name === "UpdateCity") {
      await fetchUpdateCity();
    } else if (e.nativeEvent.submitter.name === "DeleteCity") {
      await fetchDeleteCity();
    }

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
          <button type="submit" disabled={loading} name="AddCity">
            Add
          </button>

          <button type="submit" disabled={loading} name="UpdateCity">
            Update
          </button>

          <button type="submit" disabled={loading} name="DeleteCity">
            Delete
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
import React from 'react';
import { useState } from "react";
import { useNavigate } from "react-router-dom";

function Login({url, handleUserRole}) {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  async function fetchRole() {
    try{
      const res = await fetch(`${url}SolarWatch/GetRole`, {
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
  };

  const handleSubmit = async e => {
    e.preventDefault();
    try{
      const res = await fetch(`${url}Auth/Login`, {
        method: "POST",
        credentials: 'include',
        headers: { 
          "Content-Type": "application/json",
           },
        body: JSON.stringify({email: email, password: password}),
      });
      
      if (!res.ok) {
        throw new Error(`HTTP error! status: ${res.status}`);
      }

      handleUserRole(await fetchRole());

      navigate('/');
    }
    catch(error){
      throw error;
    }
  };

  return (
    <div>
      <h1>Login</h1>
      <form className="LoginForm" onSubmit={handleSubmit}>
        <div className="control">
          <label htmlFor="email">E-mail:</label>
          <input
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            name="email"
            id="email"
          />
        </div>

        <div className="control">
          <label htmlFor="password">Password:</label>
          <input
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            name="password"
            id="password"
          />
        </div>

        <div className="buttons">
          <button type="submit" disabled={loading}>
            Login
          </button>

          <button type="button" onClick={() => navigate("/")} disabled={loading}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}

export default Login
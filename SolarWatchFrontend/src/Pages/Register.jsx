import React from 'react';
import { useState } from "react";
import { useNavigate } from "react-router-dom";

function Register({url}) {
  const navigate = useNavigate();
  const [loading, setLoading] = useState(false);
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async e => {
    e.preventDefault();
    try{
      const res = await fetch(`${url}Auth/Register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({email: email, username: userName, password: password}),
      });
      if (!res.ok) {
        throw new Error(`HTTP error! status: ${res.status}`);
      }
      navigate('/');
    }
    catch(error){
      throw error;
    }
  };

  return (
    <div>
      <h1>Register</h1>
      <form className="RegistrationForm" onSubmit={handleSubmit}>
        <div className="control">
          <label htmlFor="name">Name:</label>
          <input
            value={userName}
            onChange={(e) => setUserName(e.target.value)}
            name="name"
            id="name"
          />
        </div>

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
            Register
          </button>

          <button type="button" onClick={() => navigate("/")} disabled={loading}>
            Cancel
          </button>
        </div>
      </form>
    </div>
  )
}

export default Register
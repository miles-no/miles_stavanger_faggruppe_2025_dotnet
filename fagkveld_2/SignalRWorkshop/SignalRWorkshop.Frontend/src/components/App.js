import { useEffect, useState } from "react";
import "./App.css";

function App() {
  const [forecasts, setForecasts] = useState([]);

  const requestWeather = async () => {
    const weather = await fetch("api/weatherforecast");
    console.log(weather);

    const weatherJson = await weather.json();
    console.log(weatherJson);

    setForecasts(weatherJson);
  };

  useEffect(() => {
    requestWeather();
  }, []);

  const [message, setMessage] = useState("");

  const sendMessage = async () => {
    await fetch("api/sendmessage", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ message }),
    });
  };

  return (
    <div className="App">
      <header className="App-header">
        <h1>SignalR Workshop</h1>
        <textarea
          value={message}
          onChange={(e) => setMessage(e.target.value)}
          rows="10"
          cols="50"
        ></textarea>
        <button onClick={sendMessage}>Send Message</button>
      </header>
    </div>
  );
}

export default App;

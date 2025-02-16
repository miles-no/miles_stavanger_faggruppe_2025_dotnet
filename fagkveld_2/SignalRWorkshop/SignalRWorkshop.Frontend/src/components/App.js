import { useEffect, useState } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import "./App.css";

function App() {
  const [forecasts, setForecasts] = useState([]);
  const [message, setMessage] = useState("");
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl("/chat")
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    newConnection.start()
      .then(() => {
        console.log("Connected to SignalR hub");
      })
      .catch((err) => console.log("Error connecting to SignalR hub: ", err));
  }, []);

  const sendMessage = async () => {
    if (connection) {
      try {
        await connection.invoke("SendMessage", message);
        console.log("Message sent: ", message);
      } catch (err) {
        console.error("Error sending message: ", err);
      }
    }
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
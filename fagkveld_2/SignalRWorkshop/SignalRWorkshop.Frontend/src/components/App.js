import { useEffect, useState } from "react";
import { HubConnectionBuilder, HubConnectionState } from "@microsoft/signalr";
import "./App.css";

function App() {
  const [message, setMessage] = useState("");
  const [receiveMessage, setReceiveMessage] = useState("Initial message");
  const [connection, setConnection] = useState(null);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl("https://localhost:7094/chathub", {
        withCredentials: false
      })
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);

    newConnection.start()
    .then(() => {
      console.log("Connected to SignalR hub");
      connection.on("ReceiveMessage", (msg) => {
        console.log("Message received: ", msg);
        setReceiveMessage(msg);
      });
    })
    .catch((err) => console.log("Error connecting to SignalR hub: ", err));
  }, []);
  
  const sendMessage = async () => {
    await fetch(`/api/chat?message=${message}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
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
        <p>{receiveMessage}</p>
      </header>
    </div>
  );
}

export default App;
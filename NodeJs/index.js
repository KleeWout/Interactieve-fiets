const WebSocket = require("ws");
const readline = require("readline");
const wss = new WebSocket.Server({ port: 8080 }, () => {
  console.log("server started");
});

wss.on("connection", (ws) => {
  ws.on("message", (data) => {
    // Convert Buffer to string
    const message = data.toString();
    console.log("data received:", message);
    ws.send(message);
  });
});

wss.on("listening", () => {
  console.log("server is listening on port 8080");
});
const rl = readline.createInterface({
  input: process.stdin,
  output: process.stdout,
});

wss.on("close", function () {
  console.log("I lost a client");
});

rl.on("line", (input) => {
  wss.clients.forEach((client) => {
    if (client.readyState === WebSocket.OPEN) {
      client.send(input);
    }
  });
});

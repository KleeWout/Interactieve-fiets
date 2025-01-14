const WebSocket = require("ws");
const express = require("express");
const app = express();
const path = require("path");
const readline = require("readline");

app.use("/", express.static(path.resolve(__dirname, "../webpagina")));

const myServer = app.listen(80); //http server
const wss = new WebSocket.Server({ port: 8080 }, () => {
  console.log("server started");
});

wss.on("connection", function (ws) {
  //client connects
  console.log("A new client connected!");
  ws.on("message", function (msg) {
    wss.clients.forEach(function each(client) {
      if (client.readyState === WebSocket.OPEN) {//check if client is ready
        //send message to all clients
        client.send(msg.toString());
        
      }
    });
  });

  // ws.on("message", (data) => {
  //   // Convert Buffer to string
  //   const message = data.toString();
  //   console.log("data received:", message);

  //   // Broadcast to all clients
    wss.clients.forEach((client) => {
      if (client.readyState === WebSocket.OPEN) {
        client.send(message);
      }
    });
  // });
});

myServer.on("upgrade", async function upgrade(request, socket, head) {
  //handling upgrade(http to websocekt) event
  // accepts half requests and rejects half. Reload browser page in case of rejection

  if (Math.random() > 0.5) {
    return socket.end("HTTP/1.1 401 Unauthorized\r\n", "ascii"); //proper connection close in case of rejection
  }

  //emit connection when request accepted
  wss.handleUpgrade(request, socket, head, function done(ws) {
    wss.emit("connection", ws, request);
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

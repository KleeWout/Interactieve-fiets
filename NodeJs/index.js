const WebSocket = require("ws");
const express = require("express");
const app = express();
const path = require("path");
const readline = require("readline");
const { Console } = require("console");

app.use("/", express.static(path.resolve(__dirname, "../webpagina")));

const myServer = app.listen(80); //http server
const wss = new WebSocket.Server({ port: 8080 }, () => {
  console.log("server started");
});


let gameClients = new Map();
let activeGameCodes = [];

let game2browser = new Map();
let browser2game = new Map();

wss.on("connection", function (ws) {

  ws.on("message", function (msg) {

    // serialize incoming message
    let message;
    try {
      message = JSON.parse(msg.toString());
    } catch (e) {
      console.error("Invalid JSON received:", msg.toString());
      return;
    }

    // check for game clients a stuur game code om spel te starten
    if (message.IsGameClient === true) {

      // generate game code number
      do {
        gameCode = Math.floor(100000 + Math.random() * 900000);
      } while (activeGameCodes.includes(gameCode));      
      gameCode = gameCode.toString();
      activeGameCodes.push(gameCode);

      gameClients.set(gameCode, ws);
      console.log("Active game codes: ", activeGameCodes);

      ws.send(JSON.stringify({ gameCode }));
    }
    else{
      // Handle browser client joining a game
      const gameCode = message.gameCode;
      if (activeGameCodes.includes(gameCode)) {
        if(game2browser.has(gameClients.get(gameCode))){
          console.log("Browser client already connected to game");
        }
        else{
          game2browser.set(gameClients.get(gameCode), ws);
          browser2game.set(ws, gameClients.get(gameCode));

          console.log("Browser client joined game with code: ", gameCode);
        }
      }
      else{
        console.log("Invalid game code");
      }
        

      //   if (gameCodeToBrowserClient.has(gameCode)) {
      //     ws.send(JSON.stringify({ success: false, error: "Game already has a browser client" }));
      //   } else {
      //     browserClients.set(ws, gameCode);
      //     gameCodeToBrowserClient.set(gameCode, ws);
      //     console.log(`Browser client joined game with code: ${gameCode}`);
      //     ws.send(JSON.stringify({ success: true, gameCode }));
      //   }
      // } else {
      //   ws.send(JSON.stringify({ success: false, error: "Invalid game code" }));
    }



  });

  ws.on("close", function () {
    if (getGameClientFromGameCode(ws) !== null) {
      activeGameCodes = activeGameCodes.filter((code) => code !== getGameClientFromGameCode(ws));
      gameClients.delete(ws);
      console.log("Game client disconnected");
      console.log("Active game codes: ", activeGameCodes);
    }
    else if(browser2game.has(ws)){
      game2browser.delete(browser2game.get(ws));
      browser2game.delete(ws);
      console.log("Browser client disconnected");
    }


  });

});

function getGameClientFromGameCode(ws) {
  for (let [gameCode, clientWs] of gameClients.entries()) {
    if (clientWs === ws) {
      return gameCode;
    }
  }
  return null;
}

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



rl.on("line", (input) => {
  wss.clients.forEach((client) => {
    if (client.readyState === WebSocket.OPEN) {
      client.send(input);
    }
  });
});

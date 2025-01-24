// todo
// clear gamescore
// clear gamestatus

const WebSocket = require("ws");
const express = require("express");
const QRCode = require('qrcode');
const app = express();
const path = require("path");
const readline = require("readline");
const os = require("os");


// Serve static files from 'public' directory
app.use(express.static(path.join(__dirname, '../docs')));

const myServer = app.listen(80); //http server
const port = 3000;
app.listen(port, () => {
  console.log(`Server is running at http://localhost:${port}`);
});


const wss = new WebSocket.Server({ port: 8080 }, () => {
  console.log("server started");
});


let gameClients = new Map();
let activeGameCodes = [];

let game2browser = new Map();
let browser2game = new Map();

let gameStatus = new Map();
let gameScore = new Map();

function getIPAddress() {
  const interfaces = os.networkInterfaces();
  for (const name of Object.keys(interfaces)) {
    for (const iface of interfaces[name]) {
      if (iface.family === 'IPv4' && !iface.internal) {
        return iface.address;
      }
    }
  }
  return '127.0.0.1';
}

app.get('/generateQR', async (req, res) => {
  try {
    const url = req.query.url || `http://${getIPAddress()}:3000`;
    const code = req.query.code || 'none';
    const qrCodeImage = await QRCode.toDataURL(`${url}?code=${code}`, {
      color: {
        dark: "#261516",
        light: "#0000"
      },
      margin: 2
    });

    const imgBuffer = Buffer.from(qrCodeImage.split(",")[1], 'base64');
    res.writeHead(200, {
      'Content-Type': 'image/png',
      'Content-Length': imgBuffer.length
    });
    res.end(imgBuffer);
  } catch (err) {
    console.error('Error generating QR code:', err);
    res.status(500).send('Internal Server Error');
  }
});

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

      if(message.NewConnection === true){
        // generate game code number
        ws.send(JSON.stringify({connectionStatus: "disconnected"}));
        activeGameCodes = activeGameCodes.filter((code) => code !== getGameCodeFromGameClient(ws));
        gameClients.delete(getGameCodeFromGameClient(ws));
        gameScore.delete(ws);
        gameStatus.delete(ws);
        do {
          gameCode = Math.floor(1000 + Math.random() * 9000);
        } while (activeGameCodes.includes(gameCode));      
        gameCode = gameCode.toString();
        activeGameCodes.push(gameCode);

        gameClients.set(gameCode, ws);
        console.log("Active game codes: ", activeGameCodes);

        browser2game.delete(game2browser.get(ws));
        game2browser.delete(ws);

        ws.send(JSON.stringify({ gameCode }));
      }
      // else if(message.ReNewConnection === true){

      // }
      else if (game2browser.has(ws)){
        const strippedMessage = { ...message };
        delete strippedMessage.IsGameClient;
        delete strippedMessage.NewConnection;
        console.log(message)
        if(strippedMessage.Score){
          gameScore.set(ws, strippedMessage.Score);
        }
        game2browser.get(ws).send(JSON.stringify(strippedMessage));
      }
  
      else{
        console.log("Game client not associated with any browser client");
      }

    }
    else if (message.gameState) {
      if (browser2game.has(ws)) {
        if(message.gameState === "start"){
          gameStatus.set(browser2game.get(ws), "start");
          browser2game.get(ws).send(JSON.stringify({gameState: "start", gameMode: message.gameMode}));
          gameScore.set(browser2game.get(ws), 0);
        }
        else if (message.gameState === "stop"){
          gameStatus.set(browser2game.get(ws), "stop");
          browser2game.get(ws).send(JSON.stringify({gameState: "stop"}));
        }
        else if(message.gameState === "restart"){
          if(game2browser.has(browser2game.get(ws))){
            browser2game.get(ws).send(JSON.stringify({gameState: "restart"}));
          }
        }
      }
    }
    else{
      // Handle browser client joining a game
      if (message.hasOwnProperty('gameCode')){
        const gameCode = message.gameCode;
        if (activeGameCodes.includes(gameCode)) {
          if(game2browser.has(gameClients.get(gameCode))){
            console.log("Browser client already connected to game");
            ws.send(JSON.stringify({connectionStatus: "busy", id: message.id}));
          }
          else{
            game2browser.set(gameClients.get(gameCode), ws);
            browser2game.set(ws, gameClients.get(gameCode));
            // make a reconnect option 
            // if game was already started, give reconnect instead of connect
            browser2game.get(ws).send(JSON.stringify({connectionStatus: "connected", userName: message.userName, gameMode: message.gameMode}));
  
            // console.log("Browser client joined game with code: ", gameCode);
            // handle reconnect here (check if game was already started with gameStatus)
            if(gameStatus.get(gameClients.get(gameCode)) == "start"){
              ws.send(JSON.stringify({connectionStatus: "reconnect-start", id: message.id, score: gameScore.get(gameClients.get(gameCode))}));
            }
            else{
              ws.send(JSON.stringify({connectionStatus: "success", id: message.id}));
            }
          }
        }
        else{
          // console.log("Invalid game code");
          ws.send(JSON.stringify({connectionStatus: "failed", id: message.id}));
        }
      }
      else{
        if (browser2game.has(ws)) {
          browser2game.get(ws).send(JSON.stringify(message));
          // console.log(JSON.stringify(message));
        } else {
          console.log("Browser client not associated with any game");
        }
      }
    }



  });

  ws.on("close", function () {
    if (getGameCodeFromGameClient(ws) !== null) {
      activeGameCodes = activeGameCodes.filter((code) => code !== getGameCodeFromGameClient(ws));
      gameClients.delete(getGameCodeFromGameClient(ws));
      console.log("Game client disconnected");
      console.log("Active game codes: ", activeGameCodes);
    }
    else if(browser2game.has(ws)){
      if(gameStatus.get(browser2game.get(ws)) == "start"){
        browser2game.get(ws).send(JSON.stringify({connectionStatus: "idle"}));
      }
      else{
        browser2game.get(ws).send(JSON.stringify({connectionStatus: "disconnected"}));
      }
      game2browser.delete(browser2game.get(ws));
      browser2game.delete(ws);
      console.log("Browser client disconnected");
    }
  });

});

function getGameCodeFromGameClient(ws) {
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
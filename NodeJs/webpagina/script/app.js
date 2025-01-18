//to do:
// input voor naam --> width dynamisch aanpassen gebaseerd op lengte van naam

"use strict";
let htmlSingleplayer, htmlMultiplayer, htmlMain, messagesDiv, htmlscore, htmlButtons, htmlStop;
const lanIP = `${window.location.hostname}:8080`;
const ws = new WebSocket(`ws://${lanIP}`);

ws.onopen = () => {
  console.log("Connected to WebSocket server");
};
ws.onmessage = (event) => {
  const message = event.data;
  console.log("Raw message received:", message);
  try {
    // Parse JSON data
    const jsonData = JSON.parse(message);
    console.log("Parsed JSON data:", jsonData);
    // Check if it has score property
    if (jsonData.score !== undefined) {
      htmlscore.innerHTML = `<p>Current Score: ${jsonData.score}</p>`;
    }
    if (jsonData.gameState == "started") {
      htmlButtons.innerHTML = `<button class="button js-stop">Stop</button>`;
      htmlStop = document.querySelector(".js-stop");
      htmlStop.addEventListener("click", function () {
        ws.send('{"gameState": "stop"}');
      });
    }
    if (jsonData.gameState == "stopped") {
      htmlButtons.innerHTML = `<button class="js-singleplayer">Singleplayer</button>
        <button class="js-multiplayer">Multiplayer</button>
        <button class="js-main">Main</button>`;
    }
  } catch (error) {
    console.error("Error parsing JSON:", error);
  }
};

ws.onerror = (error) => {
  console.error("WebSocket error:", error);
};

const listenToButtons = function () {
  htmlSingleplayer.addEventListener("click", function () {
    console.log("singleplayer");
    ws.send('{"gamemode": "singleplayer"}');
  });
  htmlMultiplayer.addEventListener("click", function () {
    console.log("multiplayer");
    ws.send('{"gamemode": "multiplayer"}');
  });
  htmlMain.addEventListener("click", function () {
    console.log("main");
    ws.send('{"gamemode": "main"}');
  });
};

const init = function () {
  console.log("DOM loaded");
  htmlSingleplayer = document.querySelector(".js-singleplayer");
  htmlMultiplayer = document.querySelector(".js-multiplayer");
  htmlMain = document.querySelector(".js-main");
  messagesDiv = document.querySelector(".js-messages");
  htmlscore = document.querySelector(".js-score");
  htmlButtons = document.querySelector(".js-buttons");
  listenToButtons();
};

document.addEventListener("DOMContentLoaded", init);

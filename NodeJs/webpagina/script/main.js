"use strict";
let htmlSingleplayer, htmlMultiplayer, htmlMain, messagesDiv, htmlscore;
const lanIP = `${window.location.hostname}:8080`;
const ws = new WebSocket(`ws://${lanIP}`);

ws.onopen = () => {
  console.log("Connected to WebSocket server");
};

ws.onmessage = (event) => {
  const message = event.data;
  console.log("Message received:", message);
  console.log(messagesDiv);
  if (message.startsWith("Recieved")) {
    console.log("joined message received");
    messagesDiv.innerHTML = `<p>Joined: ${message}</p>`;
  }
  messagesDiv.innerHTML = `<p>Received: ${message}</p>`;
if (message.startsWith('score')) {
    console.log("Score message received");
    htmlscore.innerHTML = `<p>Current Score: ${message}</p>`;
}
};

ws.onerror = (error) => {
  console.error("WebSocket error:", error);
};

const listenToButtons = function () {
  htmlSingleplayer.addEventListener("click", function () {
    console.log("singleplayer");
    ws.send("singleplayer");
  });
  htmlMultiplayer.addEventListener("click", function () {
    console.log("multiplayer");
    ws.send("multiplayer");
  });
  htmlMain.addEventListener("click", function () {
    console.log("main");
    ws.send("main");
  });
};

const init = function () {
  console.log("DOM loaded");
  htmlSingleplayer = document.querySelector(".js-singleplayer");
  htmlMultiplayer = document.querySelector(".js-multiplayer");
  htmlMain = document.querySelector(".js-main");
  messagesDiv = document.querySelector(".js-messages");
  htmlscore = document.querySelector(".js-score");
  //   listenToSocket();
  listenToButtons();
};

document.addEventListener("DOMContentLoaded", init);

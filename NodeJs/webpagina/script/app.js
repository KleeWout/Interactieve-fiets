"use strict";
let htmlSingleplayerButton, htmlMultiplayerButton, htmlScoreValue;
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
      htmlScoreValue.innerHTML = `<p>Current Score: ${jsonData.score}</p>`;
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
  htmlSingleplayerButton.addEventListener("click", function () {
    ws.send('{"gameMode": "singleplayer"}');
  });
  htmlMultiplayerButton.addEventListener("click", function () {
    ws.send('{"gameMode": "multiplayer"}');
  });
};

const joinGameConnection = async function (gameCode) {
  let name = await getRandomName();

  // Basic validation: check if gameCode is a 6-digit number
  if (/^\d{4}$/.test(gameCode)) {
    if (ws.readyState === WebSocket.OPEN) {
      ws.send('{"gameCode": "' + gameCode + '", "userName": "' + name + '"}');
    } else {
      ws.addEventListener('open', function () {
        ws.send('{"gameCode": "' + gameCode + '", "userName": "' + name + '"}');
      });
    }
  } else {
    console.error("Invalid game code:", gameCode);
  }

  return name;
}
const changeUserName = function(name){
  ws.send('{"userName": "' + name + '"}');
}
const getRandomName = async function () {
  try {
    const response = await fetch('script/names.json');
    const data = await response.json();
    const names = data.names;
    const randomIndex = Math.floor(Math.random() * names.length);
    return names[randomIndex];
  } catch (error) {
    console.error("Error fetching names:", error);
    return "PeddelPiraat";
  }
};

function validateAndMove(current, nextFieldID) {
  current.value = current.value.replace(/[^0-9]/g, ''); // Remove non-numeric characters
  if (current.value.length >= current.maxLength && nextFieldID) {
      document.getElementById(nextFieldID).focus();
  }
  if(current === document.getElementById('input4')){
    getCombinedInput();
  }
}

function moveToPrevious(event, previousFieldID) {
  if (event.key === 'Backspace' && event.target.value === '') {
      document.getElementById(previousFieldID).focus();
  }
}
function getCombinedInput() {
  const input1 = document.getElementById('input1');
  const input2 = document.getElementById('input2');
  const input3 = document.getElementById('input3');
  const input4 = document.getElementById('input4');
  const combinedInput = input1.value + input2.value + input3.value + input4.value;
  joinGameConnection(combinedInput);
  input1.value = "";
  input2.value = "";
  input3.value = "";
  input4.value = "";
}


const init = function () {
  console.log("DOM loaded");


  htmlSingleplayerButton = document.querySelector(".js-singleplayer");
  htmlMultiplayerButton = document.querySelector(".js-multiplayer");
  htmlScoreValue = document.querySelector(".js-score");

  listenToButtons();

  joinGameConnection();




};

document.addEventListener("DOMContentLoaded", init);

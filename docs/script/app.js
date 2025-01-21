"use strict";
let htmlGameMode, htmlScoreValue, htmlName, touchArea;
const lanIP = `${window.location.hostname}:8080`;
const ws = new WebSocket(`ws://${lanIP}`);

ws.onopen = () => {
  console.log("Connected to WebSocket server");
};
ws.onmessage = (event) => {
  console.log("Data received from server:", event.data);
  const message = event.data;
  try {
    const jsonData = JSON.parse(message);
    if (jsonData.Score !== undefined) {
      htmlScoreValue.innerHTML = `${jsonData.Score}m</p>`;
    }
  } catch (error) {
    console.error("Error parsing JSON:", error);
  }
};

ws.onerror = (error) => {
  console.error("WebSocket error:", error);
};

const listenToInputs = function () {
  console.log("Listening to inputs");
  htmlName.addEventListener("input", function () {
    ws.send('{"userName": "' + htmlName.value + '"}');
  });
  htmlGameMode.forEach(radio => {
    radio.addEventListener('change', () => {
      const selectedOption = document.querySelector('input[name="gameMode"]:checked').value;
      if (selectedOption == "Singleplayer") {
        console.log("Singleplayer");
        ws.send('{"gameMode": "singleplayer"}');
      }
      if (selectedOption === "Multiplayer") {
        console.log("Multiplayer");
        ws.send('{"gameMode": "multiplayer"}');
      }
    });
  });
};

const joinGameConnection = async function (gameCode) {
  let name = await getRandomName();


  const requestId = generateUniqueId();
  const request = {
      id: requestId,
      gameCode: gameCode,
      userName: name
  };

  if (ws.readyState === WebSocket.OPEN) {
    ws.send(JSON.stringify(request));
  } else {
    ws.addEventListener('open', function () {
      ws.send(JSON.stringify(request));
    });
  }

  ws.addEventListener('message', function(event) {
    const response = JSON.parse(event.data);
    if (response.id === requestId) {
      if (response.connectionStatus === "failed") { 
        console.log("Invalid game code");
      }
      else if(response.connectionStatus === "busy"){
        console.log("Game already has a browser client");
      }
      else if (response.connectionStatus === "success"){
        htmlName.value = name;
        document.querySelector('.c-inputname__widthbox').value = name;

        document.querySelector(".c-main").classList.remove('blurred');
        document.querySelector(".c-buttons").classList.remove('blurred');
        document.querySelector(".c-entry").classList.add('hidden');



        console.log("Name set to:", name);
      }
    }
  }, { once: true });
}

function generateUniqueId() {
  return Math.random().toString(36).substr(2, 9);
}

const changeUserName = function(name) {
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

const validateAndMove = function(current, nextFieldID) {
  current.value = current.value.replace(/[^0-9]/g, ''); // Remove non-numeric characters
  if (current.value.length >= current.maxLength && nextFieldID) {
      document.getElementById(nextFieldID).focus();
  }
  if(current === document.getElementById('input4')){
    getCombinedInput();
  }
}

const moveToPrevious = function(event, previousFieldID) {
  if (event.key === 'Backspace' && event.target.value === '') {
      document.getElementById(previousFieldID).focus();
  }
}

const getCombinedInput = async function() {
  const input1 = document.getElementById('input1');
  const input2 = document.getElementById('input2');
  const input3 = document.getElementById('input3');
  const input4 = document.getElementById('input4');
  const combinedInput = input1.value + input2.value + input3.value + input4.value;
  await joinGameConnection(combinedInput);
  input1.value = "";
  input2.value = "";
  input3.value = "";
  input4.value = "";
}




let touchStartX = 0;
let touchStartY = 0;
let touchEndX = 0;
let touchEndY = 0;

const handleTouchStart = (event) => {
  touchStartX = event.touches[0].clientX;
  touchStartY = event.touches[0].clientY;
};

const handleTouchMove = (event) => {
  touchEndX = event.touches[0].clientX;
  touchEndY = event.touches[0].clientY;
};

const handleTouchEnd = () => {
  const deltaX = touchEndX - touchStartX;
  const deltaY = touchEndY - touchStartY;

  if (Math.abs(deltaX) > Math.abs(deltaY)) {
    if (deltaX > 0) {
      if (htmlGameMode[1].checked){
        htmlGameMode[0].checked = true;
        htmlGameMode[0].dispatchEvent(new Event('change'));
      }
    } 
    else {
      if(htmlGameMode[0].checked){
        htmlGameMode[1].checked = true;
        htmlGameMode[0].dispatchEvent(new Event('change'));
      }
    }
  }
};

document.addEventListener('DOMContentLoaded', function() {
  const input = document.querySelector('.c-namecontainer__input');
  const width = document.querySelector('.c-inputname__widthbox');

  function adjustWidth() {
    const inputValue = input.value;
    width.textContent = inputValue; // Set the widthBox text to the input value
    input.style.width = (width.offsetWidth + 1) + 'px'; // Set the input width to the widthBox width
  }

  input.addEventListener('input', adjustWidth);
  adjustWidth(); // Initial call to set the width based on the initial value
  input.addEventListener('keydown', function(event) {
    if (event.key === ' ') {
      event.preventDefault(); // Prevent space character
    }
  });
});

const init = function () {

  htmlScoreValue = document.querySelector(".js-score");
  htmlName = document.querySelector(".js-name");
  htmlGameMode = document.querySelectorAll('input[name="gameMode"]');

  touchArea = document.querySelector(".c-gamemode__container");
  touchArea.addEventListener("touchstart", handleTouchStart);
  touchArea.addEventListener("touchmove", handleTouchMove);
  touchArea.addEventListener("touchend", handleTouchEnd);

  listenToInputs();
};

document.addEventListener("DOMContentLoaded", init);
"use strict";
let htmlGameMode, htmlScoreValue, htmlName, htmlNameBox, htmlStartButton, touchArea, touchAreaHelp, htmlHelpScreens;
let htmlStopButton, htmlEndScoreMenuButton, htmlLeaderboardReturnButton, htmlLeaderboardButton, htmlLeaderBoardPage, htmlLeaderBoardList, htmlEndGameName, htmlEndGameScore, htmlHelpButton, htmlHelpReturnButton;
let htmlPencilButton;
const lanIP = `${window.location.hostname}:8080`;
const ws = new WebSocket(`ws://${lanIP}`);
let clientId;

const urlParams = new URLSearchParams(window.location.search);
const code = urlParams.get("code");
if (!urlParams.has("clientid")) {
  clientId = generateUniqueId();
  const url = new URL(window.location);
  url.searchParams.set("clientid", clientId.toString());
  window.history.pushState({}, "", url);
} else {
  clientId = urlParams.get("clientid");
}

ws.onopen = () => {
  console.log("Connected to WebSocket server");
  joinGameConnection(code || "0000");
};
ws.onmessage = (event) => {
  console.log("Data received from server:", event.data);
  const message = event.data;
  try {
    const jsonData = JSON.parse(message);
    if (jsonData.Score !== undefined) {
      htmlScoreValue.forEach((element) => {
        element.innerHTML = `${jsonData.Score}m</p>`;
      });
    }
    if (jsonData.GameOver !== undefined) {
      if (jsonData.GameOver == "true") {
        console.log("Game over");
        document.querySelector(".c-boat").classList.remove("activated");
        document.querySelector(".c-boat").classList.add("deactivated");
        document.querySelector(".c-endgame__fixed").classList.add("activated");
      }
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
  htmlName.addEventListener("click", function () {
    htmlName.select();
  });
  htmlName.addEventListener("input", function () {
    ws.send('{"userName": "' + htmlName.value + '"}');
  });
  htmlGameMode.forEach((radio) => {
    radio.addEventListener("change", () => {
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
  htmlName.addEventListener("input", adjustWidth);
  htmlName.addEventListener("keydown", function (event) {
    if (event.key === " ") {
      event.preventDefault();
    }
  });
  htmlStartButton.addEventListener("click", function () {
    // ws.send(`{"gameState": "start", "gameMode": ${htmlGameMode[0].checked ? `"singleplayer"` : `"multiplayer"`}}`);
    ws.send(`{"gameState": "start"}`);
    console.log("Start button clicked");
    document.querySelector(".c-home").classList.add("hide");
    document.querySelector(".c-endgame__fixed").classList.remove("deactivated");
    document.querySelector(".c-boat").classList.add("activated");
  });

  htmlStopButton.addEventListener("click", function () {
    ws.send('{"gameState": "stop"}');
    document.querySelector(".c-boat").classList.remove("activated");
    document.querySelector(".c-boat").classList.add("deactivated");
    document.querySelector(".c-endgame__fixed").classList.add("activated");
  });

  htmlEndScoreMenuButton = document.querySelector(".js-endscore-menu");
  htmlEndScoreMenuButton.addEventListener("click", function () {
    ws.send('{"gameState": "restart"}');
    window.location.reload();
  });
  htmlLeaderboardButton.addEventListener("click", function () {
    window.location.href = "/leaderboard";
  });

  htmlHelpButton.addEventListener("click", function () {
    document.querySelector(".c-help__fixed").classList.add("activated");
  });

  htmlHelpReturnButton.addEventListener("click", function () {
    document.querySelector(".c-help__fixed").classList.remove("activated");
  });

  htmlPencilButton.addEventListener("click", function () {
    htmlName.focus();
    htmlName.select();
  });
};

const listenToLeaderBoard = function () {
  htmlLeaderboardButton = document.querySelector(".js-leaderboard__return");
  htmlLeaderboardButton.addEventListener("click", function () {
    window.location.href = "/index.html";
  });
};

const joinGameConnection = async function (gameCode) {
  let name = await getRandomName();

  const request = {
    id: clientId,
    gameCode: gameCode,
    userName: name,
    gameMode: htmlGameMode[0].checked ? "singleplayer" : "multiplayer",
  };

  if (ws.readyState === WebSocket.OPEN) {
    ws.send(JSON.stringify(request));
  } else {
    ws.addEventListener("open", function () {
      ws.send(JSON.stringify(request));
    });
  }

  ws.addEventListener(
    "message",
    function (event) {
      const response = JSON.parse(event.data);
      if (response.id === clientId) {
        if (response.connectionStatus === "failed") {
          console.log("Invalid game code");
        } else if (response.connectionStatus === "busy") {
          console.log("Game already has a browser client");
        } else if (response.connectionStatus === "reconnect-start") {
          document.querySelector(".c-main").classList.remove("blurred");
          document.querySelector(".c-buttons").classList.remove("blurred");
          document.querySelector(".c-entry").classList.add("hidden");
          document.querySelector(".c-home").classList.add("hide");
          document.querySelector(".c-endgame__fixed").classList.remove("deactivated");
          document.querySelector(".c-boat").classList.add("activated");
          htmlScoreValue.forEach((element) => {
            element.innerHTML = `${response.score}m</p>`;
          });
        } else if (response.connectionStatus === "success") {
          document.querySelector(".c-inputname__widthbox").value = name;
          htmlName.value = name;
          htmlEndGameName.innerHTML = name;
          adjustWidth();

          document.querySelector(".c-main").classList.remove("blurred");
          document.querySelector(".c-buttons").classList.remove("blurred");
          document.querySelector(".c-entry").classList.add("hidden");

          console.log("Name set to:", name);
        }
      }
    },
    { once: true }
  );
};

function generateUniqueId() {
  return Math.random().toString(36).substr(2, 9);
}

const changeUserName = function (name) {
  ws.send('{"userName": "' + name + '"}');
};

const getRandomName = async function () {
  try {
    const response = await fetch("script/names.json");
    const data = await response.json();
    const names = data.names;
    const randomIndex = Math.floor(Math.random() * names.length);
    return names[randomIndex];
  } catch (error) {
    console.error("Error fetching names:", error);
    return "PeddelPiraat";
  }
};

const validateAndMove = function (current, nextFieldID) {
  current.value = current.value.replace(/[^0-9]/g, ""); // Remove non-numeric characters
  if (current.value.length >= current.maxLength && nextFieldID) {
    document.getElementById(nextFieldID).focus();
  }
  if (current === document.getElementById("input4")) {
    getCombinedInput();
  }
};

const moveToPrevious = function (event, previousFieldID) {
  if (event.key === "Backspace" && event.target.value === "") {
    document.getElementById(previousFieldID).focus();
  }
};

const getCombinedInput = async function () {
  const input1 = document.getElementById("input1");
  const input2 = document.getElementById("input2");
  const input3 = document.getElementById("input3");
  const input4 = document.getElementById("input4");
  const combinedInput = input1.value + input2.value + input3.value + input4.value;
  await joinGameConnection(combinedInput);
  const url = new URL(window.location);
  url.searchParams.set("code", combinedInput);
  window.history.pushState({}, "", url);

  input1.value = "";
  input2.value = "";
  input3.value = "";
  input4.value = "";
};

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

const handleTouchEnd = (area) => {
  const deltaX = touchEndX - touchStartX;
  const deltaY = touchEndY - touchStartY;

  if (area === "touchAreaGameMode") {
    if (Math.abs(deltaX) > Math.abs(deltaY)) {
      if (deltaX > 0) {
        if (htmlGameMode[1].checked) {
          htmlGameMode[0].checked = true;
          htmlGameMode[0].dispatchEvent(new Event("change"));
        }
      } else {
        if (htmlGameMode[0].checked) {
          htmlGameMode[1].checked = true;
          htmlGameMode[0].dispatchEvent(new Event("change"));
        }
      }
    }
  } else if (area === "touchAreaHelp") {
    if (Math.abs(deltaX) > Math.abs(deltaY)) {
      if (deltaX > 0) {
        if (htmlHelpScreens[0].checked) {
          return;
        }
        for (let i = 0; i < htmlHelpScreens.length; i++) {
          if (htmlHelpScreens[i].checked) {
            htmlHelpScreens[i - 1].checked = true;
          }
        }
      } else {
        if (htmlHelpScreens[htmlHelpScreens.length - 1].checked) {
          return;
        }
        for (let i = 0; i < htmlHelpScreens.length; i++) {
          if (htmlHelpScreens[i].checked) {
            htmlHelpScreens[i + 1].checked = true;
            break;
          }
        }
      }
    }
  }
};

const showLeaderboard = function (jsonObject) {
  htmlLeaderBoardList = document.querySelector(".js-leaderboard__list");
  let position = 1;
  let output = "";
  console.log(jsonObject);
  for (let player of jsonObject) {
    console.log(player.name);
    output += `<li class="c-scoreboard__item">
              <div class="c-scoreboard__item--name">
                <p>${position}</p>
                <p>${player.name}</p>
              </div>
              <p class="c-scoreboard__item--score">${player.score}m</p>
            </li>`;
    position++;
  }
  htmlLeaderBoardList.innerHTML = output;
};

const adjustWidth = function () {
  const inputValue = htmlName.value;
  htmlNameBox.textContent = inputValue; // Set the widthBox text to the input value
  htmlName.style.width = htmlNameBox.offsetWidth + 1 + "px"; // Set the input width to the widthBox width
  console.log(htmlNameBox.offsetWidth);
};

const init = function () {
  htmlLeaderBoardPage = document.querySelector(".js-leaderboard");

  if (htmlLeaderBoardPage) {
    console.log("Leaderboard page");
    listenToLeaderBoard();
    // Example function to fetch JSON data from a URL
    // let url = "https://entertainendefietsgameleaderboard.azurewebsites.net/api/leaderboard";
    let url = `http://127.0.0.1:3000/api/getleaderboard`;
    handleData(url, showLeaderboard);
  } else {
    htmlScoreValue = document.querySelectorAll(".js-score");
    htmlName = document.querySelector(".js-name");
    htmlNameBox = document.querySelector(".c-inputname__widthbox");
    htmlGameMode = document.querySelectorAll('input[name="gameMode"]');
    htmlStartButton = document.querySelector(".js-start");
    htmlStopButton = document.querySelector(".js-stop");
    htmlLeaderboardButton = document.querySelector(".js-leaderboard-btn");
    htmlEndGameName = document.querySelector(".js-endGameName");
    htmlEndGameScore = document.querySelector(".js-endgame-score");
    htmlHelpScreens = document.querySelectorAll('input[name="help"]');
    htmlHelpButton = document.querySelector(".js-help-btn");
    htmlHelpReturnButton = document.querySelector(".js-help-page-return");
    htmlPencilButton = document.querySelector(".js-pencil");

    touchArea = document.querySelector(".c-gamemode__container");
    touchArea.addEventListener("touchstart", handleTouchStart);
    touchArea.addEventListener("touchmove", handleTouchMove);
    touchArea.addEventListener("touchend", (event) => handleTouchEnd("touchAreaGameMode"));

    touchAreaHelp = document.querySelector(".c-help__slider");
    touchAreaHelp.addEventListener("touchstart", handleTouchStart);
    touchAreaHelp.addEventListener("touchmove", handleTouchMove);
    touchAreaHelp.addEventListener("touchend", (event) => handleTouchEnd("touchAreaHelp"));

    listenToInputs();
  }
};

document.addEventListener("DOMContentLoaded", init);

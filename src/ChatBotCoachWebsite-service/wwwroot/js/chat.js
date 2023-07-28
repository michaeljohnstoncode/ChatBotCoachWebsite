
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

connection.on("ReceiveMessage", function (user, message) {
    // Create a new list item for the received message
    var messageListItem = document.createElement("li");

    // Check if the user is "assistant" to apply the correct CSS class
    if (user === "assistant") {
        messageListItem.classList.add("assistant-message");
    } else {
        messageListItem.classList.add("user-message");
    }

    // Set the message text content
    messageListItem.textContent = user + ": " + message;

    // Append the message to the messages list
    document.getElementById("messagesList").appendChild(messageListItem);
    var messageList = document.getElementById("messagesList");
    messageList.scrollTop = messageList.scrollHeight;
});

// Attach click event to the Send button
document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();
    sendMessage(); // Call the function to send the message
});

// When enter key is pressed, send message
document.getElementById("messageInput").addEventListener("keydown", function (event) {
    // Check if the Enter key was pressed (keyCode 13) and Shift key was not pressed
    if (event.keyCode === 13 && !event.shiftKey) {
        event.preventDefault();
        sendMessage(); // Call the function to send the message
    }
});

// Function to send the message
function sendMessage() {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;

    // Check if the message is not empty
    if (message.trim() === "") {
        return; // If empty, do not send the message
    }

    // Call the hub method to send the message
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });

    // Clear the chat input text box
    document.getElementById("messageInput").value = "";
}

connection.start().then(function () {
    console.log("SignalR connected.");
}).catch(function (err) {
    return console.error(err.toString());
});

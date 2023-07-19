
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
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    event.preventDefault();
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;


    // Call the hub method to send the message
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });

    // Clear the chat input text box
    document.getElementById("messageInput").value = "";
});

connection.start().then(function () {
    console.log("SignalR connected.");
}).catch(function (err) {
    return console.error(err.toString());
});

/*
connection.start().then(function () {
    document.getElementById("sendButton").addEventListener("click", function (event) {
        var user = document.getElementById("userInput").value;
        var message = document.getElementById("messageInput").value;
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
}).catch(function (err) {
    return console.error(err.toString());
});

connection.start().then(function () {
    console.log("SignalR connected.");
}).catch(function (err) {
    return console.error(err.toString());
});
*/
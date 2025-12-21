// SignalR Connection for Online Status
// This script connects all authenticated users to the SignalR hub
"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/online")
    .withAutomaticReconnect()
    .build();

// Start the connection
connection.start()
    .then(function () {
        console.log("SignalR Connected - User is now online");
    })
    .catch(function (err) {
        console.error("SignalR Connection Error:", err.toString());
    });

// Handle reconnection
connection.onreconnecting(function() {
    console.log("SignalR Reconnecting...");
});

connection.onreconnected(function() {
    console.log("SignalR Reconnected");
});

connection.onclose(function() {
    console.log("SignalR Connection Closed");
});

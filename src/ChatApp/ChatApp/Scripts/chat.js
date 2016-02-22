$(function () {
    var room = $('#roomId').val();
     $('.messageInput').keyup(function (e) {

        if (e.keyCode === 13) {
            $('.sendButton').trigger("click");
        }
    });
    var unseenCount = 0;
    var handleUnseen = function() {
        if (document.hidden) {
            unseenCount++;
            document.title = "Nya meddelanden (" + unseenCount + ")";
            ion.sound.play("door_bell");
        }
    }

    document.addEventListener("visibilitychange", function() {
        document.title = "Chat";
    });

    // Reference the auto-generated proxy for the hub.
    var chat = $.connection.chatHub;
    // Create a function that the hub can call back to display messages.
    chat.client.addNewMessageToPage = function (name, time, message, msgtype) {
        // Add the message to the page.
        var chatArea = $('.chatArea');
        
        chatArea.append('<div class="message  ' + msgtype + '"><div class="header">' + htmlEncode(name) + " @ " + time
             + '</div><div class="messageBody">' + htmlEncode(message) + '</div></div>');
        var body = $('body');
        $(body).scrollTop($(body).height());
        handleUnseen();
       

    };
    var domList = $('.userList');
    var userList;
    var updateUserlist = function () {
        domList.empty();
        for (var i = 0;i<userList.length;i++) {
            console.log("appending: " + userList[i]);
            domList.append('<div>' + userList[i] + ' </div>');
            
        }
    }
    chat.client.joined = function (username) {
        console.log("joining: " + username);
         userList.push(username);
         updateUserlist();
    }
    chat.client.left = function (username) {
        console.log("left: " + username);
        userList.pop(username);
        updateUserlist();

    }
   
    chat.client.currentUsers = function (userlist) {
        console.log("users: " + userlist);
        userList = userlist;
        updateUserlist();
    }
    // Get the user name and store it to prepend to messages.
    // Set initial focus to message input box.
    $('.chatbuttonInput').focus();
    // Start the connection.
    $.connection.hub.start().done(function () {
        chat.server.join(room);
        $('.sendButton').click(function () {
            // Call the Send method on the hub.
            chat.server.send($('.messageInput').val());

            // Clear text box and reset focus for next comment.
            $('.messageInput').val('').focus();

        });
    });
    $(window).unload(function () {

        chat.server.leave(room);

    });


    ion.sound({
        sounds: [
           
            { name: "door_bell" },
           
        ],

        // main config
        path: "scripts/sounds/",
        preload: true,
        multiplay: true,
        volume: 0.7
    });

    // play sound
    

});
// This optional function html-encodes messages for display in the page.
function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
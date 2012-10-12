<?php
    $host = 'localhost';
    $port = 4444;
    
    // create socket -- fail silently if can't connect
    $socket = socket_create(AF_INET, SOCK_STREAM, 0);
    if($socket) {
        socket_connect($socket, $host, $port);
        $msg = '|||' . $_SERVER['REMOTE_ADDR'] . '|WebClick';
        socket_send($socket, $msg, strlen($msg), 0);
        socket_close($socket);
    }
    
    // redirect the user so they are unaware we just flagged their click
    header("location:http://localhost");
?>
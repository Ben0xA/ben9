#!/usr/bin/python

import socket
import getpass
import time

# create socket object
xport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

# send message to server; fail quietly if server is not running
try:
    xport.connect(('localhost',4444))
    xport.send(getpass.getuser()+" clicked shit at "+time.strftime("%d/%m/%Y  %H:%M:%S",time.gmtime()))
except:
    pass

# close socket
xport.shutdown(1)
xport.close()


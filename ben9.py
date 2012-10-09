#!/usr/bin/python

import socket
import getpass

# create socket object
xport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
rslt = 0

# send message to server; fail quietly if server is not running
try:
    xport.connect(('localhost',4444))
    xport.setblocking(0)
    # export in psv time|username|ip|machine_name
    msg = getpass.getuser()
    rslt = xport.send(msg.encode())
except:
  print "failcamping"

# close socket
if rslt != 0: # only close the socket if it was open to begin with
    xport.shutdown(1)
    xport.close()


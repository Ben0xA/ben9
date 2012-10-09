#!/usr/bin/python

import socket
import getpass
import time

# create socket object
xport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

# send message to server; fail quietly if server is not running
try:
  xport.connect(('localhost',4444))
  xport.setblocking(0)
  # export in psv time|username|ip|machine_name
  msg = getpass.getuser()
  xport.send(msg.encode())
except:
  print "failcamping"

# close socket
xport.shutdown(1)
xport.close()


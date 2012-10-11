#!/usr/bin/python

import socket
import getpass
import time

# create socket object
xport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
rslt = 0

# send message to server; fail quietly if server is not running
try:
    xport.connect(('localhost',4444))
    xport.setblocking(0)
    # export in psv time|username|ip|machine_name
    msg = time.strftime("%m/%d/%Y %H:%M:%S",time.localtime()) + "|"
    msg += getpass.getuser() + "|" 
    msg += str(socket.gethostbyname(socket.gethostname())) + "|"
    msg += socket.gethostname()
    rslt = xport.send(msg.encode())
except:
  pass # silent fail

# close socket
if rslt != 0: # only close the socket if it was open to begin with
    xport.shutdown(1)
    xport.close()


#!/usr/bin/python

import socket
import getpass
import time
import os

# create socket object
xport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
rslt = 0

# send message to server; fail quietly if server is not running
try:
    xport.connect(('192.168.135.35',4444))
    # export in psv time|username|ip|machine_name
    msg = time.strftime("%m/%d/%Y %H:%M:%S",time.gmtime()) + '|' + getpass.getuser() + '|'
    msg += socket.gethostbyname(socket.gethostname()) + '|' + os.getenv('HOSTNAME')
    rslt = xport.send(msg.encode())
except:
    pass

# close socket
if rslt != 0: # only close the socket if it was open to begin with
    xport.shutdown(1)
    xport.close()


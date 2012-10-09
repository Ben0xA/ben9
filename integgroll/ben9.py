#!/usr/bin/python

import socket
import getpass
import time
import os

# create socket object
xport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

# send message to server; fail quietly if server is not running
try:
    xport.connect(('localhost',4444))
    # export in psv time|username|ip|machine_name
    msg = time.strftime("%d/%m/%Y  %H:%M:%S",time.gmtime()) + "|" + getpass.getuser() + "|"
    msg += xport.gethostbyname(xport.gethostname()) + "|" + os.getenv('HOSTNAME')
except:
    pass

# close socket
xport.shutdown(1)
xport.close()


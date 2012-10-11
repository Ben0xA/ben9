#!/usr/bin/python

from optparse import OptionParser
from sys import exit
import socket
import time
import select
import threading

# variable declaration
kp = ""
timeout = 1

def input():
    global kp
    kp = raw_input()
    exit(1)

thd = threading.Thread(target=input)
thd.start()
thd.join(2)

# handle command-line options for ben9-listener
usageString = "Usage: %prog [-l log_file]"
parser = OptionParser(usage=usageString)

parser.add_option("-l", "--logfile", dest="logfile", metavar="LOGFILE", type="str", help="Name of file in which to record logged runs of ben9.py client.  Not required; prints log to standard output without this option set.")

(opts,args) = parser.parse_args()

# open up the logfile, if user opts to use one
if opts.logfile:
  try:
    logfile = open(opts.logfile,"a")
    logfile.write("\nben9-listener started at " + time.strftime("%d/%m/%Y  %H:%M:%S",time.gmtime()) + "\n")
  except:
    # changed opts.logfile to str(opts.logfile) for str concatenation error with null object
    print "Error writing to log file: " + str(opts.logfile)
    exit(1)

print "[*]Starting listener service."
# create and configure listening socket
inport = socket.socket(socket.AF_INET,socket.SOCK_STREAM)

inport.bind(("localhost",4444))
inport.setblocking(0)
inport.listen(5)
print "[*]Server started."
print "[*]Type q [enter] to stop server."
print "ServerTime|RemoteIP|RemoteName|ClientTime|ClientUser|ClientIP|ClientName"

# wait for responses from ben9.py client
while kp.lower() != 'q':
  readers,writers,errors = select.select([inport],[],[], timeout)
  if readers:      
    for read in readers:
      csocket, address = read.accept()
      cltext = csocket.recv(300)
      text = time.strftime("%m/%d/%Y %H:%M:%S",time.localtime()) + "|" 
      text += address[0] + "|" + str(socket.gethostbyaddr(address[0])[0]) + "|"
      text += cltext

      if opts.logfile:
        logfile.write(text + "\n")
      else:
        print text

print "[*]Server stopped."
# Memtool

A console tool for reading, writing, and search another process's memory heap.
Currently supports 4-byte integers, 1-byte bytes, and strings of multiple encoding.

## Example
To start, run the program.  A small help menu appears.

![](http://i.imgur.com/7hPk3qU.png)

Okay, so we want to search for the process ID of our process, for example, we'll take a look at ZSNES, a super nintendo emulator. We need to run zsnes first, of course.

![](http://i.imgur.com/npfRQ4W.png)

A simple search by the name found the id of 10160 very quickly.
Alright, so let's search for 'NETPLAY', which is one of the menubar options. We need to change our encoding type to default, as this application does not use Unicode. Try both if you're not sure.

![](http://i.imgur.com/iUB1vf0.png)

And there is our address.  Let's try to edit it with write memory.

![](http://i.imgur.com/m1ZMkg4.png)

Sometimes this fails, and that could be for a number of reasons. Usually it's due to a protected memory location.
In this case, it works just fine. Let's take a look at our zsnes application.

![](http://i.imgur.com/5rsqBH8.png)

And we edited some memory to change 'netplay' over to 'banana!'.
We can double-check our memory by calling the read memory method as well.

![](http://i.imgur.com/sJNNst6.png)

And notice on the right we can see 'banana!' being displayed in the print-out.

Have fun!


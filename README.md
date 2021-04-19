# alex-latency-window
Mathematical proof that true transaction order is unknowable within the latency width of a trustless network.

Written to support the thesis in 'Targeting Zero MEV - A Content Layer Solution' that randomizing transaction order within the latency width of a trustless network is ideal.

For an explanation, consider this idealized view of the network:
- Nodes are distributed around the world and latency is linear to the geographic distance between two nodes.
- Each node sends one transaction to the pool over a 10 second period.
- In this model we know precisely when each transaction was sent (in the real world this is impossible to know with zero trust).
- For each node and each transaction we compare the node's view of the transaction timestamp (arrival time) with the objective view (send time).
- We then calculate the mean and standard deviation of the error term.
- This tells us how accurately any node can know the true objective transaction order. 

The result is proof that the mean +  stdev of the error term is equivalent to the latency width.

Sample output:
```
error term of each node's view of every transaction timestamp:
avg error = 521.4325689391673 ms
stdev error = 577.3502691896257 ms
avg + stddev = 1000 ms <<< true timestamps are unknowable within this time
latency width = 1000 ms <<< the above is equivalent to the latency width
which is mathematical proof that randomizing transaction order within the latency width of a trustless network does not lose information
press any key to exit
```

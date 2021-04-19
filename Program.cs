/// MIT License
/// Copyright © 2021 pmcgoohan
/// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
/// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace AlexLatencyWindow
{
    /// <summary>
    /// Mathematical proof that true transaction order is unknowable within the latency width of a trustless network.
    /// Written to support the thesis in 'Targeting Zero MEV - A Content Layer Solution' that randomizing transaction order within the latency width of a trustless network is ideal.    
    ///
    /// For an explanation, consider this idealized view of the network:
    /// - Nodes are distributed around the world and latency is linear to the geographic distance between two nodes.
    /// - Each node sends one transaction to the pool over a 10 second period.
    /// - In this model we know precisely when each transaction was sent (in the real world this is impossible to know with zero trust).
    /// - For each node and each transaction we compare the node's view of the transaction timestamp (arrival time) with the objective view (send time).
    /// - We then calculate the mean and standard deviation of the error term.
    /// - This tells us how accurately any node can know the true objective transaction order.    
    /// </summary>
    class Program
    {
        // parameter constants (you can fiddle with these)
        const int GlobalNodeWidthHeight = 100; // width and height of the world in nodes 
        const double GlobalLatencyMs = 1000; // max latency between the two furthest nodes
        const double TxnSendWindowMs = 10000; // the maximum age of every transaction sent in the mempool

        // derived constants
        const double LatencyStepMs = GlobalLatencyMs / GlobalNodeWidthHeight; // ms latency between adjacent nodes
        const int GlobalTotalNodes = GlobalNodeWidthHeight * GlobalNodeWidthHeight; // the count of all nodes in the world
        const double TxnSendStepMs = TxnSendWindowMs / GlobalTotalNodes;

        static void Main(string[] args)
        {
            IdealizedLatencyWindow();
            Console.WriteLine("press any key to exit");
            Console.ReadKey();
        }

        static void IdealizedLatencyWindow()
        {
            int txnNum = 0;
            double sumError = 0;
            double sumErrorCount = 0;
            double sumErrorSquared = 0;

            // send a transaction for for each node
            for (int x = 0; x < GlobalNodeWidthHeight; x++)
            {
                for (int y = 0; y < GlobalNodeWidthHeight; y++)
                {
                    // space our transaction sends out to fill the transaction send width
                    double txnSendTimestampMs = txnNum * TxnSendStepMs; // send time (objective)

                    // and see when it arrives at each other node
                    for (int x2 = 0; x2 < GlobalNodeWidthHeight; x2++)
                    {
                        for (int y2 = 0; y2 < GlobalNodeWidthHeight; y2++)
                        {
                            // ignore yourself
                            if (x == x2 && y == y2) continue;

                            // calculate the distance between this node and the originator
                            double b = Math.Abs(x2 - x);
                            double p = Math.Abs(y2 - y);
                            double d = Math.Sqrt(b * b + p * p); // pythagoras's theorem

                            // convert distance into latency
                            double latencyMs = d * LatencyStepMs;

                            // recieve time (subjective)
                            double txnRecieveTimestampMs = txnSendTimestampMs + latencyMs;

                            // error term (here is our proof that the error term is equivalent to latency- so let's spell it out)
                            double error = txnRecieveTimestampMs - txnSendTimestampMs;
                            sumError += error;
                            sumErrorCount++;
                            sumErrorSquared += error * error;
                        }
                    }

                    // next transaction
                    txnNum++;
                }
            }

            double avgErr = sumError / sumErrorCount;
            double variance = sumErrorSquared / sumErrorCount;
            double stdevErr = (double)Math.Sqrt((double)variance);

            Console.WriteLine("error term of each node's view of every transaction timestamp:", sumError / sumErrorCount);
            Console.WriteLine("avg error = {0} ms", avgErr);
            Console.WriteLine("stdev error = {0} ms", stdevErr);
            Console.WriteLine("avg + stddev = {0} ms <<< true timestamps are unknowable within this time", GlobalLatencyMs);
            Console.WriteLine("latency width = {0} ms <<< the above is equivalent to the latency width", GlobalLatencyMs);
            Console.WriteLine("which is mathematical proof that randomizing transaction order within the latency width of a trustless network does not lose information");
        }
    }
}
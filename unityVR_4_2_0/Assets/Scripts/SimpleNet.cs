/** \mainpage A simple-to-use networkinterface that uses ProtoBuf and TCP/IP, it connect Unity (C#/Windows) and ANNarchy (C++/Linux)
 *  \author
 *    Marcel Richter
 *
 *    \version    1.3
 *
 * @section apr1 General idea
 * The receiving and sending are separated in two extra threads.
 * The function controller is running on an own background thread too.
 * It accepts incomming new clients (at the moment, only one connected client is allowed).
 * The send function puts the data in a MsgObject and in a buffer.
 * The data is then sent in the background.
 *      
 * If data is received, the background thread for receiving puts the data into a buffer.
 * The user gets the data by calling the receive function.
 * It is important to call the stop function to abort all background threads!
 *
 * @section apr2 How to add new Msg-types
 * - 1) Create new class and do not forget the Protobuf attributes.
 * - 2) Add class to the MsgObject-class in such a way that this class knows how to handle the new datatype.
 * - 3) Add the new Message to the Protofile. For more details <A HREF="../networkSpecification/tex/AddingMessages.pdf">look here</a>.
 * - 4) Update the "messageSpecification.tex" and "timeflowMessage.odt".
 *
 * @section apr3 Design-hints:
 * - Blackbox, Nobody cares what is inside! -> Simple Interface 
 * - Inside :  
 *     - 3 Threads controlling, sending , reciving
 *     - MsgImages has lower sending priority than the other MsgX
 *
 * @section apr5 Development environment:
 * - <A HREF="../../VR-doc/html/index.html#apr4">See</a>
 *
 * @section apr6 Message overview and message protocoll
 * - The description of each message could be found via <A HREF="../networkSpecification/tex/messageSpecification.pdf">networkSpecification/tex/messageSpecification.pdf</a>.
 * - The protocoll between VR and agent is asynchronous, hence the agent sends a movement, which is executed over several steps and for which
 *   MsgActionExecution Messages are sent. They first encode that the action is executing, and at the end that the action is finished.
 * - The timeflow of such a message protokoc is described in <A HREF="../timeflowMessage.pdf">timeflowMessage.pdf</a>
 *      
 * @section apr4 Installation:
 * - If you recompile, everything should be copied automatically into the right place.
 * - If manually: copy simplenet.dll into the folder: unityVR/assets/plugins 
 * 
 * @section apr7 Class diagramm:
 *
 * \image html SimpleNetClassdiagramR2.png
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using ProtoBuf;

namespace SimpleNetwork
{
    /** @brief I/O class to receive data from the virtual reality (VR) into ANNarchy (or C++ in general).
    *
    * @details
    * - Created on: November 2011
    * - Author: Marcel Richter, Robert Baruck
    */

    public class SimpleNet
    {
        TcpListener TCPListener;
        TcpClient Client;
        NetworkStream ClientDataStream;

        Queue<MsgObject> SimpleMsgOut = new Queue<MsgObject>();
        Queue<MsgObject> ImagesOut = new Queue<MsgObject>();

        Queue<MsgObject> MsgInbox = new Queue<MsgObject>();

        System.Threading.Thread ConntrolThread;
        System.Threading.Thread ReceiveThread;
        System.Threading.Thread SendThread;

        // Logging
        public bool logSimpleNet { get; private set; }
        private System.IO.StreamWriter file = null;

        /// <summary>
        /// Creates an instance of the SimpleNetwork.
        /// </summary>
        /// <param name="IP">Optional:IP the server listens to. (default 127.0.0.1)</param>
        /// <param name="Port">Optional Port to wait for incoming connections (default 1337)</param>
        public SimpleNet(string IP, int Port, bool logSimpleNet = false)
        {
            this.logSimpleNet = logSimpleNet;
            if (logSimpleNet)
            {
                file = new System.IO.StreamWriter("log_SimpleNet" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Port + ".txt");
                file.WriteLine("Start logging\n");
                file.Flush();
            }

            TCPListener = new TcpListener(IPAddress.Parse(IP), Port);
            TCPListener.Start();
            ConntrolThread = System.Threading.Thread.CurrentThread;

            //im Hintergrund laufen immer diese Prodzeduren
            ConntrolThread= Run(Controler); //Im Hintergrud läuft immer diese Prozedur
            SendThread = Run(BackgroundDatasendingLoop);
            ReceiveThread = Run(BackroundRecivingLoop);
        }
        
        //mehr bilder zwichen Fehlermeldung wenn nicht alle bilder übermittelt werden können
        //fehler bei überlastung generell TODO

        /// <summary>
        /// Sends a Msg over the network to the client. Function takes care of identification of the Data, and "builds" an
        /// corresponding MsgObject that is enqueued for sending over the network in a background thread.
        /// </summary>
        /// <param name="Object">Any instance of an MsgX-class</param>
        public void Send(object Object)
        {
            if (IsConnected)
            {
                MsgObject MyMsgObject = new MsgObject(Object);

                //Bilder Kommen in eigene Queue
                if (Object.GetType() == typeof(MsgImages))
                {
                    lock (ImagesOut)
                    {
                        ImagesOut.Enqueue(MyMsgObject);
                        while (ImagesOut.Count > 2)//Wir Puffern nur 2 Bilder
                        {
                            ImagesOut.Dequeue();
                        }
                        return;
                    }
                }

                lock (SimpleMsgOut)
                {
                    SimpleMsgOut.Enqueue(MyMsgObject);
                }
            }
        }

        /// <summary>
        /// Takes a MsgObject from the queue of available revived data. Waits if there is no data available!
        /// </summary>
        /// <returns>First MsgObject in Queue</returns>
        public MsgObject Receive()
        {
            while (MsgInbox.Count == 0)
            {
                System.Threading.Thread.Sleep(1); //Pop wartet bis Daten da sind, es ist blockierend!
            }
            
            lock (MsgInbox)
            {
               return MsgInbox.Dequeue();
            }
        }

        /// <summary>
        /// Indicates whether there were messages received.
        /// </summary>
        /// <returns>true if Data available</returns>
        public bool MsgAvailable()
        {
            return MsgInbox.Count != 0;
        }

        /// <summary>
        /// Checks whether there is a client connected.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Client != null && Client.Connected;
            }
        }
        
        /// <summary>
        /// Actual sending of data over the wire is managed by this function in the background. It takes the data from the SimpleMsg- and Image-OutBox. SimpleMsg are allways sent first.
        /// </summary>
        private void BackgroundDatasendingLoop()
        {
            while (true)
            {
                if (die)
                    return;
                try
                {

                    //Send Simple Data to Client
                    if (IsConnected && SimpleMsgOut.Count > 0)
                    {
                        MsgObject MsgToSend;
                        lock (SimpleMsgOut)
                        {
                            MsgToSend = SimpleMsgOut.Dequeue();
                        }
                        Serializer.SerializeWithLengthPrefix<MsgObject>(ClientDataStream, MsgToSend, PrefixStyle.Fixed32);
                        continue;
                    }

                    //Send Images to Client
                    if (IsConnected && ImagesOut.Count > 0)
                    {
                        MsgObject MsgToSend;
                        lock (ImagesOut)
                        {
                            MsgToSend = ImagesOut.Dequeue();
                        }
                        Serializer.SerializeWithLengthPrefix<MsgObject>(ClientDataStream, MsgToSend, PrefixStyle.Fixed32);
                        continue;
                    }

                    //Nothing to do, release the CPU
                    System.Threading.Thread.Sleep(1);
                    //Console.WriteLine("Tick!");
                }
                catch(Exception e) 
                { 
                    Console.WriteLine(e.Message);
                    if (logSimpleNet)
                    {
                        file.WriteLine("Catched Exception!");
                        file.WriteLine(e.ToString()); // catch protobuf exception
                        file.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// Actual receiving of data over the wire is managed by this function in the background. It fills the MsgInbox-Queue.
        /// </summary>
        private void BackroundRecivingLoop()
        {
            while (true)
            {
                if (die)
                    return;
                try
                {
                    //Progress Data from Client
                    if (IsConnected)
                    {
                        while (ClientDataStream.DataAvailable)
                        {

                            Console.WriteLine("Starte Empfangen!");
                            if (logSimpleNet)
                            {
                                file.WriteLine("Starte Empfangen!"); //DEBUG
                            }
                            MsgObject Buffer;
							Buffer = Serializer.DeserializeWithLengthPrefix<MsgObject>(ClientDataStream, PrefixStyle.Fixed32);
                            //in spez rein das gleich deserialisiert wird TODO
                            Console.WriteLine("Empfangen fertig");
                            if (logSimpleNet)
                            {
                                file.WriteLine("Empfangen fertig"); //DEBUG
                            }
							
                            lock (MsgInbox)
                            {
                                MsgInbox.Enqueue(Buffer);
                            }
                        }
                    }

                    //Nothing to Do, release the CPU
                    System.Threading.Thread.Sleep(1);
                    //Console.WriteLine("Tick!");
                }
                catch(Exception e)
				{
                    if (logSimpleNet)
                    {
                        file.WriteLine("Catched Exception!");
                        file.WriteLine(e.ToString()); // catch protobuf exception
                        file.Flush();
                    }
				}
            }
        }

        /// <summary>
        /// Handels incoming connections.
        /// </summary>
        private void Controler()
        {
            while (true)
            {
                if (die)
                    return;
                try
                {

                    //Take incoming Connections
                    if (TCPListener.Pending())
                    {
                        Console.WriteLine("Verbindungsaufbau");
                        if (logSimpleNet)
                        {
                            file.WriteLine("Verbindungsaufbau"); //DEBUG
                        }
                        Client = TCPListener.AcceptTcpClient();
                        ClientDataStream = Client.GetStream();
                        Client.ReceiveBufferSize = 4096;//Every Msg should fit Inside at ones, Ram is Cheap ;)
                        Client.NoDelay = true;
                        //in spezifikation TODO
                    }

                    //Nothing to Do, release the CPU
                    System.Threading.Thread.Sleep(1);
                    //Console.WriteLine("Tick!");
                }
                catch (Exception e) 
                {
                    if (logSimpleNet)
                    {
                        file.WriteLine("Catched Exception!");
                        file.WriteLine(e.ToString()); // catch protobuf exception
                        file.Flush();
                    }
                }
            }
        }

        /// <summary>
        /// If set to true, all the background threads should stop :)
        /// </summary>
        volatile bool die = false;

        /// <summary>
        /// Stops all concurrent threads and closes network connections.
        /// Important to call this function when the programm is terminated to stop all background threads!
        /// </summary>
        public void Stop()
        {
            //We had some problems with freezing becouse of the Network not shutting down
            //so we do everything possible to shut everything down!
            die = true;
            ConntrolThread.Abort();
            SendThread.Abort();
            ReceiveThread.Abort();
            TCPListener.Stop();
            if(Client!=null)
                Client.Close();
            if(ClientDataStream!=null)
                ClientDataStream.Close();
            if (logSimpleNet)
            {
                file.Close();
            }
        }

        /// <summary>
        /// utillity to start Parallel Threats
        /// </summary>
        /// <param name="TheDelagate">Delagate to invoke</param>
        /// <returns></returns>
        static System.Threading.Thread Run(System.Threading.ThreadStart TheDelagate)
        {
            System.Threading.Thread thread = new System.Threading.Thread(TheDelagate);
            thread.Start();
            return thread;
        } 
        
    } 
}

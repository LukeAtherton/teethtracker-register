using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace teethtracker_register {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private DataSet dataSet;
        private DataTable dataTable;

        const int BUFFER_SIZE = 1024;
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        const int DefaultTimeout = 5 * 1000;

        public MainWindow() {
            InitializeComponent();
            Loaded += onLoad;
        }

        // Abort the request if the timer fires.
        private static void TimeoutCallback(object state, bool timedOut) {
            if (timedOut) {
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null) {
                    request.Abort();
                }
            }
        }

        private void onLoad(object sender, RoutedEventArgs e) {
            dataSet = new DataSet();
            dataTable = new DataTable("Device");

            // Create a new DataTable and set two DataColumn objects as primary keys.
            DataColumn[] keys = new DataColumn[1];
            DataColumn column;

            // Create column 1.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Device Name";

            // Add the column to the DataTable.Columns collection.
            dataTable.Columns.Add(column);

            // Create column 3.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "Phone Number";

            // Add the column to the DataTable.Columns collection.
            dataTable.Columns.Add(column);

            // Create column 2 and add it to the array.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.String");
            column.ColumnName = "MAC Address";
            column.ReadOnly = true;

            dataTable.Columns.Add(column);

            // Add the column to the array.
            keys[0] = column;

            // Create column 2 and add it to the array.
            column = new DataColumn();
            column.DataType = System.Type.GetType("System.Boolean");
            column.ColumnName = "Synced?";

            dataTable.Columns.Add(column);

            // Set the PrimaryKeys property to the array.
            dataTable.PrimaryKey = keys;

            // add a ColumnChanged event handler for the table.
            //dataTable.ColumnChanged += new DataColumnChangeEventHandler(column_Changed);

            dataSet.Tables.Add(dataTable);

            dataTable.ColumnChanged += new DataColumnChangeEventHandler(datacolumn_Changed);

            dataGrid.DataContext = dataSet.Tables[0];

            //dataGrid.ItemsSource = dataSet.Tables[0].DefaultView;

            // Create a new XmlDocument  
            XmlDocument doc = new XmlDocument();

            // Load data  
            doc.Load("http://teethtracker.heroku.com/devices.xml");

            // Get nodes
            XmlNodeList nodes = doc.SelectNodes("/devices/device");

            foreach (XmlNode node in nodes) {
                dataTable.Rows.Add(node.SelectSingleNode("device-name").InnerText,
                            node.SelectSingleNode("local-device-number").InnerText,
                            node.SelectSingleNode("bluetooth-id").InnerText,
                            true);
            }

            //buildTree();
        }

        private void Button_Click_Search(object sender, EventArgs e) {
            DiscoDevicesAsync();
            btnSearch.IsEnabled = false;
            btnClear.IsEnabled = false;
        }

        public void DiscoDevicesAsync() {

            BluetoothComponent btComponent = new BluetoothComponent();
            btComponent.DiscoverDevicesProgress += HandleDiscoDevicesProgress;
            btComponent.DiscoverDevicesComplete += HandleDiscoDevicesComplete;
            btComponent.DiscoverDevicesAsync(255, true, true, true, false, 99);
        }

        private void HandleDiscoDevicesProgress(object sender, DiscoverDevicesEventArgs e) {

            foreach (BluetoothDeviceInfo device in e.Devices) {

                object[] findTheseVals = new object[1];
                findTheseVals[0] = device.DeviceAddress.ToString();
                DataRow foundRow = dataTable.Rows.Find(findTheseVals);

                if (foundRow == null) {
                    dataTable.Rows.Add(device.DeviceName, "", device.DeviceAddress.ToString());
                }
            }
        }

        private void buildTree() {
            try {
                // Initialize the TreeView control.
                //nodeTree.Nodes.Clear();
                //TreeNode tNode = new TreeNode();

                XmlDocument cDom = new XmlDocument();
                cDom.LoadXml("<nodes></nodes>");

                XDocument devMovements = XDocument.Load("http://teethtracker.heroku.com/device_movements.xml");
                nodeTree.DataContext = devMovements;
                //XDocument devMovements = XDocument.Load("test.xml");

                var nodeList = from nodeStation in devMovements.Descendants("device-movements").Descendants("device-movement").Descendants("node") select nodeStation.Value;
                foreach (var nodeName in nodeList.Distinct()) {
                    XmlNode nodeElement = cDom.CreateNode(XmlNodeType.Element, nodeName.ToString(), "");

                    var deviceList = from deviceStation
                                  in devMovements.Descendants("device-movements").Descendants("device-movement").Elements("device-bluetooth-id")
                                     select deviceStation.Value;

                    foreach (var deviceName in deviceList.Distinct()) {
                        var locationChanges = (from c in devMovements.Descendants("device-movements").Descendants("device-movement")
                                               where String.Compare((string)c.Element("device-bluetooth-id"), deviceName.ToString()) == 0 && (string)c.Element("movement-type") == "arrival"
                                               orderby (string)c.Element("updated-at")
                                               select (string)c.Element("node").Value);

                        string currentLocation = locationChanges.Last();

                        if (String.Compare(currentLocation, nodeName.ToString()) == 0) {
                            XmlNode deviceElement = cDom.CreateTextNode(deviceName.ToString());
                            nodeElement.AppendChild(deviceElement);
                        }
                    }

                    cDom.DocumentElement.AppendChild(nodeElement);
                }

               // nodeTree.Nodes.Add(new TreeNode(cDom.DocumentElement.Name));
                //tNode = nodeTree.Nodes[0];
                //AddNode(cDom.DocumentElement, tNode);

                // Create a new XmlDocument  
                XmlDocument deviceDoc = new XmlDocument();
                XmlDocument deviceTreeDoc = new XmlDocument();

                deviceTreeDoc.LoadXml("<devices></devices>");

                // Load data  
                deviceDoc.Load("http://teethtracker.heroku.com/devices.xml");

                // Get nodes
                XmlNodeList deviceNames = deviceDoc.SelectNodes("/devices/device/device-name");
                foreach (XmlNode name in deviceNames) {
                    XmlNode newElem = deviceTreeDoc.CreateTextNode(name.InnerText);
                    newElem.InnerText = name.InnerText;
                    deviceTreeDoc.DocumentElement.AppendChild(newElem);
                }

                //nodeTree.Nodes.Add(new TreeNode(deviceTreeDoc.DocumentElement.Name));
                //tNode = nodeTree.Nodes[1];
                //AddNode(deviceTreeDoc.DocumentElement, tNode);

                //nodeTree.ExpandAll();
            } catch (XmlException xmlEx) {
                MessageBox.Show(xmlEx.Message);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        /*private void AddNode(XmlNode inXmlNode, TreeNode inTreeNode) {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList nodeList;
            int i;

            // Loop through the XML nodes until the leaf is reached.
            // Add the nodes to the TreeView during the looping process.
            if (inXmlNode.HasChildNodes) {
                nodeList = inXmlNode.ChildNodes;
                for (i = 0; i <= nodeList.Count - 1; i++) {
                    xNode = inXmlNode.ChildNodes[i];
                    inTreeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = inTreeNode.Nodes[i];
                    AddNode(xNode, tNode);
                }
            } else {
                // Here you need to pull the data from the XmlNode based on the
                // type of node, whether attribute values are required, and so forth.
                inTreeNode.Text = (inXmlNode.OuterXml).Trim();
            }
        }*/

        private void HandleDiscoDevicesComplete(object sender, DiscoverDevicesEventArgs e) {
            if (e.Cancelled) {
                Console.WriteLine("DiscoDevicesAsync cancelled.");
            } else if (e.Error != null) {
                Console.WriteLine("DiscoDevicesAsync error: {0}.", e.Error.Message);
            } else {
                Console.WriteLine("DiscoDevicesAsync complete found {0} devices.", e.Devices.Length);
                sendPhoneInfo();
            }
            btnSearch.IsEnabled = true;
            btnClear.IsEnabled = true;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e) {
            buildTree();
        }

        private void FinishWebRequest(IAsyncResult asynchronousResult) {
            try {
                // State of request is asynchronous.
                RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
                HttpWebRequest myHttpWebRequest = myRequestState.request;
                myRequestState.response = (HttpWebResponse)myHttpWebRequest.EndGetResponse(asynchronousResult);

                // Read the response into a Stream object.
                Stream responseStream = myRequestState.response.GetResponseStream();
                myRequestState.streamResponse = responseStream;

                string id = myRequestState.bluetoothID;

                if (id != null) {

                    object[] findTheseVals = new object[1];
                    findTheseVals[0] = id;
                    DataRow foundRow = dataTable.Rows.Find(findTheseVals);

                    if (foundRow != null) {
                        foundRow.SetField(3, true);
                    }
                }

                // Begin the Reading of the contents of the HTML page and print it to the console.
                IAsyncResult asynchronousInputRead = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                return;

            } catch (WebException e) {
                Console.WriteLine("\nRespCallback Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();
        }

        private static void ReadCallBack(IAsyncResult asyncResult) {
            try {

                RequestState myRequestState = (RequestState)asyncResult.AsyncState;
                Stream responseStream = myRequestState.streamResponse;
                int read = responseStream.EndRead(asyncResult);
                // Read the HTML page and then print it to the console.
                if (read > 0) {
                    myRequestState.requestData.Append(Encoding.ASCII.GetString(myRequestState.BufferRead, 0, read));
                    IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
                    return;
                }/* else {
                    Console.WriteLine("\nThe contents of the Html page are : ");
                    if (myRequestState.requestData.Length > 1) {
                        string stringContent;
                        stringContent = myRequestState.requestData.ToString();
                        Console.WriteLine(stringContent);
                    }
                    Console.WriteLine("Press any key to continue..........");
                    Console.ReadLine();*/

                responseStream.Close();
                //}

            } catch (WebException e) {
                Console.WriteLine("\nReadCallBack Exception raised!");
                Console.WriteLine("\nMessage:{0}", e.Message);
                Console.WriteLine("\nStatus:{0}", e.Status);
            }
            allDone.Set();

        }

        private void Button_Click_Clear(object sender, RoutedEventArgs e) {
            dataTable.Clear();
            string url = "http://teethtracker.heroku.com/clear-database";
            doWebRequest(url, new RequestState());
        }

        private void datacolumn_Changed(object sender, DataColumnChangeEventArgs e) {
            if (e.Column.ColumnName == "Phone Number" || e.Column.ColumnName == "Device Name") {
                e.Row[3] = false;
                sendPhoneInfo();
            }
        }

        private void doWebRequest(string url, RequestState myRequestState) {

            Console.WriteLine("Sending Request: {0}", url);

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                myRequestState.request = request;

                // Start the asynchronous request.
                IAsyncResult result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(FinishWebRequest), myRequestState);

                // this line implements the timeout, if there is a timeout, the callback fires and the request becomes aborted
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), myRequestState, DefaultTimeout, true);

                // The response came in the allowed time. The work processing will happen in the 
                // callback function.
                allDone.WaitOne();

                // Release the HttpWebResponse resource.
               // myRequestState.response.Close();

            } catch (WebException exception) {
                Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("\nMessage:{0}", exception.Message);
                Console.WriteLine("\nStatus:{0}", exception.Status);
            } catch (Exception exception) {
                Console.WriteLine("\nMain Exception raised!");
                Console.WriteLine("Source :{0} ", exception.Source);
                Console.WriteLine("Message :{0} ", exception.Message);
            }
        }

        private void sendPhoneInfo() {
            foreach (DataRow row in dataTable.Rows) {
                string name = row[0].ToString();
                string number = row[1].ToString();
                string id = row[2].ToString();

                string url = "http://teethtracker.heroku.com/devices/new?"
                                                            + "bluetooth_id=" + id
                                                            + "&name=" + name
                                                            + "&number=" + number;

                doWebRequest(url, new RequestState(id));
            }
        }

    }
}

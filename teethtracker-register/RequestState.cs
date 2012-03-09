using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace teethtracker_register {
     class RequestState {
        // This class stores the State of the request.
        const int BUFFER_SIZE = 1024;
        public StringBuilder requestData;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream streamResponse;
        public string bluetoothID;
        public RequestState() {
            BufferRead = new byte[BUFFER_SIZE];
            requestData = new StringBuilder("");
            request = null;
            streamResponse = null;
        }
        public RequestState(string id)
            : this() {
            bluetoothID = id;
        }
    }
}

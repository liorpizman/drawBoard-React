using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using draw_board.Classes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace draw_board.Controllers
{
    /// <summary>
    /// BoardController which manages all the activity on the board in the server-side according to client-side calls and vice versa
    /// </summary>
    [Route("api/[controller]")]
    public class BoardController : Controller
    {
        db DB = new db();

        /// <summary>
        /// Create a new path object to insert it into the database
        /// </summary>
        /// <param name="path">path data from client-side</param>
        /// <returns>new path object</returns>
        public static Path createPath(JArray path)
        {
            Point[] arr = new Point[path.Count];
            for (int i = 0; i < path.Count - 2; i++)
            {
                float x = path[i]["x"].ToObject<float>();
                float y = path[i]["y"].ToObject<float>();
                arr[i] = new Point(x, y);
            }
            return new Path(arr);
        }

        /// <summary>
        /// Inserts a new path to the database and returns its unique ID obtained there
        /// </summary>
        /// <param name="data">path data from client-side</param>
        /// <returns>unique ID</returns>
        [HttpPost("[action]")]
        public int addPath([FromBody] JObject data)
        {
            int pathId = -1;
            if (data.Count != 3)
                return -1;
            Object path = data.First;
            Object boardData = ((JProperty)path).Next;
            Object ip = data.Last.ToString();
            string cIP = "default";
            try
            {
                cIP = ((string)ip).Split(' ')[1];
            }
            catch (Exception e) { e.StackTrace.ToString(); }
            if (cIP.Equals("default"))
                cIP = ip.ToString();
            string boardName = ((JProperty)boardData).Value.ToString();
            if (path != null)
            {
                Path p = createPath(((JProperty)path).First as JArray);
                pathId = DB.insertPath(p, boardName, cIP);
            }
            return pathId;
        }

        /// <summary>
        /// Returns all paths that belong to the current draw board from the database
        /// </summary>
        /// <param name="boardname">current draw board</param>
        /// <returns>paths array</returns>
        [HttpPost("[action]")]
        public Path[] getPath([FromBody] JObject boardname)
        {
            string board = ((JProperty)boardname.First).Value.ToString();
            Path[] paths = DB.getPath(board);
            return paths;
        }

        /// <summary>
        /// Deletes a path from the database based on its unique identifier
        /// </summary>
        /// <param name="pathId">path unique identifier</param>
        [HttpPost("[action]")]
        public void deletePath([FromBody] int pathId)
        {
            DB.deletePathFromDB(pathId);
        }

        /// <summary>
        /// Returns the IP of the current user
        /// </summary>
        /// <returns>IP Address</returns>
        [HttpGet("[action]")]
        public Object getClientIp()
        {
            string ip = GetIpAddress();
            string IP = "";
            try
            {
                IP = GetIPAddress();
            }
            catch (Exception e)
            {
                e.StackTrace.ToString();
            }
            if (!String.IsNullOrEmpty(IP))
            {
                return IP.Replace(".", string.Empty);
            }
            return ip.Replace(".", string.Empty);
        }

        /// <summary>
        /// Get client's IP Address
        /// </summary>
        /// <returns>IP Address</returns>
        static string GetIPAddress()
        {
            String address = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            using (WebResponse response = request.GetResponse())
            using (System.IO.StreamReader stream = new System.IO.StreamReader(response.GetResponseStream()))
            {
                address = stream.ReadToEnd();
            }
            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);
            return address;
        }

        /// <summary>
        /// Get IP Address
        /// </summary>
        /// <returns>IP Address</returns>
        public static string GetIpAddress()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(GetCompCode());
            IPAddress[] addr = ipEntry.AddressList;
            string ip = addr[3].ToString();
            return ip;
        }

        /// <summary>
        /// Get Computer Name
        /// </summary>
        /// <returns>Name</returns>
        public static string GetCompCode()
        {
            string strHostName = Dns.GetHostName();
            return strHostName;
        }

    }
}
